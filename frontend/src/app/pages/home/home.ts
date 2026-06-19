// Un componente => a una pantalla
// 3 partes, ts => logica, html => estructura visual y css => a los estilos

//EL ONINIT ES PARA CUANDO SE INICIA
import { Component, OnInit } from '@angular/core';
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
import { NotificationService } from '../../services/notification.service';



@Component({
  selector: 'app-home',
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
    //Tarjeta que contiene componentes de Angular Material
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
})

export class HomeComponent implements OnInit{

  notifications: any[] = [];
  totalNotifications = 0;

  constructor(
  private notificationService: NotificationService
) {}

cargarNotificaciones(): void {

  this.notificationService.getMyNotifications().subscribe({

    next: (response) => {

      console.log(response);

      this.notifications = response.notifications;

      this.totalNotifications = response.total;

    },

    error: (err) => {

      console.error(err);

    }

  });

}

ngOnInit(): void {
  

  this.cargarNotificaciones();

}




}

