import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DemandesService } from '../demandes.service';
import { SousTypeDemande } from '../type-demande.model';

@Component({
  selector: 'app-sous-type-demande-form',
  templateUrl: './sous-type-demande-form.component.html',
  styleUrls: ['./sous-type-demande-form.component.css'],
  standalone: false
})
export class SousTypeDemandeFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  typeDemandeId: string = '';
  sousTypeId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private demandesService: DemandesService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      intitule: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.typeDemandeId = this.route.snapshot.paramMap.get('typeId') || '';
    this.sousTypeId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.sousTypeId && this.sousTypeId !== 'new';
    if (this.isEdit && this.sousTypeId) {
      this.demandesService.getSousType(this.sousTypeId).subscribe(sousType => {
        this.form.patchValue(sousType);
      });
    }
  }

  onSubmit() {
    if (this.form.invalid) return;
    const formValue = this.form.value;
    if (this.isEdit && this.sousTypeId) {
      // For update, the backend likely expects the full SousTypeDemande structure or a specific UpdateDto.
      // Assuming it expects properties similar to SousTypeDemande interface for now.
      const updatePayload: Partial<SousTypeDemande> = {
        intitule: formValue.intitule,
        description: formValue.description,
        typeDemandeId: this.typeDemandeId // Ensure typeDemandeId is included if updatable or required by backend for update
      };
      this.demandesService.updateSousType(this.sousTypeId, updatePayload).subscribe(() => {
        this.router.navigate(['../../'], { relativeTo: this.route });
      });
    } else {
      // For create, map to CreateSousTypeDemandeDto structure
      const createPayload = {
        intitule: formValue.intitule,       // Map to backend DTO
        description: formValue.description,  // Map to backend DTO
        typeDemandeId: this.typeDemandeId           // Ensure correct casing if backend is case-sensitive
      };
      // The service method createSousType expects Partial<SousTypeDemande>,
      // but the HTTP call should send the createPayload structure.
      // We'll adjust the service method or use an intermediate type if strict typing is desired here.
      // For now, casting to any to make it pass, but this highlights a type mismatch between service method signature and actual payload needed.
      this.demandesService.createSousType(createPayload).subscribe(() => {
        this.router.navigate(['../'], { relativeTo: this.route });
      });
    }
  }

  onCancel() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
