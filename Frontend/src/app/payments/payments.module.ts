import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { PaymentsRoutingModule } from './payments-routing.module';
import { PaymentsListComponent } from './pages/payments-list/payments-list.component';
import { PaymentFormComponent } from './pages/payment-form/payment-form.component';

@NgModule({
  declarations: [PaymentsListComponent, PaymentFormComponent],
  imports: [CommonModule, ReactiveFormsModule, PaymentsRoutingModule]
})
export class PaymentsModule {}
