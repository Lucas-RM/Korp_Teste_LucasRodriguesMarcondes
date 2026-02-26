import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  BaixaEstoqueRequest,
  BaixaEstoqueResponse,
  CriarProdutoRequest,
  Produto,
} from '../models/produto.model';

@Injectable({ providedIn: 'root' })
export class EstoqueService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.estoqueApiUrl;

  listarProdutos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(`${this.baseUrl}/produtos`);
  }

  obterProduto(id: string): Observable<Produto> {
    return this.http.get<Produto>(`${this.baseUrl}/produtos/${encodeURIComponent(id)}`);
  }

  criarProduto(request: CriarProdutoRequest): Observable<Produto> {
    return this.http.post<Produto>(`${this.baseUrl}/produtos`, request);
  }

  baixarEstoque(request: BaixaEstoqueRequest): Observable<BaixaEstoqueResponse> {
    return this.http.post<BaixaEstoqueResponse>(`${this.baseUrl}/produtos/baixa-estoque`, request);
  }

  alternarSimularFalha(id: string): Observable<Produto> {
    return this.http.patch<Produto>(
      `${this.baseUrl}/produtos/${encodeURIComponent(id)}/simular-falha`,
      {},
    );
  }
}

