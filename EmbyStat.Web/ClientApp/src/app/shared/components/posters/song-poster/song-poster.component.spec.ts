import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SongPosterComponent } from './song-poster.component';

describe('SongPosterComponent', () => {
  let component: SongPosterComponent;
  let fixture: ComponentFixture<SongPosterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SongPosterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SongPosterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
