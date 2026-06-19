import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MediatorService } from '../../services/mediator.service';
import { CreateMediatorRequest } from '../../models/createmediator.model';

@Component({
  selector: 'app-registermediator',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    RouterLink,
    MatToolbarModule,
    MatIconModule
  ],
  templateUrl: './registermediator.html',
  styleUrl: './registermediator.css',
})
export class RegisterMediatorComponent {

  fullName = '';
  email = '';
  zone = '';
  specialty = '';
  availability = '';

  isLoading = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private mediatorService: MediatorService
  ) {}

  registerMediator(): void {

    if (this.isLoading) {
      return;
    }

    if (
      !this.fullName.trim() ||
      !this.email.trim() ||
      !this.zone.trim() ||
      !this.specialty.trim() ||
      !this.availability.trim()
    ) {
      this.errorMessage = 'Todos los campos son obligatorios';
      return;
    }

    const mediator: CreateMediatorRequest = {
      fullName: this.fullName,
      email: this.email,
      zone: this.zone,
      specialty: this.specialty,
      availability: this.availability
    };

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.mediatorService.create(mediator).subscribe({
      next: (response) => {

        this.isLoading = false;

        this.successMessage =
          `Mediador ${response.fullName} registrado correctamente`;

        this.fullName = '';
        this.email = '';
        this.zone = '';
        this.specialty = '';
        this.availability = '';
      },

      error: (err) => {

        this.isLoading = false;

        this.errorMessage =
          err.error?.message ||
          'Error al registrar mediador';

        console.error(err);
      }
    });
  }
}