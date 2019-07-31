using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Music
{
    public class MusicGeneral
    {
        public Card<int> SongCount { get; set; }
        public Card<int> GenreCount { get; set; }
        public Card<int> BoxsetCount { get; set; }
        public Card<string> MostUsedContainer { get; set; }
        public SongPoster HighestRatedSong { get; set; }
        public SongPoster LowestRatedSong { get; set; }
        public SongPoster LongestSong { get; set; }
        public SongPoster ShortestSong { get; set; }
        public SongPoster OldestReleasedSong { get; set; }
        public SongPoster YoungestReleasedSong { get; set; }
        public SongPoster YoungestAddedSong { get; set; }
        public Card<string> MostFeaturedArtist { get; set; }
        public Card<string> MostFeaturedWriter { get; set; }
        public Card<string> LastPlayedSong { get; set; }
        public TimeSpanCard TotalPlayableTime { get; set; }
    }
}
