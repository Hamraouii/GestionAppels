import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Fiche } from '../../../../core/models/fiche.model';
import { StatutDemande } from '../../../../core/models/statut-demande.enum';
import { FicheService } from '../../../../core/services/fiche.service';

@Component({
  selector: 'app-fiche-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fiche-list.component.html',
  styleUrls: ['./fiche-list.component.scss']
})
export class FicheListComponent implements OnInit {
  fiches$: Observable<Fiche[]> = of([]);
  error: any = null;

  constructor(
    private ficheService: FicheService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadFiches();
  }

  loadFiches(): void {
    this.fiches$ = this.ficheService.getAllFiches().pipe(
      catchError(err => {
        this.error = err;
        console.error('Error loading fiches:', err);
        return of([]);
      })
    );
  }

  getStatutName(statut: StatutDemande): string {
    return StatutDemande[statut];
  }

  createNewFiche(): void {
    this.router.navigate(['/fiches/new']);
  }

  viewFiche(id: string): void {
    this.router.navigate(['/fiches/view', id]);
  }

  editFiche(id: string): void {
    this.router.navigate(['/fiches/edit', id]);
  }

  deleteFiche(id: string): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cette fiche ?')) {
      this.ficheService.deleteFiche(id).subscribe({
        next: () => this.loadFiches(),
        error: (err) => {
          this.error = err;
          console.error('Error deleting fiche:', err);
        }
      });
    }
  }
}

