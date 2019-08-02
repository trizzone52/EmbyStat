import { EmbySettings } from './emby-settings';
import { TvdbSettings } from './tvdb-settings';

export class Settings {
  id: string;
  appName: string;
  wizardFinished: boolean;
  username: string;
  language: string;
  toShortMovie: number;
  toShortMovieEnabled: boolean;
  toShortSong: number;
  toShortSongEnabled: boolean;
  keepLogsCount: number;
  movieCollectionTypes: number[];
  musicCollectionTypes: number[];
  showCollectionTypes: number[];
  autoUpdate: boolean;
  updateTrain: number;
  updateInProgress: boolean;
  version: string;
  emby: EmbySettings;
  tvdb: TvdbSettings;
  enableRollbarLogging: boolean ;
  isLoaded: boolean;
}
