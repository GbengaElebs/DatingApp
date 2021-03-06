import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginationResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';
import { Photo } from '../_models/photo';




@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getUsers(page?, itemsperPage?, userParams?, likesParam?): Observable<PaginationResult<User[]>> {
  const paginatedResult: PaginationResult<User[]> = new PaginationResult<User[]>();
  let params =  new HttpParams();

  if (page != null && itemsperPage != null)
  {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsperPage);
  }
  if (userParams != null){
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);


  }
  if (likesParam === 'Likers' ){
      params = params.append('liker', 'true');

  }
  if (likesParam === 'Likees' ){
    params = params.append('likee', 'true');

}

  return this.http.get<User[]>(this.baseUrl + 'Users/' + 'GetUsers' , {observe: 'response', params})
  .pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') != null){
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  )
  ;
}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'Users/' + id );
}

getUserEdit(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'Users/' + 'GetUserEdit/' + id );
}
updateuser(id: number, user: User) {
  return this.http.put(this.baseUrl + 'Users/' + id, user);
}

setMainPhoto(userId: number, id: number ) {
return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {} );
}

deletePhoto(userId: number, id: number ) {
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id );
  }

  sendLike(id: number, recipientId: number){
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }

  getMessages(id: number, page?, itemsperPage?, messageContainer?){
    const paginatedResult: PaginationResult<Message[]> = new PaginationResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsperPage != null)
    {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsperPage);
    }

    return this.http.get<Message[]>(this.baseUrl + id + '/messages', {observe: 'response', params})
        .pipe(
          map(response => {
            paginatedResult.result = response.body;
            if (response.headers.get('Pagination') != null){
              paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
            }
            return paginatedResult;
          })
        );
  }

  getMessageThread(id: number, recipientId: number){
      return this.http.get<Message[]>(this.baseUrl +  id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message){
    return this.http.post(this.baseUrl + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number){
    return this.http.post(this.baseUrl + userId + '/messages/' + id, {} );
  }

  markAsRead(userId: number, messageId: number){
    return this.http.post(this.baseUrl + userId + '/messages/' + messageId + '/read', {})
    .subscribe();

  }

  getPhotoForApproval(){
    return this.http.get<Photo[]>(this.baseUrl + 'admin/' + 'photosForModeration/');

  }

  ApprovePhotosForUser(publiccId: string){
    return this.http.post(this.baseUrl + 'admin/' + 'ApprovePhotosForUser/' + publiccId, {});

  }

  disapprovePhotosForUser(publiccId: string){
    return this.http.delete(this.baseUrl + 'admin/' + 'DisapprovePhotoForUser/' + publiccId, {});

  }
}
