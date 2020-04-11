import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { tokenName } from '@angular/compiler';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

constructor(private http: HttpClient) { }
  baseUrl = 'http://localhost:5003/Auth/';

login(model: any){

  return this.http.post(this.baseUrl + 'Login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        console.log(user);
        if (user){
          localStorage.setItem('token', user.token);
        }
      })
    );
}

register(model: any){

  return this.http.post(this.baseUrl + 'Register', model);
}


}
