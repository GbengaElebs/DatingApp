import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HasSpecificRoleComponent } from './HasSpecificRole.component';
import { HasRoleDirective } from '../_directives/HasRole.directive';

@NgModule({
  declarations: [HasRoleDirective],
 exports: [HasRoleDirective],
  imports: [
    CommonModule
  ],
})
export class HasSpecificRoleModule { }
