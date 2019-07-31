using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Music;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using Newtonsoft.Json;

namespace EmbyStat.Services
{
    public class MusicService : MediaService, IMusicService
    {
        private readonly IMusicRepository _musicRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IPersonService _personService;
        private readonly ISettingsService _settingsService;
        private readonly IStatisticsRepository _statisticsRepository;

        public MusicService(IMusicRepository musicRepository, ICollectionRepository collectionRepository,
            IPersonService personService, ISettingsService settingsService,
            IStatisticsRepository statisticsRepository, IJobRepository jobRepository) : base(jobRepository)
        {
            _musicRepository = musicRepository;
            _collectionRepository = collectionRepository;
            _personService = personService;
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
        }

        public IEnumerable<Collection> GetMusicCollections()
        {
            var settings = _settingsService.GetUserSettings();
            return _collectionRepository.GetCollectionByTypes(settings.MovieCollectionTypes);
        }

        public async Task<MusicStatistics> GetMusicStatisticsAsync(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Movie, collectionIds);

            MusicStatistics statistics;
            if (StatisticsAreValid(statistic, collectionIds))
            {
                statistics = JsonConvert.DeserializeObject<MusicStatistics>(statistic.JsonResult);
            }
            else
            {
                statistics = await CalculateMusicStatistics(collectionIds);
            }

            return statistics;
        }

        public async Task<MusicStatistics> CalculateMusicStatistics(List<string> collectionIds)
        {
            var songs = _musicRepository.GetAll(collectionIds).ToList();

            var statistics = new MusicStatistics
            {
                General = CalculateGeneralStatistics(songs),
                Charts = CalculateCharts(songs),
                People = await CalculatePeopleStatistics(songs),
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Movie, collectionIds);

            return statistics;
        }

        private MusicGeneral CalculateGeneralStatistics(IReadOnlyCollection<Song> songs)
        {
            return new MusicGeneral
            {
                SongCount = TotalSongCount(songs),
                GenreCount = TotalMusicGenres(songs),
                //TotalPlayableTime = TotalPlayLength(songs),
               //HighestRatedSong = HighestRatedSong(songs),
               //LowestRatedSong = LowestRatedSong(songs),
                OldestReleasedSong = OldestReleasedSong(songs),
                YoungestReleasedSong = YoungestReleasedSong(songs),
                //ShortestSong = ShortestMovie(songs),
                //LongestSong = LongestMovie(songs),
                YoungestAddedSong = YoungestAddedSong(songs)
            };
        }

        public async Task<PersonStats> CalculatePeopleStatistics(IReadOnlyCollection<Song> songs)
        {
            return new PersonStats
            {
                TotalActorCount = TotalTypeCount(songs, PersonType.Actor, Constants.Common.TotalActors),
                TotalDirectorCount = TotalTypeCount(songs, PersonType.Director, Constants.Common.TotalDirectors),
                TotalWriterCount = TotalTypeCount(songs, PersonType.Writer, Constants.Common.TotalWriters),
                MostFeaturedActor = await GetMostFeaturedPersonAsync(songs, PersonType.Actor, Constants.Common.MostFeaturedActor),
                MostFeaturedDirector = await GetMostFeaturedPersonAsync(songs, PersonType.Director, Constants.Common.MostFeaturedDirector),
                MostFeaturedWriter = await GetMostFeaturedPersonAsync(songs, PersonType.Writer, Constants.Common.MostFeaturedWriter),
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenreAsync(songs)
            };
        }

        private MusicCharts CalculateCharts(IReadOnlyCollection<Song> songs)
        {
            var stats = new MusicCharts();
            stats.BarCharts.Add(CalculateGenreChart(songs));
            //stats.BarCharts.Add(CalculateRatingChart(songs.Select(x => x.CommunityRating)));
            stats.BarCharts.Add(CalculatePremiereYearChart(songs.Select(x => x.PremiereDate)));
            //stats.BarCharts.Add(CalculateOfficialRatingChart(songs));

            return stats;
        }

        public bool TypeIsPresent()
        {
            return _musicRepository.Any();
        }

        private IEnumerable<SongDuplicate> GetDuplicates(IReadOnlyCollection<Song> songs)
        {
            var list = new List<SongDuplicate>();

            var duplicatesByTitle = songs.Where(x => !string.IsNullOrWhiteSpace(x.OriginalTitle)).GroupBy(x => x.OriginalTitle).Select(x => new { x.Key, Count = x.Count() }).Where(x => x.Count > 1).ToList();
            for (var i = 0; i < duplicatesByTitle.Count; i++)
            {
                var duplicateMovies = songs.Where(x => x.OriginalTitle == duplicatesByTitle[i].Key).OrderBy(x => x.Id).ToList();
                var itemOne = duplicateMovies[0];
                var itemTwo = duplicateMovies[1];

                if (itemOne.Codec != itemTwo.Codec)
                {
                    continue;
                }

                list.Add(new SongDuplicate
                {
                    Number = i,
                    Title = itemOne.OriginalTitle,
                    Reason = Constants.ByTitle,
                    ItemOne = new SongDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = string.Join(string.Join(",", itemOne.Codec),string.Join(",",itemOne.BitRate.ToString())) },
                    ItemTwo = new SongDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = string.Join(string.Join(",", itemTwo.Codec),string.Join(",", itemOne.BitRate.ToString())) }
                });
            }

            return list.OrderBy(x => x.Title).ToList();
        }

        #region StatCreators

        private Card<int> TotalSongCount(IEnumerable<Song> songs)
        {
            return new Card<int>
            {
                Title = Constants.Music.TotalSongs,
                Value = songs.Count()
            };
        }

        private Card<int> TotalMusicGenres(IEnumerable<Song> songs)
        {
            return new Card<int>
            {
                Title = Constants.Music.TotalGenres,
                Value = songs.SelectMany(x => x.Genres)
                              .Distinct()
                              .Count()
            };
        }

        private SongPoster OldestReleasedSong(IEnumerable<Song> songs)
        {
            var song = songs.Where(x => x.PremiereDate != null)
                              .OrderBy(x => x.PremiereDate)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (song != null)
            {
                return PosterHelper.ConvertToSongPoster(song, Constants.Music.OldestPremiered);
            }

            return new SongPoster();
        }

        private SongPoster YoungestReleasedSong(IEnumerable<Song> songs)
        {
            var song = songs.Where(x => x.PremiereDate != null)
                              .OrderByDescending(x => x.PremiereDate)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (song != null)
            {
                return PosterHelper.ConvertToSongPoster(song, Constants.Music.YoungestPremiered);
            }

            return new SongPoster();
        }

        /*
        private SongPoster ShortestSong(IEnumerable<Song> songs)
        {
            var settings = _settingsService.GetUserSettings();
            var song = songs.Where(x => x.RunTimeTicks != null && x.RunTimeTicks >= settings.ToShortMovie)
                              .OrderBy(x => x.RunTimeTicks)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (song != null)
            {
                return PosterHelper.ConvertToSongPoster(song, Constants.Music.Shortest);
            }

            return new SongPoster();
        }

        private SongPoster LongestSong(IEnumerable<Song> songs)
        {
            var song = songs.Where(x => x.RunTimeTicks != null)
                              .OrderByDescending(x => x.RunTimeTicks)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (song != null)
            {
                return PosterHelper.ConvertToSongPoster(song, Constants.Movies.Longest);
            }

            return new SongPoster();
        }
        */

        private SongPoster YoungestAddedSong(IEnumerable<Song> songs)
        {
            var song = songs.Where(x => x.DateCreated != null)
                              .OrderByDescending(x => x.DateCreated)
                              .ThenBy(x => x.SortName)
                              .FirstOrDefault();

            if (song != null)
            {
                return PosterHelper.ConvertToSongPoster(song, Constants.Music.YoungestAdded);
            }

            return new SongPoster();
        }

        /*
        private TimeSpanCard TotalPlayLength(IEnumerable<Song> songs)
        {
            var playLength = new TimeSpan(songs.Sum(x => x.RunTimeTicks ?? 0));
            return new TimeSpanCard
            {
                Title = Constants.Music.TotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }
        */

        private Card<int> TotalTypeCount(IEnumerable<Song> songs, PersonType type, string title)
        {
            var value = songs.SelectMany(x => x.People)
                .DistinctBy(x => x.Id)
                .Count(x => x.Type == type);
            return new Card<int>
            {
                Value = value,
                Title = title
            };
        }

        private async Task<PersonPoster> GetMostFeaturedPersonAsync(IEnumerable<Song> songs, PersonType type, string title)
        {
            var personId = songs.SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Id, x => x.Name, (id, name) => new { Key = id, Count = name.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Key)
                .FirstOrDefault();
            if (personId != null)
            {
                var person = await _personService.GetPersonByIdAsync(personId);
                if (person != null)
                {
                    return PosterHelper.ConvertToPersonPoster(person, title);
                }
            }

            return new PersonPoster(title);

        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenreAsync(IReadOnlyCollection<Song> songs)
        {
            var list = new List<PersonPoster>();
            foreach (var genre in songs.SelectMany(x => x.Genres).Distinct().OrderBy(x => x))
            {
                var selectedMovies = songs.Where(x => x.Genres.Any(y => y == genre));
                var personId = selectedMovies
                    .SelectMany(x => x.People)
                    .Where(x => x.Type == PersonType.Actor)
                    .GroupBy(x => x.Id)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                var person = await _personService.GetPersonByIdAsync(personId);
                if (person != null)
                {
                    list.Add(PosterHelper.ConvertToPersonPoster(person, genre));
                }
            }

            return list;
        }

        private Chart CalculateGenreChart(IEnumerable<Song> songs)
        {
            var genresData = songs
                .SelectMany(x => x.Genres)
                .GroupBy(x => x)
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerGenre,
                Labels = genresData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { genresData.Select(x => x.Count) }
            };
        }
        #endregion
    }
}
