import { CommonModule, DatePipe } from '@angular/common';
import {
  AfterViewInit,
  Component,
  DestroyRef,
  ViewChild,
  effect,
  inject,
  signal,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatExpansionModule } from '@angular/material/expansion';
import { finalize, forkJoin } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ItemNotaFiscalDto, NotaFiscal } from '../../core/models/nota-fiscal.model';
import { FaturamentoService } from '../../core/services/faturamento.service';
import { EstoqueService } from '../../core/services/estoque.service';
import { NotificationService } from '../../shared/components/notification/notification.service';

type ItemForm = {
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number | null;
};

type ItemFormGroup = FormGroup<{
  codigoProduto: FormControl<string>;
  descricaoProduto: FormControl<string>;
  quantidade: FormControl<number | null>;
}>;

type NotaFiscalForm = FormGroup<{
  itens: FormArray<ItemFormGroup>;
}>;

@Component({
  selector: 'app-notas-fiscais',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatExpansionModule,
    DatePipe,
  ],
  templateUrl: './notas-fiscais.component.html',
  styleUrl: './notas-fiscais.component.scss',
})
export class NotasFiscaisComponent implements AfterViewInit {
  private readonly fb = inject(FormBuilder);
  private readonly faturamentoService = inject(FaturamentoService);
  private readonly estoqueService = inject(EstoqueService);
  private readonly notification = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  readonly isSubmitting = signal(false);
  readonly isLoadingList = signal(false);
  readonly printingIds = signal<Set<string>>(new Set());
  readonly processingIds = signal<Set<string>>(new Set());

  readonly expandedId = signal<string | null>(null);

  readonly displayedColumns: Array<
    'numero' | 'status' | 'criadoEm' | 'fechadoEm' | 'actions'
  > = ['numero', 'status', 'criadoEm', 'fechadoEm', 'actions'];

  readonly notas = signal<NotaFiscal[]>([]);
  readonly dataSource = new MatTableDataSource<NotaFiscal>([]);

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  readonly form: NotaFiscalForm = this.fb.group({
    itens: this.fb.array<ItemFormGroup>([this.createItemGroup()]),
  });

  constructor() {
    effect(() => {
      this.dataSource.data = this.notas();
    });

    this.carregarNotas();
  }

  ngAfterViewInit(): void {
    if (this.paginator) this.dataSource.paginator = this.paginator;
    if (this.sort) this.dataSource.sort = this.sort;
  }

  get itens(): FormArray<ItemFormGroup> {
    return this.form.controls.itens;
  }

  createItemGroup(): ItemFormGroup {
    return this.fb.group({
      codigoProduto: this.fb.nonNullable.control('', [Validators.required]),
      descricaoProduto: this.fb.nonNullable.control('', [Validators.required]),
      quantidade: this.fb.control<number | null>(null, [Validators.required, Validators.min(0.01)]),
    });
  }

  addItem(): void {
    this.itens.push(this.createItemGroup());
  }

  removeItem(index: number): void {
    if (this.itens.length <= 1) return;
    this.itens.removeAt(index);
  }

  carregarNotas(): void {
    this.isLoadingList.set(true);
    this.faturamentoService
      .listarNotasFiscais()
      .pipe(
        finalize(() => this.isLoadingList.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (notas) => this.notas.set(notas),
      });
  }

  submit(): void {
    if (this.itens.length < 1) {
      this.notification.showWarning('Adicione ao menos 1 item.');
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notification.showWarning('Verifique os campos destacados no formulário.');
      return;
    }

    const itens: ItemNotaFiscalDto[] = this.itens.controls.map((ctrl) => {
      const value: ItemForm = ctrl.getRawValue();
      return {
        codigoProduto: String(value.codigoProduto ?? '').trim(),
        descricaoProduto: String(value.descricaoProduto ?? '').trim(),
        quantidade: Number(value.quantidade ?? 0),
      };
    });

    if (itens.length < 1) {
      this.notification.showWarning('Adicione ao menos 1 item.');
      return;
    }

    this.isSubmitting.set(true);
    this.faturamentoService
      .criarNotaFiscal({ itens })
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notification.showSuccess('Nota fiscal criada com sucesso!');
          this.resetForm();
          this.carregarNotas();
        },
      });
  }

  resetForm(): void {
    this.form.setControl('itens', this.fb.array<ItemFormGroup>([this.createItemGroup()]));
    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.form.updateValueAndValidity();
  }

  toggleExpand(nota: NotaFiscal): void {
    this.expandedId.set(this.expandedId() === nota.id ? null : nota.id);
  }

  isPrinting(id: string): boolean {
    return this.printingIds().has(id);
  }

  isProcessing(id: string): boolean {
    return this.processingIds().has(id);
  }

  imprimir(nota: NotaFiscal): void {
    if (this.isPrinting(nota.id) || this.isProcessing(nota.id)) return;

    // Se a nota já estiver fechada, não chamamos o endpoint de impressão (que faz baixa de estoque).
    // Apenas validamos se os itens ainda existem no estoque e, se estiver tudo ok, geramos o download.
    if (nota.status === 'Fechada') {
      const p = new Set(this.printingIds());
      p.add(nota.id);
      this.printingIds.set(p);

      forkJoin({
        produtos: this.estoqueService.listarProdutos(),
        notaAtualizada: this.faturamentoService.obterNotaFiscal(nota.id),
      })
        .pipe(
          finalize(() => {
            const s = new Set(this.printingIds());
            s.delete(nota.id);
            this.printingIds.set(s);
          }),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: ({ produtos, notaAtualizada }) => {
            const codigosProdutos = new Set(produtos.map((p) => p.codigo));
            const itensInvalidos = notaAtualizada.itens.filter(
              (i) => !codigosProdutos.has(i.codigoProduto),
            );

            if (itensInvalidos.length > 0) {
              const codigosFaltando = Array.from(new Set(itensInvalidos.map((i) => i.codigoProduto))).join(
                ', ',
              );
              this.notification.showWarning(
                `Não é possível baixar esta nota. Os seguintes produtos não existem mais no estoque: ${codigosFaltando}.`,
              );
              return;
            }

            this.downloadNotaFiscalArquivo(notaAtualizada);
          },
          error: () => {
            this.notification.showError(
              'Não foi possível validar os itens da nota para download. Tente novamente mais tarde.',
            );
          },
        });

      return;
    }

    const p = new Set(this.printingIds());
    p.add(nota.id);
    this.printingIds.set(p);

    this.faturamentoService
      .imprimirNotaFiscal(nota.id)
      .pipe(
        finalize(() => {
          const s = new Set(this.printingIds());
          s.delete(nota.id);
          this.printingIds.set(s);
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (resp) => {
          if (!resp.sucesso) {
            console.log(resp.mensagem)
            this.notification.showError(resp.mensagem);
            return;
          }

          if (resp.emProcessamento) {
            const proc = new Set(this.processingIds());
            proc.add(nota.id);
            this.processingIds.set(proc);
            this.notification.showInfo('Nota em processamento. Verifique em instantes.');
            return;
          }

          this.notification.showSuccess(resp.mensagem || 'Nota fiscal impressa com sucesso!');

          if (resp.nota) {
            this.updateNotaLocal(resp.nota.id, { status: 'Fechada', fechadoEm: resp.nota.fechadoEm });
            this.downloadNotaFiscalArquivo(resp.nota);
          } else {
            this.carregarNotas();
          }
        },
      });
  }

  private updateNotaLocal(id: string, patch: Partial<NotaFiscal>): void {
    const next = this.notas().map((n) => (n.id === id ? { ...n, ...patch } : n));
    this.notas.set(next);
  }

  private downloadNotaFiscalArquivo(nota: NotaFiscal): void {
    const fileNameBase = `nota-fiscal-${nota.numero}`;
    const html = this.buildNotaHtml(nota, `${fileNameBase}.html`);

    const blob = new Blob([html], { type: 'text/html;charset=utf-8' });
    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = `${fileNameBase}.html`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(url);
  }

  private buildNotaHtml(nota: NotaFiscal, title: string): string {
    const fechadoEm = nota.fechadoEm ? new Date(nota.fechadoEm).toLocaleString('pt-BR') : '–';
    const criadoEm = new Date(nota.criadoEm).toLocaleString('pt-BR');

    const itensRows = nota.itens
      .map(
        (i) => `
          <tr>
            <td>${this.escapeHtml(i.codigoProduto)}</td>
            <td>${this.escapeHtml(i.descricaoProduto)}</td>
            <td style="text-align:right">${i.quantidade}</td>
          </tr>
        `,
      )
      .join('');

    return `<!doctype html>
<html lang="pt-BR">
  <head>
    <meta charset="utf-8" />
    <title>${this.escapeHtml(title)}</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
      :root { color-scheme: light; }
      body { font-family: Inter, Arial, sans-serif; margin: 24px; color: #1c1c2e; }
      .header { border-bottom: 2px solid #1a237e; padding-bottom: 12px; margin-bottom: 16px; }
      .title { font-size: 20px; font-weight: 700; letter-spacing: 0.3px; }
      .meta { margin-top: 8px; color: #6b7280; font-size: 12px; }
      .grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; margin-top: 16px; }
      .card { border: 1px solid rgba(107,114,128,0.25); border-radius: 10px; padding: 12px; }
      .label { color: #6b7280; font-size: 12px; }
      .value { font-weight: 600; margin-top: 2px; }
      table { width: 100%; border-collapse: collapse; margin-top: 16px; }
      th, td { border: 1px solid rgba(107,114,128,0.25); padding: 10px; font-size: 12px; }
      th { background: rgba(26,35,126,0.06); text-align: left; }
      @media print { body { margin: 0; } }
    </style>
  </head>
  <body>
    <div class="header">
      <div class="title">NOTA FISCAL Nº ${nota.numero}</div>
      <div class="meta">Gerado em ${new Date().toLocaleString('pt-BR')}</div>
    </div>

    <div class="grid">
      <div class="card">
        <div class="label">Status</div>
        <div class="value">${this.escapeHtml(nota.status)}</div>
      </div>
      <div class="card">
        <div class="label">Emitida em</div>
        <div class="value">${this.escapeHtml(criadoEm)}</div>
      </div>
      <div class="card">
        <div class="label">Fechada em</div>
        <div class="value">${this.escapeHtml(fechadoEm)}</div>
      </div>
      <div class="card">
        <div class="label">Quantidade de itens</div>
        <div class="value">${nota.itens.length}</div>
      </div>
    </div>

    <h3 style="margin-top: 20px; font-size: 14px;">Itens</h3>
    <table>
      <thead>
        <tr>
          <th>Código</th>
          <th>Descrição</th>
          <th style="text-align:right">Quantidade</th>
        </tr>
      </thead>
      <tbody>
        ${itensRows}
      </tbody>
    </table>
  </body>
</html>`;
  }

  private escapeHtml(value: string): string {
    return value
      .replaceAll('&', '&amp;')
      .replaceAll('<', '&lt;')
      .replaceAll('>', '&gt;')
      .replaceAll('"', '&quot;')
      .replaceAll("'", '&#039;');
  }
}

