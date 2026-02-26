export interface Produto {
  id: string;
  codigo: string;
  descricao: string;
  saldo: number;
  simularFalha: boolean;
  criadoEm: string;
  atualizadoEm: string | null;
}

export interface CriarProdutoRequest {
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface ItemBaixaDto {
  codigoProduto: string;
  quantidade: number;
}

export interface BaixaEstoqueRequest {
  itensBaixa: ItemBaixaDto[];
}

export interface ItemBaixaProcessadoDto {
  codigoProduto: string;
  saldoAnterior: number;
  saldoAtual: number;
  quantidadeBaixada: number;
}

export interface BaixaEstoqueResponse {
  sucesso: boolean;
  mensagem: string;
  itensProcessados: ItemBaixaProcessadoDto[];
}

