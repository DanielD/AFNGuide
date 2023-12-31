﻿using AfnGuideAPI.Data.SearchEngines.Schedules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduleController(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache memoryCache)
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = _loggerFactory.CreateLogger<ScheduleController>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Searches the schedule for the given query.
        /// </summary>
        /// <param name="q">Comma separated words (required)</param>
        /// <param name="tz">TiemZoneId (required)</param>
        /// <param name="ch">Comma separated channels</param>
        /// <param name="sdt">Date rage start</param>
        /// <param name="edt">Date range end</param>
        /// <param name="r">Rating</param>
        /// <param name="f">Search field</param>
        /// <param name="ph">Search phrase</param>
        /// <param name="uw">Unwanted words, comma separated</param>
        /// <param name="sz">Page size (default is 10)</param>
        /// <param name="p">Page number (default is 1)</param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ViewModels.SearchResult<ViewModels.Schedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search(
            [FromQuery] string[] q, [FromQuery] int tz, [FromQuery] int[]? ch,
            [FromQuery] DateTime? sdt, [FromQuery] DateTime? edt,
            [FromQuery] string? r, [FromQuery] string? f, [FromQuery] string? ph,
            [FromQuery] string[]? uw, [FromQuery] int? sz = 10, [FromQuery] int? p = 1)
        {
            if ((q == null || q.Length == 0) && (ph == null || ph.Length == 0))
            {
                return BadRequest("Query (q) is required.");
            }
            if (tz < 1)
            {
                return BadRequest("TimeZoneId (tz) is required.");
            }
            var timeZone = await GetTimeZoneAsync(tz);
            if (timeZone == null)
            {
                return NotFound("TimeZoneId (tz) not found.");
            }

            try
            {
                p ??= 1;
                sz ??= 10;
                ch ??= (await GetChannelsAsync())!.Select(c => c.Id).ToArray();
                r ??= ScheduleRatings.Any;
                f ??= ScheduleSearchFields.All;
                ph ??= string.Empty;
                uw ??= Array.Empty<string>();
                q ??= Array.Empty<string>();

                uw = uw.Select(w => w.Replace("\"", string.Empty).Trim()).ToArray();

                var searchQuery = new Data.SearchEngines.Schedules.SearchQuery
                {
                    SearchWords = q,
                    Channels = ch,
                    StartDate = sdt,
                    EndDate = edt,
                    Rating = r,
                    SearchField = f,
                    SearchPhrase = ph,
                    UnwantedWords = uw
                };

                var cacheKey = $"ScheduleController.Search({JsonConvert.SerializeObject(searchQuery)})";
                var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                    using var searchEngine = new Data.SearchEngines.Schedules.SearchEngine(_loggerFactory, _serviceScopeFactory);
                    var result = await searchEngine.SearchAsync(searchQuery);

                    return result;
                });

                var totalCount = result!.TotalCount;
                if (totalCount == 0)
                {
                    return Ok(new ViewModels.SearchResult<ViewModels.Schedule>
                    {
                        Total = 0,
                        Page = 1,
                        Schedules = new List<ViewModels.Schedule>()
                    });
                }

                if (p > 1 && p > Math.Ceiling(Convert.ToDecimal(totalCount / sz)))
                {
                    return BadRequest($"Page number ({p}) is out of range.");
                }

                var output = new ViewModels.SearchResult<ViewModels.Schedule>
                {
                    Total = totalCount,
                    Page = p.Value,
                    Schedules = result.Schedules
                        .Skip((p!.Value - 1) * sz!.Value)
                        .Take(sz!.Value)
                        .Select(s => new ViewModels.Schedule
                        {
                            Id = s.AfnId,
                            Title = s.Title,
                            Description = s.Description,
                            EpisodeTitle = s.EpisodeTitle,
                            AirDateLocal = s.AirDateUTC.AddMinutes(timeZone.GetCurrentOffset()),
                            ChannelId = s.ChannelId,
                            Duration = s.Duration,
                            Rating = s.Rating,
                            Genre = s.Genre,
                            IsPremiere = s.IsPremiere,
                            Year = s.Year,
                        })
                    .ToList()
                };

                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching schedule.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("series")]
        [ProducesResponseType(typeof(List<ViewModels.TVSeries>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTVSeries()
        {
            try
            {
                var tvSeries = await GetTVSeriesAsync();

                var output = tvSeries!.Select(x => new ViewModels.TVSeries
                {
                    ChannelId = x.ChannelId,
                    Title = x.Name,
                    IsSplit = x.IsSplit,
                    PremiereType = x.PremiereType,
                    Season = x.Season,
                    StartDate = x.StartDate,
                });

                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting TV series.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}