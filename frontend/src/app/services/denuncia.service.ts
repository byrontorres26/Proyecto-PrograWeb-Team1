import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Denuncia } from '../models/denuncia.model';


@Injectable({
  providedIn: 'root'
})
export class DenunciaService {

  private apiUrl = 'http://localhost:5224';

  constructor(private http: HttpClient) {}

crearDenuncia(denuncia: Denuncia): Observable<any> {

  const token = localStorage.getItem('token');

  return this.http.post(
    `${this.apiUrl}/api/denuncia`,
    denuncia,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}
assignMediator(
  denunciaId: string,
  mediatorId: string,
  mediatorName: string
) {

  const token = localStorage.getItem('token');

  return this.http.patch(
    `${this.apiUrl}/api/denuncia/${denunciaId}/assign-mediator`,
    {
      mediatorId,
      mediatorName
    },
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}

//Mediador
getMyCases(): Observable<Denuncia[]> {

  const token = localStorage.getItem('token');

  return this.http.get<Denuncia[]>(
    `${this.apiUrl}/api/denuncia/my-cases`,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}

//users  
getMisDenuncias() {

  const token = localStorage.getItem('token');

  return this.http.get(
    `${this.apiUrl}/api/denuncia`,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}
//admin
getAll(): Observable<Denuncia[]> {

  const token = localStorage.getItem('token');

  return this.http.get<Denuncia[]>(
    `${this.apiUrl}/api/denuncia/all`,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );
}
}