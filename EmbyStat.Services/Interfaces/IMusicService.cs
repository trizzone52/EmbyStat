using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Music;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IMusicService
    {
        IEnumerable<Collection> GetMusicCollections();
        Task<MusicStatistics> GetMusicStatisticsAsync(List<string> collectionIds);
        Task<MusicStatistics> CalculateMusicStatistics(List<string> collectionIds);
        bool TypeIsPresent();
    }
}
