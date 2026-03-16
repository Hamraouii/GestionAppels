import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TypeDemandeListComponent } from './type-demande-list/type-demande-list.component';
import { TypeDemandeFormComponent } from './type-demande-form/type-demande-form.component';
import { SousTypeDemandeListComponent } from './sous-type-demande-list/sous-type-demande-list.component';
import { SousTypeDemandeFormComponent } from './sous-type-demande-form/sous-type-demande-form.component';
import { AdminGuard } from './admin.guard';

const routes: Routes = [
  {
    path: 'types',
    canActivate: [AdminGuard],
    children: [
      { path: '', component: TypeDemandeListComponent },
      { path: 'new', component: TypeDemandeFormComponent },
      { path: ':id/edit', component: TypeDemandeFormComponent },
      {
        path: ':typeId/soustypes',
        children: [
          { path: '', component: SousTypeDemandeListComponent },
          { path: 'new', component: SousTypeDemandeFormComponent },
          { path: ':id/edit', component: SousTypeDemandeFormComponent }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DemandesRoutingModule {}
