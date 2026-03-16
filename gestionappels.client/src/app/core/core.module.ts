import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'; // Import RouterModule
import { AuthService } from './auth/auth.service';
import { LayoutComponent } from './layout/layout.component'; // Import LayoutComponent

@NgModule({
  imports: [
    CommonModule,
    RouterModule, // For router directives used in the layout
    LayoutComponent // Import the standalone component
  ],
  providers: [
    AuthService
  ],
  exports: [
    LayoutComponent // Export so AppRoutingModule can use it
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule?: CoreModule) {
    if (parentModule) {
      throw new Error(
        'CoreModule is already loaded. Import it in the AppModule only');
    }
  }
}
