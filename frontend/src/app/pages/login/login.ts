// Un componente => a una pantalla
// 3 partes, ts => logica, html => estructura visual y css => a los estilos

import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { AuthService } from '../../services/auth.service';
import { LoginResponse } from '../../models/user.model';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, // *ngIf y *ngFor
    FormsModule, // [(ngModel)]
    MatInputModule, //Campos de texto de Angular Material
    MatButtonModule, //Botones de Angular Material
    MatCardModule,
    RouterLink,
    //Tarjeta que contiene componentes de Angular Material
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  errorMessage: string = '';

  isLoading: boolean = false;

  // AuthService para intentar hacer el login y Router para redirigir a la siguiente pantalla
  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onLogin(): void {
    // Limpiamos las variables antes de usar
    this.errorMessage = '';
    this.isLoading = true;

    this.authService.login(this.email, this.password).subscribe({
      // Si el login tiene exito entonces redirigimos al experimento
      next: (response) => {
        this.authService.saveToken(response.token);
        this.router.navigate(['/denuncias']);
      },

      // Algo fallo, credenciales, servidor, etc

      error: (err) => {
        ((this.errorMessage = 'Credenciales invalidas. Intenta de nuevo: '), err);
        this.isLoading = false;
      },
    });
  }
}
