using System;

namespace EmbyStat.Controllers.Music
{
    public class MusicDuplicateViewModel
    {
        public int Number { get; set; }
        public MusicDuplicateItemViewModel ItemOne { get; set; }
        public MusicDuplicateItemViewModel ItemTwo { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
    }

    public class MusicDuplicateItemViewModel
    {
        public DateTimeOffset? DateCreated { get; set; }
        public int Id { get; set; }
        public string Quality { get; set; }
    }
}
