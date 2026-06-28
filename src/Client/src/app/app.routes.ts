import { Routes } from '@angular/router';
import { GamesList } from './games-list/games-list';
export const routes: Routes = [
    {path: 'games', component: GamesList},
    { path: 'games/edit', loadComponent: () => import('./games-edit/games-edit').then(m => m.GamesEdit) },
    {path: '', redirectTo: 'games', pathMatch: 'full'}
];
