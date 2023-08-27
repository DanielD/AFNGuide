using AfnGuideAPI.Models;
using TimeZone = AfnGuideAPI.Models.TimeZone;

namespace AfnGuideAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AfnGuideDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Channels.Any())
            {
                var channels = new[]
                {
                    new Channel
                    {
                        Id = 2,
                        Abbreviation = "ATL",
                        ChannelNumber = 2,
                        Color = "#eec3c3",
                        Image = "chnl_icon_primeATL.png",
                        Title = "AFN|prime Atlantic"
                    },
                    new Channel
                    {
                        Id = 5,
                        Abbreviation = "NEW",
                        ChannelNumber = 5,
                        Color = "#b4d1db",
                        Image = "chnl_icon_news.png",
                        Title = "AFN|news"
                    },
                    new Channel
                    {
                        Id = 1,
                        Abbreviation = "SPO",
                        ChannelNumber = 1,
                        Color = "#a3dcbc",
                        Image = "chnl_icon_sports.png",
                        Title = "AFN|sports"
                    },
                    new Channel
                    {
                        Id = 4,
                        Abbreviation = "PAC",
                        ChannelNumber = 4,
                        Color = "#eec3c3",
                        Image = "chnl_icon_primePAC.png",
                        Title = "AFN|prime Pacific"
                    },
                    new Channel
                    {
                        Id = 3,
                        Abbreviation = "SPE",
                        ChannelNumber = 3,
                        Color = "#c7ccf8",
                        Image = "chnl_icon_spectrum.png",
                        Title = "AFN|spectrum"
                    },
                    new Channel
                    {
                        Id = 6,
                        Abbreviation = "SP2",
                        ChannelNumber = 6,
                        Color = "#afecde",
                        Image = "chnl_icon_sports2.png",
                        Title = "AFN|sports2"
                    },
                    new Channel
                    {
                        Id = 8,
                        Abbreviation = "FAM",
                        ChannelNumber = 9,
                        Color = "#f6d5af,#e5c2ff",
                        Image = "chnl_icon_family.png,chnl_icon_pulse.png",
                        Title = "AFN|family,AFN|pulse",
                        IsSplit = true,
                        StartTime = "PT22H",
                        EndTime = "PT7H59M"
                    },
                    new Channel
                    {
                        Id = 9,
                        Abbreviation = "MOV",
                        ChannelNumber = 10,
                        Color = "#d1d0d1",
                        Image = "chnl_icon_movie.png",
                        Title = "AFN|movie"
                    }
                };
                foreach (var channel in channels)
                {
                    context.Channels.Add(channel);
                }
                context.SaveChanges();
            }

            if (!context.TimeZones.Any())
            {
                var timeZones = new[]
                {
                    new TimeZone(1, "International Dateline", null, -12, false),
                    new TimeZone(3, "Hawaii, Tahiti", null, -10, false),
                    new TimeZone(2, "Midway Island, Samoa", null, -11, false),
                    new TimeZone(97, "Apia Samoa", null, 13, false),
                    new TimeZone(98, "Marquesas Islands", null, -9.5, false),
                    new TimeZone(4, "Alaska", null, -9, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(5, "Pacific Time (US \u0026 Canada)", null, -8, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(10, "Mountain Time", null, -7, false),
                    new TimeZone(8, "Mountain, Chihuahua, Mazatlan", null, -7, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(11, "Central America", null, -6, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(15, "Central America", null, -6, false),
                    new TimeZone(12, "Central Time (US \u0026 Canada)", null, -6, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(16, "South America", null, -5, false),
                    new TimeZone(20, "Caracas", null, -4.5, false),
                    new TimeZone(18, "Cuba", null, -5, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(17, "Eastern Time (US \u0026 Canada)", null, -5, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(92, "Guantanamo Bay (GTMO)", null, -6, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(21, "Guyana, Brazil, Canada", null, -4, false),
                    new TimeZone(19, "Atlantic Time (Canada)", null, -4, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(22, "Santiago", null, -5, true, DateTime.Parse("9/3/2023 12:00 AM"), DateTime.Parse("4/2/2024 12:00 AM"), true),
                    new TimeZone(25, "Buenos Aires, Georgetown", null, -3, false),
                    new TimeZone(23, "Newfoundland", null, -3.5, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(24, "Brasilia", null, -3, false),
                    new TimeZone(100, "Brazil", null, -2, false),
                    new TimeZone(26, "Greenland", null, -3, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(30, "Cape Verde Is.", null, -1, false),
                    new TimeZone(29, "Azores", null, -1, true, DateTime.Parse("3/26/2023 2:00 AM"), DateTime.Parse("10/29/2023 2:00 AM")),
                    new TimeZone(32, "Coordinated Universal Time", "UTC", 0, false),
                    new TimeZone(95, "Dublin, Edinburgh, Lisbon, London", null, 0, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(37, "West Central Africa", null, 1, false),
                    new TimeZone(31, "Casablanca", null, 1, true, DateTime.Parse("4/23/2023 2:00 AM"), DateTime.Parse("3/19/2023 2:00 AM"), true),
                    new TimeZone(33, "Central Europe, Germany, Italy", "CET", 1, true, DateTime.Parse("3/12/2023 2:00 AM"), DateTime.Parse("11/5/2023 2:00 AM")),
                    new TimeZone(42, "South Africa, Libya, Egypt, East Europe", null, 2, false),
                    new TimeZone(47, "Iraq, Kuwait, Saudi Arabia, Eastern Africa", null, 3, false),
                    new TimeZone(50, "Tanzania, Nairobi", null, 3, false),
                    new TimeZone(45, "Eastern Europe, Gaza", null, 2, true, DateTime.Parse("4/29/2023 2:00 AM"), DateTime.Parse("10/28/2023 2:00 AM")),
                    new TimeZone(39, "Greece, Romania, Turkey", null, 2, true, DateTime.Parse("3/26/2023 2:00 AM"), DateTime.Parse("10/29/2023 2:00 AM")),
                    new TimeZone(44, "Jerusalem, Israel", null, 2, true, DateTime.Parse("3/28/2023 2:00 AM"), DateTime.Parse("10/29/2023 2:00 AM")),
                    new TimeZone(53, "Russia, United Arab Emirates, Oman", null, 4, false),
                    new TimeZone(52, "Iran", null, 3.5, false),
                    new TimeZone(54, "Baku, Armenia", null, 4, false),
                    new TimeZone(58, "Pakistan, Uzbekistan", null, 5, false),
                    new TimeZone(59, "India, Sri Lanka", null, 5.5, false),
                    new TimeZone(61, "Nepal", null, 5.75, false),
                    new TimeZone(63, "Russia, Bishkek, Manas, Diego Garcia", null, 6, false),
                    new TimeZone(64, "Yangon (Rangoon)", null, 6.5, false),
                    new TimeZone(65, "Russia, Thailand, Indonesia, Vietnam, Australia", null, 7, false),
                    new TimeZone(66, "Russia, China, Philippines, Singapore, Perth", null, 8, false),
                    new TimeZone(73, "Japan, Korea (JKT)", "JKT", 9, false),
                    new TimeZone(76, "Northern Territory Australia", null, 9.5, false),
                    new TimeZone(77, "Queensland, Yakutsk Russia, Guam, Port Moresby", null, 10, false),
                    new TimeZone(78, "Canberra, Melbourne, Sydney", null, 10, true, DateTime.Parse("10/1/2023 2:00 AM"), DateTime.Parse("4/2/2024 2:00 AM"), true),
                    new TimeZone(75, "Adelaide, South Australia", null, 8.5, true, DateTime.Parse("10/1/2023 2:00 AM"), DateTime.Parse("4/2/2024 2:00 AM"), true),
                    new TimeZone(82, "Vladivostok, New Caledonia, Solomon Islands", null, 11, false),
                    new TimeZone(86, "Kwajalein Atoll", null, 12, false),
                    new TimeZone(84, "Fiji", null, 12, true, DateTime.Parse("11/12/2023 2:00 AM"), DateTime.Parse("1/14/2024 3:00 AM"), true),
                    new TimeZone(83, "New Zealand", null, 11, true, DateTime.Parse("9/24/2023 2:00 AM"), DateTime.Parse("4/2/2024 3:00 AM"), true)
                };

                foreach (var timeZone in timeZones)
                {
                    context.TimeZones.Add(timeZone);
                }
                context.SaveChanges();
            }
        }

        public static IServiceCollection InitializeDatabase(this IServiceCollection serviceCollection)
        {
            using var scope = serviceCollection.BuildServiceProvider().CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AfnGuideDbContext>();
                Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<AfnGuideDbContext>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }

            return serviceCollection;
        }
    }
}