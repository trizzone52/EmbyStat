using LiteDB;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class Audio : Media
    {
        [BsonId]
        public string MediaType { get; set; }
        public long? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public long? RunTimeTicks { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
    }
}
