import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { DenunciaService } from '../../services/denuncia.service';
import { Denuncia } from '../../models/denuncia.model';
import { MediatorService } from '../../services/mediator.service';
import { MatSelectModule } from '@angular/material/select';
import { MediatorResponse } from '../../models/mediator.model';


@Component({
  selector: 'app-administrardenuncias',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    RouterLink,
    MatToolbarModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './administrardenuncias.html',
  styleUrl: './administrardenuncias.css',
})
export class AdministrardenunciasComponent {

  mostrarDenuncias = false;
  mostrarAsignacion = false;

  denuncias: any[] = [];

  isLoading = false;
  errorMessage = '';
  mediadores: MediatorResponse[] = [];
  mediadorSeleccionado: MediatorResponse | null = null;
  denunciaSeleccionada: Denuncia | null = null;


  constructor(
    private denunciaService: DenunciaService,
    private mediatorService: MediatorService
  ) {}

  
abrirDenuncias() {
  this.cargarDenuncias();
  this.mostrarDenuncias = true;
}

  cargarDenuncias(): void {

  console.log('Cargando denuncias...');

  this.denunciaService.getAll().subscribe({
    next: (data) => {
      console.log('Respuesta backend:', data);

      this.denuncias = data;
    },
    error: (err) => {
      console.error('Error:', err);
    }
  });
}

asignarMediador(): void {

  if (!this.denunciaSeleccionada) {
    return;
  }

  if (!this.mediadorSeleccionado) {

    this.errorMessage =
      'Seleccione un mediador';

    return;
  }

this.denunciaService.assignMediator(
  this.denunciaSeleccionada.id!,
  this.mediadorSeleccionado.id,
  this.mediadorSeleccionado.fullName

  ).subscribe({

    next: () => {

      alert('Mediador asignado correctamente');

      this.cerrarAsignacion();

      this.cargarDenuncias();
    },

    error: (err) => {

      console.error(err);

      this.errorMessage =
        err.error?.message ||
        'Error al asignar mediador';
    }

  });
}

abrirAsignacion(denuncia: Denuncia): void {
  
  this.cargarMediadores();
  this.denunciaSeleccionada = denuncia;
  this.mostrarAsignacion = true;

  
}

cargarMediadores(): void {

  this.mediatorService.getAll().subscribe({

    next: (data) => {

      this.mediadores = data;

      console.log('Mediadores:', data);
    },

    error: (err) => {

      console.error(err);

    }

  });
}

cerrarDenuncias(): void {
this.mostrarDenuncias = false;
}

cerrarAsignacion(): void {
this.mostrarAsignacion = false;
}
}