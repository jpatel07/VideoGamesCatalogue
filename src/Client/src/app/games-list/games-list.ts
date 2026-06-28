import { Component, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { VideoGame, PageResult } from '../models/video-game';
import { httpResource } from '@angular/common/http';

@Component({
  selector: 'app-games-list',
  imports: [NgbPaginationModule, DatePipe],
  templateUrl: './games-list.html',
  styleUrl: './games-list.css',
})
export class GamesList {
  result = signal<PageResult<VideoGame> | null>(null);
  pageNumber = signal(1);
  pageSize = signal(25);

 

 gamesResource = httpResource<PageResult<VideoGame>>(() => ({
    url: 'http://localhost:5098/api/VideoGames',
    params: {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    }
  }));

}
