import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { AfterViewInit, Component, DestroyRef, ViewChild, effect, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleChange, MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { finalize } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { Produto } from '../../core/models/produto.model';
import { EstoqueService } from '../../core/services/estoque.service';
import { NotificationService } from '../../shared/components/notification/notification.service';

@Component({
  selector: 'app-produtos',
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
    MatSlideToggleModule,
    MatTooltipModule,
    DatePipe,
    DecimalPipe,
  ],
  templateUrl: './produtos.component.html',
  styleUrl: './produtos.component.scss',
})
export class ProdutosComponent implements AfterViewInit {
  private readonly fb = inject(FormBuilder);
  private readonly estoqueService = inject(EstoqueService);
  private readonly notification = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  readonly isSubmitting = signal(false);
  readonly isLoadingList = signal(false);
  readonly togglingIds = signal<Set<string>>(new Set());

  readonly displayedColumns: Array<keyof Produto | 'simularFalha'> = [
    'codigo',
    'descricao',
    'saldo',
    'simularFalha',
    'criadoEm',
  ];

  readonly produtos = signal<Produto[]>([]);
  readonly dataSource = new MatTableDataSource<Produto>([]);

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  readonly form = this.fb.nonNullable.group({
    codigo: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(50)]),
    descricao: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(200)]),
    saldo: this.fb.nonNullable.control<number | null>(0, [Validators.required, Validators.min(0)]),
  });

  constructor() {
    effect(() => {
      this.dataSource.data = this.produtos();
    });

    this.carregarProdutos();
  }

  ngAfterViewInit(): void {
    if (this.paginator) this.dataSource.paginator = this.paginator;
    if (this.sort) this.dataSource.sort = this.sort;
  }

  carregarProdutos(): void {
    this.isLoadingList.set(true);
    this.estoqueService
      .listarProdutos()
      .pipe(
        finalize(() => this.isLoadingList.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (produtos) => this.produtos.set(produtos),
      });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notification.showWarning('Verifique os campos destacados no formulário.');
      return;
    }

    const saldoValue = this.form.controls.saldo.value;
    if (saldoValue === null || Number.isNaN(saldoValue)) {
      this.form.controls.saldo.setErrors({ required: true });
      this.form.controls.saldo.markAsTouched();
      this.notification.showWarning('Saldo é obrigatório');
      return;
    }

    this.isSubmitting.set(true);
    const request = {
      codigo: this.form.controls.codigo.value.trim(),
      descricao: this.form.controls.descricao.value.trim(),
      saldo: saldoValue,
    };

    this.estoqueService
      .criarProduto(request)
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notification.showSuccess('Produto cadastrado com sucesso!');
          this.form.reset({ codigo: '', descricao: '', saldo: 0 });
          this.carregarProdutos();
        },
      });
  }

  isToggling(id: string): boolean {
    return this.togglingIds().has(id);
  }

  alternarSimularFalha(produto: Produto, change: MatSlideToggleChange): void {
    if (this.isToggling(produto.id)) return;

    const previousValue = produto.simularFalha;
    const desiredValue = change.checked;

    // Otimista: atualiza UI imediatamente
    this.updateProdutoLocal(produto.id, { simularFalha: desiredValue });

    const nextSet = new Set(this.togglingIds());
    nextSet.add(produto.id);
    this.togglingIds.set(nextSet);

    this.estoqueService
      .alternarSimularFalha(produto.id)
      .pipe(
        finalize(() => {
          const s = new Set(this.togglingIds());
          s.delete(produto.id);
          this.togglingIds.set(s);
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (updated) => {
          this.updateProdutoLocal(produto.id, { simularFalha: updated.simularFalha });
          this.notification.showSuccess('Configuração atualizada com sucesso!');
        },
        error: () => {
          // Interceptor já notifica; aqui apenas revertimos UI
          this.updateProdutoLocal(produto.id, { simularFalha: previousValue });
        },
      });
  }

  private updateProdutoLocal(id: string, patch: Partial<Produto>): void {
    const next = this.produtos().map((p) => (p.id === id ? { ...p, ...patch } : p));
    this.produtos.set(next);
  }
}

