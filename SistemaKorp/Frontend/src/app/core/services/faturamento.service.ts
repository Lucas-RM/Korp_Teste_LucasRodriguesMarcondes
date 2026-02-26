import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CriarNotaFiscalRequest,
  ImprimirNotaFiscalResponse,
  NotaFiscal,
} from '../models/nota-fiscal.model';

@Injectable({ providedIn: 'root' })
export class FaturamentoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.faturamentoApiUrl;

  listarNotasFiscais(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(`${this.baseUrl}/notas-fiscais`);
  }

  obterNotaFiscal(id: string): Observable<NotaFiscal> {
    return this.http.get<NotaFiscal>(`${this.baseUrl}/notas-fiscais/${encodeURIComponent(id)}`);
  }

  criarNotaFiscal(request: CriarNotaFiscalRequest): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.baseUrl}/notas-fiscais`, request);
  }

  imprimirNotaFiscal(id: string): Observable<ImprimirNotaFiscalResponse> {
    return this.http.post<ImprimirNotaFiscalResponse>(
      `${this.baseUrl}/notas-fiscais/${encodeURIComponent(id)}/imprimir`,
      {},
    );
  }
}

