import { Routes } from '@angular/router';

import { ProdutosComponent } from './features/produtos/produtos.component';
import { NotasFiscaisComponent } from './features/notas-fiscais/notas-fiscais.component';

export const routes: Routes = [
  { path: '', redirectTo: 'produtos', pathMatch: 'full' },
  { path: 'produtos', component: ProdutosComponent, title: 'Produtos' },
  { path: 'notas-fiscais', component: NotasFiscaisComponent, title: 'Notas Fiscais' },
];
