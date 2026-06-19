import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { HomeComponent } from './pages/home/home';
import { RegisterComponent } from './pages/register/register';
import { DenunciasComponent } from './pages/denuncias/denuncias';
import { authGuard } from './services/auth.guard';
import { AdminPanelComponent } from './pages/adminpanel/adminpanel';
import { MediatorHomeComponent } from './pages/mediatorhome/mediatorhome';
import { RegisterMediatorComponent } from './pages/registermediator/registermediator';
import { AdministrardenunciasComponent } from './pages/administrardenuncias/administrardenuncias';
import { MiscasosComponent } from './pages/miscasos/miscasos';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
  { path: 'denuncias', component: DenunciasComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'adminpanel', component: AdminPanelComponent},
  { path: 'mediatorhome', component: MediatorHomeComponent},
  { path: 'registermediator', component: RegisterMediatorComponent},
  { path: 'administrardenuncias', component: AdministrardenunciasComponent},
  {path: 'miscasos', component: MiscasosComponent},
  
  //Cualquier URL que no coincida con las anteriores entonces, va para el login
  { path: '**', redirectTo: '/login' },
];
