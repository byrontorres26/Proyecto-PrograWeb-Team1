import { Routes } from '@angular/router';

export const routes: Routes = [];
import { LoginComponent } from './pages/login/login';
import { authGuard } from './services/auth.guard';


export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },


  //canActivate
  //{ path: 'experiments', component: ExperimentsComponent, canActivate: [authGuard] },

  //Cualquier URL que no coincida con las anteriores entonces, va para el login
  { path: '**', redirectTo: '/login' },
];
