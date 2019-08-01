import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MusicOverviewComponent } from './music-overview.component';

describe('MusicOverviewComponent', () => {
  let component: MusicOverviewComponent;
  let fixture: ComponentFixture<MusicOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MusicOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MusicOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
