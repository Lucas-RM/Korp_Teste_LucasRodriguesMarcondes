import { AfterViewInit, Component, ViewChild, inject } from '@angular/core';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [ToastContainerDirective],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss',
})
export class NotificationComponent implements AfterViewInit {
  private readonly toastr = inject(ToastrService);

  @ViewChild(ToastContainerDirective, { static: true })
  toastContainer!: ToastContainerDirective;

  ngAfterViewInit(): void {
    this.toastr.overlayContainer = this.toastContainer;
  }
}

