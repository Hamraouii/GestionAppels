import { Component, OnInit } from '@angular/core';
import { DemandesService } from '../demandes.service';
import { TypeDemande } from '../type-demande.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-type-demande-list',
  templateUrl: './type-demande-list.component.html',
  styleUrls: ['./type-demande-list.component.css'],
  standalone: false
})
export class TypeDemandeListComponent implements OnInit {
  types: TypeDemande[] = [];
  isLoading = true;

  constructor(private demandesService: DemandesService, private router: Router) {}

  ngOnInit(): void {
    this.fetchTypes();
  }

  fetchTypes() {
    this.isLoading = true;
    this.demandesService.getTypes().subscribe({
      next: types => {
        this.types = types;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  onEdit(type: TypeDemande) {
    this.router.navigate(['demandes/types', type.id, 'edit']);
  }

  onDelete(type: TypeDemande) {
    if (confirm('Êtes-vous sûr de vouloir supprimer ce type de demande ?')) {
      this.demandesService.deleteType(type.id).subscribe(() => this.fetchTypes());
    }
  }

  onCreate() {
    this.router.navigate(['demandes/types', 'new']);
  }

  onManageSousTypes(type: TypeDemande) {
    this.router.navigate(['demandes/types', type.id, 'soustypes']);
  }
}
