import { Component, inject, linkedSignal, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { httpResource } from '@angular/common/http';
import { PageResult, VideoGame } from '../models/video-game';
import { VideoGameServices } from '../services/video-game';

type RowStatus = {
  saving: boolean;
  saved: boolean;
  error: string | null;
};

@Component({
  selector: 'app-games-edit',
  imports: [FormsModule, NgbPaginationModule],
  templateUrl: './games-edit.html',
  styleUrl: './games-edit.css',
})
export class GamesEdit {
  private readonly gameService = inject(VideoGameServices);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(25);

  readonly gamesResource = httpResource<PageResult<VideoGame>>(() => ({
    url: 'http://localhost:5098/api/VideoGames',
    params: {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    },
  }));

  // Working copy of the current page. Resets whenever a new page loads
  // so user edits never mutate the resource's response directly.
  readonly rows = linkedSignal<VideoGame[]>(() => {
    const page = this.gamesResource.value();
    return page ? page.items.map(g => ({ ...g })) : [];
  });

  // Per-row UI state keyed by game id.
  private readonly rowStatus = signal<Record<number, RowStatus>>({});

  statusFor(id: number): RowStatus {
    return this.rowStatus()[id] ?? { saving: false, saved: false, error: null };
  }

  save(row: VideoGame) {
    this.setStatus(row.id, { saving: true, saved: false, error: null });

    const { id, ...request } = row;
    this.gameService.update(id, request).subscribe({
      next: () => this.refreshRow(row.id, true),
      error: () => this.setStatus(row.id, { saving: false, saved: false, error: 'Failed to save.' }),
    });
  }

  reset(row: VideoGame) {
    this.setStatus(row.id, { saving: true, saved: false, error: null });
    this.refreshRow(row.id, false);
  }

  private refreshRow(id: number, savedFlag: boolean) {
    this.gameService.getById(id).subscribe({
      next: (fresh) => {
        this.rows.update(items => items.map(g => g.id === fresh.id ? { ...fresh } : g));
        this.setStatus(id, { saving: false, saved: savedFlag, error: null });
      },
      error: () => this.setStatus(id, {
        saving: false,
        saved: savedFlag,
        error: savedFlag ? 'Saved, but failed to refresh row.' : 'Failed to reload row.',
      }),
    });
  }

  private setStatus(id: number, status: RowStatus) {
    this.rowStatus.update(s => ({ ...s, [id]: status }));
  }
}
