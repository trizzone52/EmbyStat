using System.Collections.Generic;

namespace EmbyStat.Controllers.Music
{
    public class SuspiciousTablesViewModel
    {
        public List<MusicDuplicateViewModel> Duplicates { get; set; }
        public List<ShortMusicViewModel> Shorts { get; set; }
        public List<SuspiciousMusicViewModel> NoImdb { get; set; }
        public List<SuspiciousMusicViewModel> NoPrimary { get; set; }
    }
}
