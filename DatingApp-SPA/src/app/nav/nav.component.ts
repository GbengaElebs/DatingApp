import { Component, OnInit, NgModule } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { HasRoleDirective } from '../_directives/HasRole.directive';
import { AppModule } from '../app.module';
import { HasSpecificRoleModule } from '../HasSpecificRole/HasSpecificRole.module';
import { CommonModule } from '@angular/common';



@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})

export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }
  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Logged in Successfully');
        // console.log('Logged in Successfully');
      },
      (error) => {
        this.alertify.error(error);
        // console.log('Failed to Login');
      }, () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  loggout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.currentUser = null;
    this.authService.decodedToken = null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
    // console.log('logged out');
  }
}
