import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TypeDemande, SousTypeDemande, CreateSousTypeDemandePayload } from './type-demande.model';

@Injectable({ providedIn: 'root' })
export class DemandesService {
  private apiUrl = '/api/Demandes';

  constructor(private http: HttpClient) {}

  // TypeDemande CRUD
  getTypes(): Observable<TypeDemande[]> {
    return this.http.get<TypeDemande[]>(`${this.apiUrl}/types`);
  }
  getType(id: string): Observable<TypeDemande> {
    return this.http.get<TypeDemande>(`${this.apiUrl}/types/${id}`);
  }
  createType(type: Partial<TypeDemande>): Observable<TypeDemande> {
    return this.http.post<TypeDemande>(`${this.apiUrl}/types`, type);
  }
  updateType(id: string, type: Partial<TypeDemande>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/types/${id}`, type);
  }
  deleteType(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/types/${id}`);
  }

  // SousTypeDemande CRUD
  getSousTypes(typeDemandeId: string): Observable<SousTypeDemande[]> {
    return this.http.get<SousTypeDemande[]>(`${this.apiUrl}/types/${typeDemandeId}/soustypes`);
  }
  getSousType(id: string): Observable<SousTypeDemande> {
    return this.http.get<SousTypeDemande>(`${this.apiUrl}/soustypes/${id}`);
  }
  createSousType(sousType: CreateSousTypeDemandePayload): Observable<SousTypeDemande> {
    return this.http.post<SousTypeDemande>(`${this.apiUrl}/soustypes`, sousType);
  }
  updateSousType(id: string, sousType: Partial<SousTypeDemande>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/soustypes/${id}`, sousType);
  }
  deleteSousType(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/soustypes/${id}`);
  }
}
