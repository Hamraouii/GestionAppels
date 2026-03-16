import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DemandesService } from '../demandes.service';
import { SousTypeDemande } from '../type-demande.model';

@Component({
  selector: 'app-sous-type-demande-list',
  templateUrl: './sous-type-demande-list.component.html',
  styleUrls: ['./sous-type-demande-list.component.css'],
  standalone: false
})
export class SousTypeDemandeListComponent implements OnInit {
  sousTypes: SousTypeDemande[] = [];
  typeDemandeId: string = '';
  isLoading = true;

  constructor(
    private demandesService: DemandesService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.typeDemandeId = this.route.snapshot.paramMap.get('typeId') || '';
    this.fetchSousTypes();
  }

  fetchSousTypes() {
    this.isLoading = true;
    this.demandesService.getSousTypes(this.typeDemandeId).subscribe({
      next: sousTypes => {
        this.sousTypes = sousTypes;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  onEdit(sousType: SousTypeDemande) {
    this.router.navigate(['edit', sousType.id], { relativeTo: this.route });
  }

  onDelete(sousType: SousTypeDemande) {
    if (confirm('Êtes-vous sûr de vouloir supprimer ce sous-type ?')) {
      this.demandesService.deleteSousType(sousType.id).subscribe(() => this.fetchSousTypes());
    }
  }

  onCreate() {
    this.router.navigate(['new'], { relativeTo: this.route });
  }
}
