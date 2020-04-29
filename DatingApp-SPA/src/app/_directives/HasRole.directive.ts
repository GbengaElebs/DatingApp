import { Directive, Input, ViewContainerRef, TemplateRef, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Directive({
  selector: '[appRole]'
})
export class HasRoleDirective  implements OnInit{
  @Input() appRole: string[];
  appa: string[];
  isVisible = false;

  constructor(private viewContainerRef: ViewContainerRef,
              private templateRef: TemplateRef<any>,
              private authService: AuthService) { }

  ngOnInit() {
    const userRoles =  this.authService.decodedToken.role as Array<string>;
    // if no roles clear the view container ref
              // if user has role need them render the element

    if (this.authService.roleMatch(this.appRole)){
                  if (!this.isVisible ){
                    this.isVisible = true;
                    this.viewContainerRef.createEmbeddedView(this.templateRef);
                  }else{
                    this.isVisible = false;
                    this.viewContainerRef.clear();
                  }

  }else{
    console.log('not allowed');
  }
}
}


