import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MediatorResponse } from '../models/mediator.model';
import { CreateMediatorRequest } from '../models/createmediator.model';

@Injectable({
  providedIn: 'root'
})
export class MediatorService {

  private apiUrl = 'http://localhost:5224/api/Mediator';

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token');

    return {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`
      })
    };
  }

  getAll(): Observable<MediatorResponse[]> {
  return this.http.get<MediatorResponse[]>(
    this.apiUrl,
    this.getHeaders()
  );
}

getById(id: string): Observable<MediatorResponse> {
  return this.http.get<MediatorResponse>(
    `${this.apiUrl}/${id}`,
    this.getHeaders()
  );
}


create(mediator: any): Observable<MediatorResponse> {
  return this.http.post<MediatorResponse>(
    this.apiUrl,
    mediator,
    this.getHeaders()
  );
}

  update(id: string, mediator: any): Observable<any> {
    return this.http.put<any>(
      `${this.apiUrl}/${id}`,
      mediator,
      this.getHeaders()
    );
  }

  activate(id: string): Observable<any> {
    return this.http.patch<any>(
      `${this.apiUrl}/${id}/activate`,
      {},
      this.getHeaders()
    );
  }

  deactivate(id: string): Observable<any> {
    return this.http.patch<any>(
      `${this.apiUrl}/${id}/deactivate`,
      {},
      this.getHeaders()
    );
  }
}