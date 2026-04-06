import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import {
  CreatePaymentRequest,
  CreatePaymentResponse,
  GetPaymentsQuery,
  PaymentViewModel,
  PagedPaymentsResponse,
  UpdatePaymentRequest
} from '../models/payment.model';
import { environment as emt } from 'src/environments/enviornement';

@Injectable({
  providedIn: 'root'
})
export class PaymentsApiService {
  private readonly baseUrl = emt.apiUrl+'/api/payments';

  constructor(private readonly http: HttpClient) {}

  createPayment(payload: CreatePaymentRequest): Observable<CreatePaymentResponse> {
    return this.http.post<CreatePaymentResponse>(this.baseUrl, payload);
  }

  getPayments(query?: GetPaymentsQuery): Observable<PagedPaymentsResponse> {
    let params = new HttpParams();

    if (query?.pageNumber) {
      params = params.set('pageNumber', query.pageNumber);
    }

    if (query?.pageSize) {
      params = params.set('pageSize', query.pageSize);
    }

    if (query?.currency) {
      params = params.set('currency', query.currency);
    }

    if (query?.fromCreated) {
      params = params.set('fromCreated', query.fromCreated);
    }

    if (query?.toCreated) {
      params = params.set('toCreated', query.toCreated);
    }

    return this.http.get<PagedPaymentsResponse>(this.baseUrl, { params });
  }

  getPaymentById(id: number): Observable<PaymentViewModel | null> {
    return this.getPayments({ pageNumber: 1, pageSize: 500 }).pipe(
      map((response) => response.items.find((payment) => payment.id === id) ?? null)
    );
  }

  updatePayment(id: number, payload: UpdatePaymentRequest): Observable<PaymentViewModel> {
    return this.http.put<PaymentViewModel>(`${this.baseUrl}/${id}`, payload);
  }

  deletePayment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
