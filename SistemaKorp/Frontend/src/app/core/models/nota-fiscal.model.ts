export interface ItemNotaFiscalDto {
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number;
}

export interface ItemNotaFiscalResponseDto {
  id: string;
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number;
}

export interface NotaFiscal {
  id: string;
  numero: number;
  status: 'Aberta' | 'Fechada';
  itens: ItemNotaFiscalResponseDto[];
  criadoEm: string;
  fechadoEm: string | null;
}

export interface CriarNotaFiscalRequest {
  itens: ItemNotaFiscalDto[];
}

export interface ImprimirNotaFiscalResponse {
  sucesso: boolean;
  mensagem: string;
  nota: NotaFiscal | null;
  emProcessamento: boolean;
}

