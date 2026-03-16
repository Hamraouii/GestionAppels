import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FicheListComponent } from './components/fiche-list/fiche-list.component';
import { FicheFormComponent } from './components/fiche-form/fiche-form.component';

const routes: Routes = [
  {
    path: '',
    component: FicheListComponent
  },
  { path: 'new', component: FicheFormComponent }, // For creating a new fiche
  { path: 'view/:id', component: FicheFormComponent, data: { viewMode: true } }, // For viewing an existing fiche
  { path: ':id/edit', component: FicheFormComponent } // For editing an existing fiche
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FichesRoutingModule { }
