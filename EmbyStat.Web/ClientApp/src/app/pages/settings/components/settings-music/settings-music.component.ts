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
  toShortMusicControl = new FormControl('', [Validators.required]);
  toShortMusicEnabledControl = new FormControl('', [Validators.required]);

  newCollectionList: number[];
  isSaving = false;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toastService: ToastService) {
    this.formToShort = new FormGroup({
      toShortMusic: this.toShortMusicControl,
      toShortMusicEnabled: this.toShortMusicEnabledControl
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.toShortMusicControl.setValue(this.settings.toShortMusic);
      this.toShortMusicEnabledControl.setValue(this.settings.toShortMusicEnabled);
    }
  }

  saveToShortForm() {
    if (this.checkForm(this.formToShort)) {
      this.isSaving = true;

      const settings = { ...this.settings };
      settings.toShortMovie = this.toShortMusicControl.value;
      settings.toShortMovieEnabled = this.toShortMusicEnabledControl.value;
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
