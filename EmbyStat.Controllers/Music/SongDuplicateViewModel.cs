using System;

namespace EmbyStat.Controllers.Music
{
    public class SongDuplicateViewModel
    {
        public int Number { get; set; }
        public SongDuplicateItemViewModel ItemOne { get; set; }
        public SongDuplicateItemViewModel ItemTwo { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
    }

    public class SongDuplicateItemViewModel
    {
        public DateTimeOffset? DateCreated { get; set; }
        public int Id { get; set; }
        public string Quality { get; set; }
    }
}
