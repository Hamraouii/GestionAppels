import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms'; // Import ReactiveFormsModule

import { FichesRoutingModule } from './fiches-routing.module';
import { FicheListComponent } from './components/fiche-list/fiche-list.component';
import { FicheFormComponent } from './components/fiche-form/fiche-form.component'; // Import FicheFormComponent

// If using Angular Material or other UI libraries, import their modules here
// Example: import { MatTableModule } from '@angular/material/table';
// Example: import { MatButtonModule } from '@angular/material/button';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FichesRoutingModule,
    FicheListComponent, // Import the standalone component
    FicheFormComponent  // Import the standalone component
  ]
})
export class FichesModule { }
