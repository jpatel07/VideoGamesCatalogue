import { Routes } from '@angular/router';
import { GamesList } from './games-list/games-list';

export const routes: Routes = [
    {path: 'games', component: GamesList},
    {path: '', redirectTo: 'games', pathMatch: 'full'}
];
