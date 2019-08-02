import { Duplicate } from './duplicate';
import { ShortSong } from './short-song';
import { SuspiciousSong } from './suspicious-song';

export class songSuspiciousContainer {
  duplicates: Duplicate[];
  shorts: ShortSong[];
  noImdb: SuspiciousSong[];
  noPrimary: SuspiciousSong[];
}
