using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Music
{
    public class MusicGeneral
    {
        public Card<int> SongCount { get; set; }
        public Card<int> GenreCount { get; set; }
        //public Card<int> BoxsetCount { get; set; }
        public Card<string> MostUsedContainer { get; set; }
        public SongPoster LongestSong { get; set; }
        public SongPoster ShortestSong { get; set; }
        public SongPoster OldestPremieredSong { get; set; }
        public SongPoster YoungestPremieredSong { get; set; }
        public SongPoster YoungestAddedSong { get; set; }
        public Card<string> MostFeaturedSongArtist { get; set; }
        public Card<string> MostFeaturedSongWriter { get; set; }
        public Card<string> LastPlayedSong { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
    }
}
