import { Injectable, inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly toastr = inject(ToastrService);

  showSuccess(message: string, title?: string): void {
    this.toastr.success(message, title);
  }

  showError(message: string, title?: string): void {
    this.toastr.error(message, title);
  }

  showWarning(message: string, title?: string): void {
    this.toastr.warning(message, title);
  }

  showInfo(message: string, title?: string): void {
    this.toastr.info(message, title);
  }
}

