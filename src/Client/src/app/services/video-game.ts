import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PageResult, UpdateVideoGameRequest, VideoGame } from '../models/video-game';
@Injectable({ providedIn: 'root' })
export class VideoGameServices {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5098/api/VideoGames';

  getGames(pageNumber: number, pageSize: number): Observable<PageResult<VideoGame>> {
    const params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);

    return this.http.get<PageResult<VideoGame>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<VideoGame> {
    return this.http.get<VideoGame>(`${this.baseUrl}/${id}`);
  }

  update(id: number, request: UpdateVideoGameRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }
}
