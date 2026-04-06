import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PaymentFormComponent } from './pages/payment-form/payment-form.component';
import { PaymentsListComponent } from './pages/payments-list/payments-list.component';

const routes: Routes = [
  {
    path: '',
    component: PaymentsListComponent
  },
  {
    path: 'new',
    component: PaymentFormComponent
  },
  {
    path: ':id/edit',
    component: PaymentFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PaymentsRoutingModule {}
