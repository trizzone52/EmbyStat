using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMusicRepository
    {
        void RemoveSongs();
        void UpsertRange(IEnumerable<Song> songs);
        IEnumerable<Song> GetAll(IEnumerable<string> collections);
        bool Any();
        int GetSongCountForPerson(string personId);
        Song GetSongById(string id);
    }
}
