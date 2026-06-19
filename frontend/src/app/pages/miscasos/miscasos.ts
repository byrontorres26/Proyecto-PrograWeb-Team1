import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { DenunciaService } from '../../services/denuncia.service';
import { Denuncia } from '../../models/denuncia.model';
import { RouterLink } from '@angular/router';
import { FormField } from '@angular/forms/signals';
import { MatFormField } from '@angular/material/input';
import { MatInputModule } from '@angular/material/input';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { SessionService } from '../../services/sesion.service';
import { AgreementService } from '../../services/agreement.service';


@Component({
  selector: 'app-miscasos',
  standalone: true,
  imports: [
    CommonModule, // *ngIf y *ngFor
    FormsModule, // [(ngModel)]
    MatInputModule, //Campos de texto de Angular Material
    MatButtonModule, //Botones de Angular Material
    MatCardModule,
    RouterLink,
    MatToolbarModule,
    MatIconModule,
    MatSelectModule,
  ],
  templateUrl: './miscasos.html',
  styleUrl: './miscasos.css'
})
export class MiscasosComponent implements OnInit {

  denuncias: Denuncia[] = [];

  isLoading = false;
  errorMessage = '';
  mostrarSesion = false;
  casoSeleccionado: Denuncia | null = null;
  scheduledDate = '';
  modality = '';
  meetingLink = '';
  agreementText = '';
  pointDescription = '';
  pointDeadline = '';
  mostrarAcuerdo = false;

  constructor(
    private denunciaService: DenunciaService,
    private sessionService: SessionService,
    private agreementService: AgreementService
    
  ) {}

  ngOnInit(): void {
    this.cargarCasos();
  }

  cargarCasos(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.denunciaService.getMyCases().subscribe({

      next: (data: Denuncia[]) => {

        this.denuncias = data;

        this.isLoading = false;

        console.log('Casos asignados:', data);
      },

      error: (err) => {

        this.isLoading = false;

        this.errorMessage =
          err.error?.message ||
          'Error al cargar casos';

        console.error(err);
      }
    });
  }

  crearAcuerdo(): void {

  if (!this.casoSeleccionado) {
    return;
  }

  const agreement = {

    caseId: this.casoSeleccionado.id,

    agreementText: this.agreementText,

    points: [
      {
        description: this.pointDescription,
        deadline: this.pointDeadline
      }
    ]
  };

  this.agreementService.create(agreement)
    .subscribe({

      next: () => {

        alert('Acuerdo creado');

        this.mostrarAcuerdo = false;

        this.agreementText = '';
        this.pointDescription = '';
        this.pointDeadline = '';

      },

      error: (err) => {

        console.error(err);

      }

    });
}


  programarSesion(denuncia: Denuncia): void {

    this.casoSeleccionado = denuncia;

    this.mostrarSesion = true;
  }

  cerrarSesion(): void {

  this.mostrarSesion = false;

  this.scheduledDate = '';
  this.modality = '';
  this.meetingLink = '';

}

crearSesion(): void {

  if (!this.casoSeleccionado) {
    return;
  }

  this.sessionService.create({

    caseId: this.casoSeleccionado.id!,
    scheduledDate: this.scheduledDate,
    modality: this.modality,
    meetingLink: this.meetingLink

  }).subscribe({

    next: () => {

      alert('Sesión programada correctamente');

      this.cerrarSesion();
    },

    error: (err) => {

      console.error(err);

  console.log({
  caseId: this.casoSeleccionado?.id,
  scheduledDate: this.scheduledDate,
  modality: this.modality,
  meetingLink: this.meetingLink
});
    }

  });

}

  }