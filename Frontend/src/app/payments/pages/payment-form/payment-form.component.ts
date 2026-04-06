import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CreatePaymentRequest, UpdatePaymentRequest } from '../../models/payment.model';
import { PaymentsApiService } from '../../services/payments-api.service';

@Component({
  selector: 'app-payment-form',
  templateUrl: './payment-form.component.html',
  styleUrls: ['./payment-form.component.scss']
})
export class PaymentFormComponent implements OnInit {
  readonly currencies = ['USD', 'EUR', 'INR', 'GBP'] as const;

  isEditMode = false;
  paymentId: number | null = null;
  isLoading = false;
  isSaving = false;
  errorMessage = '';

  paymentForm = this.fb.group({
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    currency: ['USD', [Validators.required]]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly paymentsApiService: PaymentsApiService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage = 'Invalid payment id.';
      return;
    }

    this.isEditMode = true;
    this.paymentId = id;
    this.loadPaymentForEdit(id);
  }

  get amountControl() {
    return this.paymentForm.controls.amount;
  }

  get currencyControl() {
    return this.paymentForm.controls.currency;
  }

  save(): void {
    this.errorMessage = '';

    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    const amount = Number(this.paymentForm.value.amount);
    const currency = this.paymentForm.value.currency as 'USD' | 'EUR' | 'INR' | 'GBP';

    this.isSaving = true;

    if (this.isEditMode && this.paymentId) {
      const payload: UpdatePaymentRequest = { amount, currency };
      this.paymentsApiService.updatePayment(this.paymentId, payload).subscribe({
        next: () => {
          this.isSaving = false;
          this.toastr.success('Payment updated successfully.');
          this.navigateToListSoon();
        },
        error: () => {
          this.isSaving = false;
          this.toastr.error('Failed to update payment.');
        }
      });

      return;
    }

    const payload: CreatePaymentRequest = {
      amount,
      currency,
      clientRequestId: this.generateClientRequestId()
    };

    this.paymentsApiService.createPayment(payload).subscribe({
      next: (response) => {
        this.isSaving = false;

        if (response.isDuplicate) {
          this.toastr.warning('Duplicate request detected. Existing payment returned.');
        } else {
          this.toastr.success('Payment created successfully.');
        }

        this.navigateToListSoon();
      },
      error: () => {
        this.isSaving = false;
        this.toastr.error('Failed to create payment.');
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/payments']);
  }

  private loadPaymentForEdit(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.paymentsApiService.getPaymentById(id).subscribe({
      next: (payment) => {
        this.isLoading = false;

        if (!payment) {
          this.errorMessage = 'Payment not found.';
          return;
        }

        this.paymentForm.patchValue({
          amount: payment.amount,
          currency: payment.currency as 'USD' | 'EUR' | 'INR' | 'GBP'
        });
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load payment details.';
      }
    });
  }

  private generateClientRequestId(): string {
    if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
      return crypto.randomUUID();
    }

    // Fallback UUID v4 generator for older browsers.
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (char) => {
      const random = Math.floor(Math.random() * 16);
      const value = char === 'x' ? random : (random & 0x3) | 0x8;
      return value.toString(16);
    });
  }

  private navigateToListSoon(): void {
    setTimeout(() => {
      this.router.navigate(['/payments']);
    }, 600);
  }
}
