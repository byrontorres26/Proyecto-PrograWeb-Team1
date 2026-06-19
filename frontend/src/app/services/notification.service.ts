import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private apiUrl = 'http://localhost:5224/api/notification';

  constructor(private http: HttpClient) {}

  getMyNotifications() {

    const token = localStorage.getItem('token');

    return this.http.get<any>(
      this.apiUrl,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
  }
}