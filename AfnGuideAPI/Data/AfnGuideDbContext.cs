using AfnGuideAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using TimeZone = AfnGuideAPI.Models.TimeZone;

namespace AfnGuideAPI.Data
{
    public class AfnGuideDbContext : DbContext
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TimeZone> TimeZones { get; set; }
        public DbSet<Bulletin> Bulletins { get; set; }
        public DbSet<Promo> Promos { get; set; }

        public AfnGuideDbContext()
        {
        }

        public AfnGuideDbContext(DbContextOptions<AfnGuideDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Channel)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasAlternateKey(s => s.AfnId);

            modelBuilder.Entity<Promo>()
                .HasOne(p => p.Schedule)
                .WithOne(s => s.Promo)
                .HasForeignKey<Promo>(p => p.AfnId)
                .HasPrincipalKey<Schedule>(s => s.AfnId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public async Task<ScheduleSearchResults> ScheduleSearch(
            string[]? searchWords,
            int[]? channelIds,
            DateTime? startDate,
            DateTime? endDate,
            string[]? rating,
            string? searchField,
            string? searchPhrase,
            string[]? unwantedWords,
            CancellationToken cancellationToken = default
            )
        {
            using var connection = Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = "dbo.sp_SearchSchedules";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@SearchWords", SqlDbType.NVarChar, -1) { Value = searchWords == null ? DBNull.Value : string.Join("|", searchWords) });
            command.Parameters.Add(new SqlParameter("@Channels", SqlDbType.NVarChar, -1) { Value = channelIds == null ? DBNull.Value : string.Join(",", channelIds) });
            command.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate == null ? DBNull.Value : startDate });
            command.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = endDate == null ? DBNull.Value : endDate });
            command.Parameters.Add(new SqlParameter("@Rating", SqlDbType.NVarChar, -1) { Value = rating == null ? DBNull.Value : string.Join("|", rating) });
            command.Parameters.Add(new SqlParameter("@SearchField", SqlDbType.NVarChar, -1) { Value = searchField == null ? DBNull.Value : searchField });
            command.Parameters.Add(new SqlParameter("@SearchPhrase", SqlDbType.NVarChar, -1) { Value = searchPhrase == null ? DBNull.Value : searchPhrase });
            command.Parameters.Add(new SqlParameter("@UnwantedWords", SqlDbType.NVarChar, -1) { Value = unwantedWords == null ? DBNull.Value : string.Join("|", unwantedWords) });
            await connection.OpenAsync(cancellationToken);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var result = new ScheduleSearchResults();

            while(await reader.ReadAsync(cancellationToken))
            {
                result.TotalCount = reader.GetInt32(0);
            }
            await reader.NextResultAsync(cancellationToken);
            await foreach (var schedule in reader.CastAsAsync<Schedule>(Schedule.Create, cancellationToken))
            {
                result.Schedules.Add(schedule);
            }

            return result;
        }
    }
}
