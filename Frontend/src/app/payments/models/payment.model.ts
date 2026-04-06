export interface PaymentViewModel {
  id: number;
  reference: string;
  amount: number;
  currency: string;
  clientRequestId: string;
  createdAt: string;
}

export interface CreatePaymentRequest {
  amount: number;
  currency: 'USD' | 'EUR' | 'INR' | 'GBP';
  clientRequestId: string;
}

export interface CreatePaymentResponse {
  payment: PaymentViewModel;
  isDuplicate: boolean;
}

export interface UpdatePaymentRequest {
  amount: number;
  currency: 'USD' | 'EUR' | 'INR' | 'GBP';
}

export interface GetPaymentsQuery {
  pageNumber?: number;
  pageSize?: number;
  currency?: string;
  fromCreated?: string;
  toCreated?: string;
}

export interface PagedPaymentsResponse {
  items: PaymentViewModel[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
