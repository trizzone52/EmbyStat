using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Music
{
    public class MusicGeneralViewModel
    {
        public CardViewModel<int> MovieCount { get; set; }
        public CardViewModel<int> GenreCount { get; set; }
        public CardViewModel<int> BoxsetCount { get; set; }
        public CardViewModel<string> MostUsedContainer { get; set; }
        //public SongPosterViewModel HighestRatedMovie { get; set; }
        //public SongPosterViewModel LowestRatedMovie { get; set; }
        //public SongPosterViewModel LongestMovie { get; set; }
        //public SongPosterViewModel ShortestMovie { get; set; }
        public SongPosterViewModel OldestPremieredMovie { get; set; }
        public SongPosterViewModel YoungestPremieredMovie { get; set; }
        public SongPosterViewModel YoungestAddedMovie { get; set; }
        public CardViewModel<string> MostFeaturedMovieActor { get; set; }
        public CardViewModel<string> MostFeaturedMovieDirector { get; set; }
        public CardViewModel<string> LastPlayedMovie { get; set; }
        public TimeSpanCardViewModel TotalPlayableTime { get; set; }

    }
}
