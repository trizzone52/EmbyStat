using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities
{
    public class Song : Audio
    {
        public string OriginalTitle { get; set; }
        public string[] Genres { get; set; }
        public ExtraPerson[] People { get; set; }
    }
}
