import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { CreateSessionRequest } from '../models/createsesion.model';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  private apiUrl = 'http://localhost:5224/api/session';

  constructor(private http: HttpClient) {}

  create(session: CreateSessionRequest): Observable<any> {

    const token = localStorage.getItem('token');

    return this.http.post(
      this.apiUrl,
      session,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
  }
}