import { Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { Settings } from '../../../../shared/models/settings/settings';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-music',
  templateUrl: './settings-music.component.html',
  styleUrls: ['./settings-music.component.scss']
})
export class SettingsMusicComponent implements OnInit, OnDestroy, OnChanges {
  @Input() settings: Settings;

  formToShort: FormGroup;
  toShortSongControl = new FormControl('', [Validators.required]);
  toShortSongEnabledControl = new FormControl('', [Validators.required]);

  newCollectionList: number[];
  isSaving = false;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toastService: ToastService) {
    this.formToShort = new FormGroup({
      toShortSong: this.toShortSongControl,
      toShortSongEnabled: this.toShortSongEnabledControl
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.toShortSongControl.setValue(this.settings.toShortSong);
      this.toShortSongEnabledControl.setValue(this.settings.toShortSongEnabled);
    }
  }

  saveToShortForm() {
    if (this.checkForm(this.formToShort)) {
      this.isSaving = true;

      const settings = { ...this.settings };
      settings.toShortSong = this.toShortSongControl.value;
      settings.toShortSongEnabled = this.toShortSongEnabledControl.value;
      this.settingsFacade.updateSettings(settings);
      this.toastService.showSuccess('SETTINGS.SAVED.MUSIC');
      this.isSaving = false;
    }
  }

  saveCollectionTypesForm() {
    this.isSaving = true;

    const settings = { ...this.settings };
    settings.musicCollectionTypes = this.newCollectionList;
    this.settingsFacade.updateSettings(settings);
    this.toastService.showSuccess('SETTINGS.SAVED.MUSIC');
    this.isSaving = false;
  }

  onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  private checkForm(form: FormGroup): boolean {
    for (const i of Object.keys(form.controls)) {
      form.controls[i].markAsTouched();
      form.controls[i].updateValueAndValidity();
    }

    return form.valid;
  }

  ngOnDestroy() {

  }
}
