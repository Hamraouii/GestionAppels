import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Fiche } from '../models/fiche.model';
import { CreateFiche } from '../models/create-fiche.model';
import { UpdateFiche } from '../models/update-fiche.model';
import { StatutDemande } from '../models/statut-demande.enum';
import { TypeDemandeDto, SousTypeDemandeDto } from '../models/demande.model'; // Added import
// import { environment } from '../../../environments/environment'; // For base API URL

@Injectable({
  providedIn: 'root' // Provided in root, so it's a singleton and available app-wide
})
export class FicheService {
  // It's good practice to get the API base URL from environment files
  // For now, assuming the API is served from the same domain, relative path /api
  private apiUrl = '/api/fiches'; // Or: environment.apiUrl + '/fiches';
  private demandesApiUrl = '/api/demandes'; // Added for demandes endpoints

  constructor(private http: HttpClient) { }

  getAllFiches(): Observable<Fiche[]> {
    return this.http.get<Fiche[]>(this.apiUrl);
  }

  getFicheById(id: string): Observable<Fiche> {
    return this.http.get<Fiche>(`${this.apiUrl}/${id}`);
  }

  getFichesByAdherentAffiliation(affiliation: string): Observable<Fiche[]> {
    // The backend endpoint is api/fiches/adherent/{affiliation}
    return this.http.get<Fiche[]>(`${this.apiUrl}/adherent/${affiliation}`);
  }

  createFiche(ficheData: CreateFiche): Observable<Fiche> {
    // The backend expects the Statut as a string, not a number.
    // We convert the enum number to its string name before sending.
    const payload = {
      ...ficheData,
      statut: StatutDemande[ficheData.statut]
    };
    return this.http.post<Fiche>(this.apiUrl, payload);
  }

  updateFiche(id: string, ficheData: UpdateFiche): Observable<void> {
    // Backend returns NoContent (204), so Observable<void>
    return this.http.put<void>(`${this.apiUrl}/${id}`, ficheData);
  }

  deleteFiche(id: string): Observable<void> {
    // Backend returns NoContent (204), so Observable<void>
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Methods for TypeDemande and SousTypeDemande
  getAllTypeDemandes(): Observable<TypeDemandeDto[]> {
    return this.http.get<TypeDemandeDto[]>(`${this.demandesApiUrl}/types`);
  }

  getSousTypeDemandesByTypeId(typeDemandeId: string): Observable<SousTypeDemandeDto[]> {
    return this.http.get<SousTypeDemandeDto[]>(`${this.demandesApiUrl}/types/${typeDemandeId}/soustypes`);
  }
}
