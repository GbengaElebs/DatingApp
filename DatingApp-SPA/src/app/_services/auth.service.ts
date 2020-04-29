import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject} from 'rxjs';
import { map } from 'rxjs/operators';
import { tokenName } from '@angular/compiler';
import {JwtHelperService} from '@auth0/angular-jwt';
import { from } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

constructor(private http: HttpClient) { }
baseUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/original.png');
  currentPhotoUrl = this.photoUrl.asObservable();


  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);

  }
login(model: any){

  return this.http.post(this.baseUrl + 'auth/' + 'Login', model)
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

  return this.http.post(this.baseUrl + 'auth/' + 'Register', user);
}

loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

roleMatch(allowedRoles): boolean {
  let IsMatch = false;
  const userRoles = this.decodedToken.role as Array<string>;
  allowedRoles.forEach((element) => {
    if(userRoles.includes(element)){
        IsMatch = true;
        return;
    }
  });
  return IsMatch;
}

}
