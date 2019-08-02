import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsMusicComponent } from './settings-music.component';

describe('SettingsMusicComponent', () => {
  let component: SettingsMusicComponent;
  let fixture: ComponentFixture<SettingsMusicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SettingsMusicComponent]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsMusicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
