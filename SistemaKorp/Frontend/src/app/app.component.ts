import { CommonModule } from '@angular/common';
import { Component, DestroyRef, ViewChild, computed, inject } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { filter, map } from 'rxjs';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';

import { SidebarComponent } from './layout/sidebar/sidebar.component';
import { NotificationComponent } from './shared/components/notification/notification.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    SidebarComponent,
    NotificationComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly breakpointObserver = inject(BreakpointObserver);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild(MatSidenav)
  sidenav?: MatSidenav;

  readonly isMobile = toSignal(
    this.breakpointObserver
      .observe([Breakpoints.Handset, Breakpoints.TabletPortrait])
      .pipe(map((result) => result.matches)),
    { initialValue: false },
  );

  readonly sidenavMode = computed(() => (this.isMobile() ? 'over' : 'side'));
  readonly sidenavOpened = computed(() => !this.isMobile());

  constructor() {
    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => {
        if (this.isMobile()) {
          void this.sidenav?.close();
        }
      });
  }

  toggleSidenav(): void {
    void this.sidenav?.toggle();
  }
}
