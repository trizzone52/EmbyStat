using System.Collections.Generic;

namespace EmbyStat.Services.Models.Music
{
    public class SuspiciousSongTables
    {
        //public IEnumerable<SongDuplicate> Duplicates { get; set; }
        public IEnumerable<ShortSong> Shorts { get; set; }
        public IEnumerable<SuspiciousSong> NoPrimary { get; set; }
    }
}
