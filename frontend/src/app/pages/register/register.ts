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
import { MatFormFieldModule } from '@angular/material/form-field';
import { LoginResponse } from '../../models/user.model';

@Component({
  selector: 'app-register',
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
  templateUrl: './register.html',
  styleUrl: './register.css',
})

export class RegisterComponent{

  fullName: string = '';
  email: string = '';
  password: string = '';

  errorMessage: string = '';
  successMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onRegister(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isLoading = true;

    this.authService.register(this.fullName, this.email, this.password).subscribe({
      // Si el registro funcionó, mostramos un mensaje y mandamos al usuario
      // a la pantalla de login para que entre con su cuenta nueva
      next: () => {
        this.successMessage = 'Cuenta creada. Ahora inicia sesión.';
        this.isLoading = false;
        // Esperamos un poco para que el usuario lea el mensaje antes de redirigir
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      // Si algo salió mal (email duplicado, datos inválidos, etc.)
      error: (err) => {
        // err.error.message viene del backend — ahí mandamos el mensaje
        // específico, como "Ya existe un usuario con ese email"
        this.errorMessage = err.error?.message || 'Error al registrar. Intenta de nuevo.';
        this.isLoading = false;
      },
    });
  }
}

