import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/Accounts'; // Using proxy. Corrected from /api/auth
  private readonly TOKEN_NAME = 'jwt_token';
  private _isAuthenticated = new BehaviorSubject<boolean>(this.hasToken());

  public isAuthenticated$: Observable<boolean> = this._isAuthenticated.asObservable();

  constructor(private http: HttpClient) { }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_NAME);
  }

  login(credentials: any): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        localStorage.setItem(this.TOKEN_NAME, response.token);
        this._isAuthenticated.next(true);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_NAME);
    this._isAuthenticated.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_NAME);
  }
}
