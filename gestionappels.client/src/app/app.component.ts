import { Component, OnInit } from '@angular/core'; // OnInit imported
import { Observable } from 'rxjs';
import { AuthService } from './core/auth/auth.service';
import { Router } from '@angular/router'; // Added Router

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit {
  folded = false;
  opened = true;

  username: string = 'Utilisateur';
  currentYear: number = new Date().getFullYear();

  public isAuthenticated$: Observable<boolean>;

  constructor(private auth: AuthService, private router: Router) {
    this.isAuthenticated$ = this.auth.isAuthenticated$;
  }

  toggleSidebar() {
    this.folded = !this.folded;
  }

  get isAdmin(): boolean {
    const token = this.auth.getToken();
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.role === 'Admin';
    } catch {
      return false;
    }
  }

  ngOnInit(): void {
    this.getUserInfo();
  }

  getUserInfo(): void {
    const token = this.auth.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        // Assuming the JWT payload has a 'name' or 'sub' claim for the username
        this.username = payload.name || payload.sub || 'Utilisateur'; 
      } catch (e) {
        console.error('Error parsing token for username:', e);
        this.username = 'Utilisateur';
      }
    } else {
      this.username = 'Utilisateur';
    }
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }
}

