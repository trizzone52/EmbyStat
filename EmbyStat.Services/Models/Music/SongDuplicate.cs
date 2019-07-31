using System;

namespace EmbyStat.Services.Models.Music
{
    public class SongDuplicate
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public SongDuplicateItem ItemOne { get; set; }
        public SongDuplicateItem ItemTwo { get; set; }
    }

    public class SongDuplicateItem
    {
        public DateTimeOffset? DateCreated { get; set; }
        public int Id { get; set; }
        public string Quality { get; set; }
    }
}
