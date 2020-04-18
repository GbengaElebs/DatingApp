import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject} from 'rxjs';
import { map } from 'rxjs/operators';
import { tokenName } from '@angular/compiler';
import {JwtHelperService} from '@auth0/angular-jwt';
import { from } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

constructor(private http: HttpClient) { }
  baseUrl = 'http://localhost:5003/Auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/original.png');
  currentPhotoUrl = this.photoUrl.asObservable();


  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);

  }
login(model: any){

  return this.http.post(this.baseUrl + 'Login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        // console.log(user);
        if (user){
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photosUrl);
        }
      })
    );
}

register(user: User){

  return this.http.post(this.baseUrl + 'Register', user);
}

loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

}
