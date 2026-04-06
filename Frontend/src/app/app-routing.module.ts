import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'payments',
    pathMatch: 'full'
  },
  {
    path: 'payments',
    loadChildren: () => import('./payments/payments.module').then((m) => m.PaymentsModule)
  },
  {
    path: '**',
    redirectTo: 'payments'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
