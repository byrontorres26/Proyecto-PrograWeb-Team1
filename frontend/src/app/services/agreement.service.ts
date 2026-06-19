import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AgreementService {

  private apiUrl = 'http://localhost:5224/api/agreement';

  constructor(private http: HttpClient) {}

  create(agreement: any) {

    const token = localStorage.getItem('token');

    return this.http.post(
      this.apiUrl,
      agreement,
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
  }

getByCase(caseId: string): Observable<any[]> {

  const token = localStorage.getItem('token');

  return this.http.get<any[]>(
    `${this.apiUrl}/case/${caseId}`,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}

getById(id: string) {

  const token = localStorage.getItem('token');

  return this.http.get(
    `${this.apiUrl}/${id}`,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}
  

  confirm(id: string) {

    const token = localStorage.getItem('token');

    return this.http.post(
      `${this.apiUrl}/${id}/confirm`,
      {},
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
  }
}