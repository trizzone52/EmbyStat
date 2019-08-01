import { PersonStatistics } from '../common/person-statistics';
import { GeneralMusicStatistics } from './general-music-statistics';
import { MusicCharts } from './music-charts';

export class MusicStatistics {
    general: GeneralMusicStatistics;
    charts: MusicCharts;
    people: PersonStatistics;
}
