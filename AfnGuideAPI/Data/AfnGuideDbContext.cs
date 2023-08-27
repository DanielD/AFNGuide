using AfnGuideAPI.Models;
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
    }
}
