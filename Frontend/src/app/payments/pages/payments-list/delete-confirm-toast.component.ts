import { Component } from '@angular/core';
import { ToastPackage } from 'ngx-toastr';

@Component({
  selector: 'app-delete-confirm-toast',
  templateUrl: './delete-confirm-toast.component.html',
  styleUrls: ['./delete-confirm-toast.component.scss']
})
export class DeleteConfirmToastComponent {
  constructor(public readonly toastPackage: ToastPackage) {}

  get message(): string {
    return this.toastPackage.message || 'Do you want to delete this payment?';
  }

  confirm(): void {
    this.toastPackage.triggerAction('yes');
    this.toastPackage.toastRef.close();
  }

  cancel(): void {
    this.toastPackage.triggerAction('no');
    this.toastPackage.toastRef.close();
  }
}