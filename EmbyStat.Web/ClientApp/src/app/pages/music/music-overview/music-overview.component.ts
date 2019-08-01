import { NgScrollbar } from 'ngx-scrollbar';
import { Observable, Subscription } from 'rxjs';
import { OptionsService } from 'src/app/shared/components/charts/options/options';

import {
  Component, HostListener, OnDestroy, OnInit, ViewChild
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';

import { Options } from '../../../shared/components/charts/options/options';
import { NoTypeFoundDialog } from '../../../shared/dialogs/no-type-found/no-type-found.component';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { Collection } from '../../../shared/models/collection';
import { MusicStatistics } from '../../../shared/models/music/music-statistics';
import { Settings } from '../../../shared/models/settings/settings';
import { MusicService } from '../service/music.service';

@Component({
  selector: 'app-music-overview',
  templateUrl: './music-overview.component.html',
  styleUrls: ['./music-overview.component.scss']
})
export class MusicOverviewComponent implements OnInit, OnDestroy {
  statistics$: Observable<MusicStatistics>;

  @ViewChild(NgScrollbar)
  textAreaScrollbar: NgScrollbar;

  selectedCollectionSub: Subscription;
  dropdownBlurredSub: Subscription;
  settingsSub: Subscription;
  isMusicTypePresentSub: Subscription;
  collections$: Observable<Collection[]>;
  collectionsFormControl = new FormControl('', { updateOn: 'blur' });
  typeIsPresent: boolean;

  settings: Settings;

  /*noPrimaryDisplayedColumns = ['number', 'title', 'link'];*/

  barOptions: Options;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly musicService: MusicService,
    public dialog: MatDialog,
    private readonly optionsService: OptionsService) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
    });

    this.isMusicTypePresentSub = this.musicService.isTypePresent().subscribe((typePresent: boolean) => {
      this.typeIsPresent = typePresent;
      if (!typePresent) {
        this.dialog.open(NoTypeFoundDialog,
          {
            width: '550px',
            data: 'MUSIC'
          });
      }
    });

    this.collections$ = this.musicService.getCollections();
    this.barOptions = this.optionsService.getBarOptions();

    this.statistics$ = this.musicService.getStatistics([]);

    this.collectionsFormControl.valueChanges.subscribe((collectionList: string[]) => {
      this.statistics$ = this.musicService.getStatistics(collectionList);
    });
  }

  ngOnInit() {
  }


  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.statistics$.subscribe(() => {
      this.textAreaScrollbar.update();
    });
  }

  openSong(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/item/item.html?id=${id}`, '_blank');
  }

  ngOnDestroy() {
    if (this.selectedCollectionSub !== undefined) {
      this.selectedCollectionSub.unsubscribe();
    }

    if (this.dropdownBlurredSub !== undefined) {
      this.dropdownBlurredSub.unsubscribe();
    }

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }

    if (this.isMusicTypePresentSub !== undefined) {
      this.isMusicTypePresentSub.unsubscribe();
    }
  }
}
