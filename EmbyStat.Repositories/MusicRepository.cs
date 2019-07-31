using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Extensions;
using NLog;
using Logger = NLog.Logger;

namespace EmbyStat.Repositories
{
    public class MusicRepository : IMusicRepository
    {
        private readonly LiteCollection<Song> _musicCollection;

        public MusicRepository(IDbContext context)
        {
            _musicCollection = context.GetContext().GetCollection<Song>();
        }

        public void UpsertRange(IEnumerable<Song> songs)
        {
            _musicCollection.Upsert(songs);
        }

        public IEnumerable<Song> GetAll(IEnumerable<string> collectionIds)
        {
            var bArray = new BsonArray();
            foreach (var collectionId in collectionIds)
            {
                bArray.Add(collectionId);
            }
            return collectionIds.Any() ?
                _musicCollection.Find(Query.In("CollectionId", bArray)) :
                _musicCollection.FindAll();
        }


        public bool Any()
        {
            return _musicCollection.Exists(Query.All());
        }

        public int GetSongCountForPerson(string personId)
        {
            return _musicCollection.Count(Query.EQ("People[*]._id", personId));
        }

        public Song GetSongById(string id)
        {
            return _musicCollection.FindById(id);
        }

        public void RemoveSongs()
        {
            _musicCollection.Delete(Query.All());
        }
    }
}
