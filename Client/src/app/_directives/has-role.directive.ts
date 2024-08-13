import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]', // *appHasRole
  standalone: true
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole : string[] = [];

  private accountService = inject(AccountService);
  private viewContainr = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);
  constructor() { }

  ngOnInit() : void{
    if(this.accountService.roles()?.some((r: string) => this.appHasRole.includes(r))){
      this.viewContainr.createEmbeddedView(this.templateRef)
    }else{
      this.viewContainr.clear;
    }
  }
}
