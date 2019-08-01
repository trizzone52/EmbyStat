import { Subscription } from 'rxjs';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';

import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { ConfigHelper } from '../../../helpers/config-helper';
import { SongPoster } from '../../../models/music/song-poster';
import { Settings } from '../../../models/settings/settings';

@Component({
  selector: 'app-song-poster',
  templateUrl: './song-poster.component.html',
  styleUrls: ['./song-poster.component.scss']
})
export class SongPosterComponent implements OnInit {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: SongPoster;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly sanitizer: DomSanitizer) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe(data => this.settings = data);
  }

  ngOnInit() {
  }

  getPoster() {
    if (this.settings === undefined) {
      return '';
    }

    if (this.poster.mediaId !== '0') {
      const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
      const url =
        `url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag
        }&quality=90&enableimageenhancers=false)`;
      return this.sanitizer.bypassSecurityTrustStyle(url);
    }
  }

  openSong() {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/item/item.html?id=${this.poster.mediaId}`, '_blank');
  }
}
