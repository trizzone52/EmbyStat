using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Music
{
    public class MusicGeneralViewModel
    {
        public CardViewModel<int> SongCount { get; set; }
        public CardViewModel<int> GenreCount { get; set; }
        public CardViewModel<int> BoxsetCount { get; set; }
        public CardViewModel<string> MostUsedContainer { get; set; }
        public SongPosterViewModel HighestRatedSong { get; set; }
        public SongPosterViewModel LowestRatedSong { get; set; }
        public SongPosterViewModel LongestSong { get; set; }
        public SongPosterViewModel ShortestSong { get; set; }
        public SongPosterViewModel OldestPremieredSong { get; set; }
        public SongPosterViewModel YoungestPremieredSong { get; set; }
        public SongPosterViewModel YoungestAddedSong { get; set; }
        public CardViewModel<string> MostFeaturedSongArtist { get; set; }
        public CardViewModel<string> MostFeaturedSongWriter { get; set; }
        public CardViewModel<string> LastPlayedSong { get; set; }
        public TimeSpanCardViewModel TotalPlayableTime { get; set; }

    }
}
