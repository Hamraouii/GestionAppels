import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { LayoutComponent } from './core/layout/layout.component';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  // Authenticated routes are children of LayoutComponent
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'demandes',
        loadChildren: () => import('./features/demandes/demandes.module').then(m => m.DemandesModule)
      },
      {
        path: 'home',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
      },
      {
        path: 'fiches',
        loadChildren: () => import('./features/fiches/fiches.module').then(m => m.FichesModule),
      },
      {
        // Redirect from the base authenticated path to the default page
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
      }
    ]
  },
  // Wildcard route to redirect any unknown paths to the default route
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
