import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TypeDemandeListComponent } from './type-demande-list/type-demande-list.component';
import { TypeDemandeFormComponent } from './type-demande-form/type-demande-form.component';
import { SousTypeDemandeListComponent } from './sous-type-demande-list/sous-type-demande-list.component';
import { SousTypeDemandeFormComponent } from './sous-type-demande-form/sous-type-demande-form.component';
import { RouterModule } from '@angular/router';
import { DemandesRoutingModule } from './demandes-routing.module';
import { AdminGuard } from './admin.guard';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [
    TypeDemandeListComponent,
    TypeDemandeFormComponent,
    SousTypeDemandeListComponent,
    SousTypeDemandeFormComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    DemandesRoutingModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatDialogModule,
    MatInputModule
  ]
})
export class DemandesModule {}
