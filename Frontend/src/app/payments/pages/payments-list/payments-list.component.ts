import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { PaymentViewModel } from '../../models/payment.model';
import { PaymentsApiService } from '../../services/payments-api.service';

@Component({
  selector: 'app-payments-list',
  templateUrl: './payments-list.component.html',
  styleUrls: ['./payments-list.component.scss']
})
export class PaymentsListComponent implements OnInit {
  payments: PaymentViewModel[] = [];
  isLoading = false;
  isDeleting = false;
  errorMessage = '';

  constructor(
    private readonly paymentsApiService: PaymentsApiService,
    private readonly router: Router,
    private readonly toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadPayments();
  }

  loadPayments(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.paymentsApiService.getPayments({ pageNumber: 1, pageSize: 20 }).subscribe({
      next: (response) => {
        this.payments = response.items;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load payments.';
        this.isLoading = false;
      }
    });
  }

  onAddPayment(): void {
    this.router.navigate(['/payments/new']);
  }

  onEditPayment(payment: PaymentViewModel): void {
    this.router.navigate(['/payments', payment.id, 'edit']);
  }

  async onDeletePayment(payment: PaymentViewModel): Promise<void> {
    const result = await Swal.fire({
      title: 'Delete payment?',
      text: `Delete payment ${payment.reference}?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No',
      reverseButtons: true,
      focusCancel: true
    });

    if (result.isConfirmed) {
      this.deletePayment(payment);
    }
  }

  private deletePayment(payment: PaymentViewModel): void {
    this.isDeleting = true;
    this.errorMessage = '';

    this.paymentsApiService.deletePayment(payment.id).subscribe({
      next: () => {
        this.isDeleting = false;
        this.toastr.success('Payment deleted successfully.');
        this.loadPayments();
      },
      error: () => {
        this.toastr.error('Unable to delete payment.');
        this.isDeleting = false;
      }
    });
  }
}
