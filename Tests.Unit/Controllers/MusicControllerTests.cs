using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Music;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Music;
using EmbyStat.Services.Models.Stat;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    [Collection("Mapper collection")]
    public class MusicControllerTests
    {
        private readonly MusicController _subject;
        private readonly Mock<IMusicService> _musicServiceMock;
        private readonly List<Collection> _collections;
        private readonly MusicGeneral _musicGeneral;

        public MusicControllerTests()
        {
            _collections = new List<Collection>
            {
                new Collection{ Id = "id1", Name = "collection1", PrimaryImage = "image1", Type = CollectionType.Music},
                new Collection{ Id = "id2", Name = "collection2", PrimaryImage = "image2", Type = CollectionType.Music}
            };

            _musicGeneral = new MusicGeneral
            {
                LongestSong = new SongPoster { Name = "The lord of the rings" }
            };

            var musicstatistics = new MusicStatistics
            {
                General = _musicGeneral
            };

            _musicServiceMock = new Mock<IMusicService>();
            _musicServiceMock.Setup(x => x.GetMusicCollections()).Returns(_collections);
            _musicServiceMock.Setup(x => x.GetMusicStatisticsAsync(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(musicstatistics));

            var _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(x => x.Map<MusicStatisticsViewModel>(It.IsAny<MusicStatistics>()))
                .Returns(new MusicStatisticsViewModel { General = new MusicGeneralViewModel { LongestSong = new SongPosterViewModel { Name = "The lord of the rings" } } });
            _mapperMock.Setup(x => x.Map<IList<CollectionViewModel>>(It.IsAny<List<Collection>>())).Returns(
                new List<CollectionViewModel>
                {
                    new CollectionViewModel
                    {
                        Name = "collection1",
                        PrimaryImage = "image1",
                        Type = (int) CollectionType.Music
                    },
                    new CollectionViewModel
                    {
                        Name = "collection2",
                        PrimaryImage = "image2",
                        Type = (int) CollectionType.Music
                    }
                });
            _subject = new MusicController(_musicServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void AreMusicCollectionsReturned()
        {
            var result = _subject.GetCollections();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var list = resultObject.Should().BeOfType<List<CollectionViewModel>>().Subject;

            list.Count.Should().Be(2);
            list[0].Name.Should().Be(_collections[0].Name);
            list[1].Name.Should().Be(_collections[1].Name);
            _musicServiceMock.Verify(x => x.GetMusicCollections(), Times.Once);
        }

        [Fact]
        public async void AreMusicStatsReturned()
        {
            var result = await _subject.GetGeneralStats(_collections.Select(x => x.Id).ToList());
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var stat = resultObject.Should().BeOfType<MusicStatisticsViewModel>().Subject;

            stat.Should().NotBeNull();
            stat.General.LongestSong.Name.Should().Be(_musicGeneral.LongestSong.Name);
            _musicServiceMock.Verify(x => x.GetMusicStatisticsAsync(It.Is<List<string>>(
                y => y[0] == _collections[0].Id &&
                     y[1] == _collections[1].Id)), Times.Once);
        }
    }
}
