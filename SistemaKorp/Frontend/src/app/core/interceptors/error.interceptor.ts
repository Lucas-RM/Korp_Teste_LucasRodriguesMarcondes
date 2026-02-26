import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';

import { ApiError } from '../models/api-response.model';
import { NotificationService } from '../../shared/components/notification/notification.service';

function extractApiMessage(error: HttpErrorResponse): string | null {
  const apiError = error.error as Partial<ApiError> | null | undefined;
  if (apiError && typeof apiError === 'object' && typeof apiError.mensagem === 'string') {
    return apiError.mensagem;
  }

  if (typeof error.error === 'string' && error.error.trim().length > 0) {
    return error.error;
  }

  return null;
}

export const errorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        notification.showError('Erro inesperado. Tente novamente.');
        return throwError(() => error);
      }

      const status = error.status;

      if (status === 0) {
        notification.showError('Não foi possível conectar ao servidor. Verifique sua conexão.');
      } else if (status === 400) {
        notification.showError(extractApiMessage(error) ?? 'Requisição inválida.');
      } else if (status === 404) {
        notification.showError('Recurso não encontrado');
      } else if (status === 503) {
        notification.showError(
          'Serviço temporariamente indisponível. Tente novamente mais tarde.',
        );
      } else {
        notification.showError('Erro inesperado. Tente novamente.');
      }

      return throwError(() => error);
    }),
  );
};

