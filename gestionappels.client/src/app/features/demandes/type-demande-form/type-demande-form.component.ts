import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DemandesService } from '../demandes.service';
import { TypeDemande } from '../type-demande.model';

@Component({
  selector: 'app-type-demande-form',
  templateUrl: './type-demande-form.component.html',
  styleUrls: ['./type-demande-form.component.css'],
  standalone: false
})
export class TypeDemandeFormComponent implements OnInit {
  form: FormGroup;
  isEdit = false;
  typeId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private demandesService: DemandesService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      intituleDemande: ['', Validators.required],
      descriptionDemande: ['']
    });
  }

  ngOnInit(): void {
    this.typeId = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.typeId && this.typeId !== 'new';
    if (this.isEdit && this.typeId) {
      this.demandesService.getType(this.typeId).subscribe(type => {
        this.form.patchValue(type);
      });
    }
  }

  onSubmit() {
    if (this.form.invalid) return;
    const data = this.form.value;
    if (this.isEdit && this.typeId) {
      this.demandesService.updateType(this.typeId, data).subscribe(() => {
        this.router.navigate(['../'], { relativeTo: this.route });
      });
    } else {
      this.demandesService.createType(data).subscribe(() => {
        this.router.navigate(['../'], { relativeTo: this.route });
      });
    }
  }

  onCancel() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
