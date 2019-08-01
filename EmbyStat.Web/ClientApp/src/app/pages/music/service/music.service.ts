import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Collection } from '../../../shared/models/collection';
import { MusicStatistics } from '../../../shared/models/music/music-statistics';

@Injectable()
export class MusicService {
  private readonly baseUrl = '/api/music/';
  private getCollectionsUrl = this.baseUrl + 'collections';
  private getStatisticsUrl = this.baseUrl + 'statistics';
  private isTypePresentUrl = this.baseUrl + 'typepresent';

  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getStatistics(list: string[]): Observable<MusicStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MusicStatistics>(this.getStatisticsUrl + params);
  }

  isTypePresent(): Observable<boolean> {
    return this.http.get<boolean>(this.isTypePresentUrl);
  }
}
