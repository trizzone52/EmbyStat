import { Card } from '../common/card';
import { TimespanCard } from '../common/timespan-card';
import { SongPoster } from './song-poster';

export class GeneralMusicStatistics {
  songCount: Card<number>;
  genreCount: Card<number>;
  boxsetCount: Card<number>;
  mostUsedContainer: Card<string>;
  highestRatedSong: SongPoster;
  lowestRatedSong: Card<string>;
  longestSong: Card<string>;
  shortestSong: Card<string>;
  oldestPremieredSong: Card<string>;
  youngestPremieredSong: Card<string>;
  youngestAddedSong: Card<string>;
  mostFeaturedSongArtist: Card<string>;
  mostFeaturedSongWriter: Card<string>;
  lastPlayedSong: Card<string>;
  totalPlayableTime: TimespanCard;
}
