import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { MusicOverviewComponent } from './music-overview/music-overview.component';
import { MusicService } from './service/music.service';


@NgModule({
  declarations: [
    MusicOverviewComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    MusicService
  ]
})
export class MusicModule { }
