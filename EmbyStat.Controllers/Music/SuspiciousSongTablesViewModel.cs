using System.Collections.Generic;

namespace EmbyStat.Controllers.Music
{
    public class SuspiciousSongTablesViewModel
    {
        public List<SongDuplicateViewModel> Duplicates { get; set; }
        public List<ShortSongViewModel> Shorts { get; set; }
        public List<SuspiciousSongViewModel> NoImdb { get; set; }
        public List<SuspiciousSongViewModel> NoPrimary { get; set; }
    }
}
