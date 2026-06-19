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
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { DenunciaService } from '../../services/denuncia.service';
import { MatSelectModule } from '@angular/material/select';
import { AgreementService } from '../../services/agreement.service';
import { Denuncia } from '../../models/denuncia.model';

@Component({
  selector: 'app-denuncias',
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
    //Tarjeta que contiene componentes de Angular Material
  ],
  templateUrl: './denuncias.html',
  styleUrl: './denuncias.css',
})


export class DenunciasComponent {

  title = '';
  status = 'Pendiente';
  comment = '';
  categoria = '';
  userreport = '';
  mostrarAcuerdo = false;
  acuerdos: any[] = [];
  acuerdoSeleccionado: any = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  //ACA PORQUE HAY CATEGORIAS MAL ESCRITAS XDD
  categorias = [
  { value: 'Ruido', label: 'Ruido' },
  { value: 'Mascotas', label: 'Mascotas' },
  { value: 'AreasComunes', label: 'Áreas Comunes' },
  { value: 'Estacionamiento', label: 'Estacionamiento' },
  { value: 'Otros', label: 'Otros' }
];

  constructor(
    private denunciaService: DenunciaService,
    private agreementService: AgreementService
  ) {}

  denuncias: any[] = [];
  mostrarFormulario = false;

  cargarDenuncias(): void {

  this.denunciaService.getMisDenuncias().subscribe({

    next: (response: any) => {

  console.log("GET:", response);

  this.denuncias = response;

  },

    error: (err) => {
      console.error(err);
    }

  });
}

  mostrarforms(): void {
    this.mostrarFormulario = true;

  }

  crearDenuncia(): void {

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const denuncia = {
      title: this.title,
      status: 'Nuevo',
      comment: this.comment,
      success: true,
      categoria: this.categoria,
      userreport: this.userreport
    };

    console.log('Enviando:', denuncia);

    this.denunciaService.crearDenuncia(denuncia).subscribe({

      next: (response: any) => {

      console.log(response);

      this.successMessage = 'Denuncia creada correctamente';
      this.title = '';
      this.comment = '';
      this.categoria = '';
      this.userreport = '';

      this.cargarDenuncias();

      this.isLoading = false;
    },

      error: (err) => {

        console.error(err);

        this.errorMessage =
          err?.error?.message;

        this.isLoading = false;
      }

    });

  }
  abrirAcuerdo(denuncia: Denuncia): void {

  this.agreementService
    .getByCase(denuncia.id!)
    .subscribe({

      next: (data: any) => {

      this.acuerdos = data as any[];

      this.mostrarAcuerdo = true;

    },

      error: (err) => {

        console.error(err);

      }

    });

}

confirmarAcuerdo(id: string): void {

  this.agreementService
    .confirm(id)
    .subscribe({

  next: (response: any) => {

    alert(response.message);

    this.mostrarAcuerdo = false;

    this.cargarDenuncias();

  },

      error: (err) => {

        console.error(err);

      }

    });

}
}