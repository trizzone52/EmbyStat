using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using MediaBrowser.Model.Entities;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using CollectionType = EmbyStat.Common.Models.Entities.CollectionType;
using Constants = EmbyStat.Common.Constants;

namespace Tests.Unit.Services
{
    [Collection("Services collection")]
    public class MusicServiceTests
    {
        private readonly MusicService _subject;
        private readonly List<Collection> _collections;
        private readonly Song _songOne;
        private readonly Song _songTwo;
        private readonly Song _songThree;
        private readonly Mock<ISettingsService> _settingsServiceMock;

        public MusicServiceTests()
        {
            _collections = new List<Collection>
            {
                new Collection{ Id = string.Empty, Name = "collection1", PrimaryImage = "image1", Type = CollectionType.Music},
                new Collection{ Id = string.Empty, Name = "collection2", PrimaryImage = "image2", Type = CollectionType.Music}
            };

            var actorIdOne = Guid.NewGuid();

            _songOne = new MusicBuilder(0)
                .AddPremiereDate(new DateTime(2002, 4, 2, 0, 0, 0))
                .AddName("The lord of the rings")
                .AddGenres("Action", "Drama")
                .AddRunTimeTicks(2, 10, 0)
                .Build();

            _songTwo = new MusicBuilder(1)
                .AddPremiereDate(new DateTime(2003, 4, 2, 0, 0, 0))
                .AddName("The lord of the rings, two towers")
                .AddPerson(new ExtraPerson { Type = PersonType.Director, Name = "Frodo", Id = Guid.NewGuid().ToString() })
                .AddPerson(new ExtraPerson { Type = PersonType.Actor, Name = "Frodo", Id = actorIdOne.ToString() })
                .AddGenres("Action", "Comedy")
                .AddRunTimeTicks(3, 30, 0)
                .Build();

            _songThree = new MusicBuilder(2)
                .AddPremiereDate(new DateTime(2004, 4, 2, 0, 0, 0))
                .AddName("The lord of the rings, return of the king")
                .AddGenres("Comedy")
                .AddRunTimeTicks(3, 50, 0)
                .AddPerson(new ExtraPerson{Type = PersonType.Actor,Name = "Frodo", Id = actorIdOne.ToString()})
                .AddPerson(new ExtraPerson{Type = PersonType.Director, Name = "Frodo", Id = Guid.NewGuid().ToString()})
                .AddPerson(new ExtraPerson{Type = PersonType.Writer, Name = "Frodo", Id = Guid.NewGuid().ToString()})
                .Build();

            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortSong = 10, MusicCollectionTypes = new List<CollectionType> { CollectionType.Music }, ToShortSongEnabled = true });
            _subject = CreateMusicService(_settingsServiceMock, _songOne, _songTwo, _songThree);
        }

        private MusicService CreateMusicService(Mock<ISettingsService> settingsServiceMock, params Song[] songs)
        {
            var musicRepositoryMock = new Mock<IMusicRepository>();
            musicRepositoryMock.Setup(x => x.GetAll(It.IsAny<IEnumerable<string>>())).Returns(songs);
            var collectionRepositoryMock = new Mock<ICollectionRepository>();
            collectionRepositoryMock.Setup(x => x.GetCollectionByTypes(It.IsAny<IEnumerable<CollectionType>>())).Returns(_collections);

            var personServiceMock = new Mock<IPersonService>();
            foreach (var person in songs.SelectMany(x => x.People))
            {
                personServiceMock.Setup(x => x.GetPersonByNameAsync(person.Name)).Returns(
                    Task.FromResult(new Person
                    {
                        Id = person.Id,
                        Name = person.Name,
                        BirthDate = new DateTime(2000, 1, 1),
                        Primary = "primary.jpg",
                        MovieCount = 0,
                        SongCount = 0,
                        ShowCount = 0
                    }));
            }
            
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var jobRepositoryMock = new Mock<IJobRepository>();
            return new MusicService(musicRepositoryMock.Object, collectionRepositoryMock.Object, personServiceMock.Object, settingsServiceMock.Object, statisticsRepositoryMock.Object, jobRepositoryMock.Object);
        }

        #region General

        [Fact]
        public void GetCollectionsFromDatabase()
        {
            var collections = _subject.GetMusicCollections().ToList();

            collections.Should().NotBeNull();
            collections.Count.Should().Be(2);
        }

        [Fact]
        public async void GetMusicCountStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.SongCount.Should().NotBeNull();
            stat.General.SongCount.Title.Should().Be(Constants.Music.TotalSongs);
            stat.General.SongCount.Value.Should().Be(3);
        }

        [Fact]
        public async void GetGenreCountStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.GenreCount.Should().NotBeNull();
            stat.General.GenreCount.Title.Should().Be(Constants.Music.TotalGenres);
            stat.General.GenreCount.Value.Should().Be(3);
        }

        [Fact]
        public async void GetOldestPremieredStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.OldestPremieredSong.Should().NotBeNull();
            stat.General.OldestPremieredSong.Title.Should().Be(Constants.Music.OldestPremiered);
            stat.General.OldestPremieredSong.Name.Should().Be(_songOne.Name);
            stat.General.OldestPremieredSong.DurationMinutes.Should().Be(130);
            stat.General.OldestPremieredSong.MediaId.Should().Be(_songOne.Id);
            stat.General.OldestPremieredSong.Tag.Should().Be(_songOne.Primary);
            stat.General.OldestPremieredSong.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetYoungestPremieredStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.YoungestPremieredSong.Should().NotBeNull();
            stat.General.YoungestPremieredSong.Title.Should().Be(Constants.Music.YoungestPremiered);
            stat.General.YoungestPremieredSong.Name.Should().Be(_songThree.Name);
            stat.General.YoungestPremieredSong.DurationMinutes.Should().Be(230);
            stat.General.YoungestPremieredSong.MediaId.Should().Be(_songThree.Id);
            stat.General.YoungestPremieredSong.Tag.Should().Be(_songThree.Primary);
            stat.General.YoungestPremieredSong.Year.Should().Be(_songThree.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetShortestStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.ShortestSong.Should().NotBeNull();
            stat.General.ShortestSong.Title.Should().Be(Constants.Music.Shortest);
            stat.General.ShortestSong.Name.Should().Be(_songOne.Name);
            stat.General.ShortestSong.DurationMinutes.Should().Be(130);
            stat.General.ShortestSong.MediaId.Should().Be(_songOne.Id);
            stat.General.ShortestSong.Tag.Should().Be(_songOne.Primary);
            stat.General.ShortestSong.Year.Should().Be(_songOne.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetLongestStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LongestSong.Should().NotBeNull();
            stat.General.LongestSong.Title.Should().Be(Constants.Music.Longest);
            stat.General.LongestSong.Name.Should().Be(_songThree.Name);
            stat.General.LongestSong.DurationMinutes.Should().Be(230);
            stat.General.LongestSong.MediaId.Should().Be(_songThree.Id);
            stat.General.LongestSong.Tag.Should().Be(_songThree.Primary);
            stat.General.LongestSong.Year.Should().Be(_songThree.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetYoungestAddedStat()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.YoungestAddedSong.Should().NotBeNull();
            stat.General.YoungestAddedSong.Title.Should().Be(Constants.Music.YoungestAdded);
            stat.General.YoungestAddedSong.Name.Should().Be(_songOne.Name);
            stat.General.YoungestAddedSong.DurationMinutes.Should().Be(130);
            stat.General.YoungestAddedSong.MediaId.Should().Be(_songOne.Id);
            stat.General.YoungestAddedSong.Tag.Should().Be(_songOne.Primary);
            stat.General.YoungestAddedSong.Year.Should().Be(2002);
        }

        #endregion

        #region Charts

        [Fact]
        public async void GetTotalPlayLengthStat()
        {
            var songFour = new MusicBuilder(3).AddRunTimeTicks(56, 34, 1).Build();
            var service = CreateMusicService(_settingsServiceMock, _songOne, _songTwo, _songThree, songFour);
            var stat = await service.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.TotalPlayableTime.Should().NotBeNull();
            stat.General.TotalPlayableTime.Title.Should().Be(Constants.Music.TotalPlayLength);
            stat.General.TotalPlayableTime.Days.Should().Be(2);
            stat.General.TotalPlayableTime.Hours.Should().Be(18);
            stat.General.TotalPlayableTime.Minutes.Should().Be(4);
        }

        [Fact]
        public async void CalculateGenreChart()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(2);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerGenre);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(3);
            var labels = graph.Labels.ToArray();

            labels[0].Should().Be("Action");
            labels[1].Should().Be("Comedy");
            labels[2].Should().Be("Drama");

            graph.DataSets.Count.Should().Be(1);

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(3);
            dataset[0].Should().Be(2);
            dataset[1].Should().Be(2);
            dataset[2].Should().Be(1);
        }

        [Fact]
        public async void CalculatePremiereYearChart()
        {
            var musicFour = new MusicBuilder(3).AddPremiereDate(new DateTime(1991, 3, 12)).Build();
            var musicFive = new MusicBuilder(4).AddPremiereDate(new DateTime(1992, 3, 12)).Build();
            var musicSix = new MusicBuilder(5).AddPremiereDate(new DateTime(1989, 3, 12)).Build();
            var service = CreateMusicService(_settingsServiceMock,_songOne, _songTwo, _songThree, musicFour, musicFive, musicSix);

            var stat = await service.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());
            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(2);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(4);
            graph.Labels.ToArray()[0].Should().Be("1985 - 1989");
            graph.Labels.ToArray()[1].Should().Be("1990 - 1994");
            graph.Labels.ToArray()[2].Should().Be("1995 - 1999");
            graph.Labels.ToArray()[3].Should().Be("2000 - 2004");

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(4);
            dataset[0].Should().Be(1);
            dataset[1].Should().Be(2);
            dataset[2].Should().Be(0);
            dataset[3].Should().Be(3);
        }

        [Fact]
        public async void CalculatePremiereYearChartWithoutMusic()
        {
            var service = CreateMusicService(_settingsServiceMock);

            var stat = await service.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());
            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(2);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(0);

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(0);
        }

        #endregion

        #region People

        [Fact]
        public async void TotalTypeCountForActors()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.TotalActorCount.Should().NotBeNull();
            stat.People.TotalActorCount.Value.Should().Be(4);
            stat.People.TotalActorCount.Title.Should().Be(Constants.Common.TotalActors);
        }

        [Fact]
        public async void TotalTypeCountForDirectors()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.TotalDirectorCount.Should().NotBeNull();
            stat.People.TotalDirectorCount.Value.Should().Be(2);
            stat.People.TotalDirectorCount.Title.Should().Be(Constants.Common.TotalDirectors);
        }

        [Fact]
        public async void TotalTypeCountForWriters()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.TotalWriterCount.Should().NotBeNull();
            stat.People.TotalWriterCount.Value.Should().Be(1);
            stat.People.TotalWriterCount.Title.Should().Be(Constants.Common.TotalWriters);
        }

        [Fact]
        public async void MostFeaturedActorsPerGenre()
        {
            var stat = await _subject.GetMusicStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenre.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenre.Count.Should().Be(3);
            stat.People.MostFeaturedActorsPerGenre[0].Title.Should().Be("Action");
            stat.People.MostFeaturedActorsPerGenre[0].Name.Should().Be("Gimli");
            stat.People.MostFeaturedActorsPerGenre[1].Title.Should().Be("Comedy");
            stat.People.MostFeaturedActorsPerGenre[1].Name.Should().Be("Gimli");
            stat.People.MostFeaturedActorsPerGenre[2].Title.Should().Be("Drama");
            stat.People.MostFeaturedActorsPerGenre[2].Name.Should().Be("Gimli");
        }

        #endregion
    }
}
