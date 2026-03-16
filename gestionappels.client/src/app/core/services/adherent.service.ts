import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { AdherentSearchResult } from '../models/adherent-search-result.model';

@Injectable({
  providedIn: 'root'
})
export class AdherentService {
  // Using environment baseUrl would be better, but for now using absolute path
  private apiUrl = 'https://localhost:7103/api/adherents'; // Update with your actual API URL

  constructor(private http: HttpClient) { }

  searchAdherents(term: string): Observable<AdherentSearchResult[]> {
    console.log('AdherentService: searchAdherents called with term:', term);
    
    if (!term || term.trim().length < 1) { // Reduced to 1 char for testing
      console.log('AdherentService: Term is empty or too short, returning empty array');
      return of([]);
    }
    
    const params = new HttpParams().set('term', term);
    console.log('AdherentService: Making HTTP request to:', `${this.apiUrl}/search`, 'with params:', params.toString());
    
    return this.http.get<AdherentSearchResult[]>(`${this.apiUrl}/search`, { params })
      .pipe(
        tap(results => console.log('AdherentService: Received results:', results)),
        catchError(error => {
          console.error('AdherentService: Error fetching results:', error);
          return of([]);
        })
      );
  }
}
