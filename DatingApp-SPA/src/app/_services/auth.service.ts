import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { tokenName } from '@angular/compiler';
import {JwtHelperService} from '@auth0/angular-jwt';
import { from } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

constructor(private http: HttpClient) { }
  baseUrl = 'http://localhost:5003/Auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;

login(model: any){

  return this.http.post(this.baseUrl + 'Login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        // console.log(user);
        if (user){
          localStorage.setItem('token', user.token);
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          console.log(this.decodedToken);
        }
      })
    );
}

register(model: any){

  return this.http.post(this.baseUrl + 'Register', model);
}

loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

}
