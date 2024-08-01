import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, OnInit, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../Models/member';
import { MemberListComponent } from '../members/member-list/member-list.component';
import { of, tap } from 'rxjs';
import { Photo } from '../Models/photo';
import { PaginatedResult } from '../Models/pagination';
import { UserParams } from '../Models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  private http = inject(HttpClient);
  accountService = inject(AccountService);

  baseUrl = environment.apiUrl;
  memberCache =  new Map();
  //members = signal<Member[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  resetUserParams()
  {
    this.userParams.set(new UserParams(this.user))
  }

  constructor() {
    console.log('Base API URL:', this.baseUrl)
  }


  getMembers(){

   
    var objValues =Object.values(this.userParams()).join('-');

    const response = this.memberCache.get(objValues);

    if(response){
      return this.setPaginatedResponse(response);
    } 

    let params = this.setPaginationHeaders(this.userParams().pageNumber,this.userParams().pageSize);

      params = params.append('minAge', this.userParams().minAge)
      params = params.append('maxAge', this.userParams().maxAge);
      params = params.append('gender', this.userParams().gender);
      params = params.append('orderBy', this.userParams().orderBy);
    
    //console.log('in get members');
    return this.http.get<Member[]>(this.baseUrl +'users',{observe:'response', params}).subscribe({
      next: response => {
        this.setPaginatedResponse(response);
        this.memberCache.set(objValues, response);
      }
    });
  }
  
  private setPaginatedResponse(response:HttpResponse<Member[]>){
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!)
    })
  }
  private setPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams();
    if(pageNumber && pageSize){
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return params;
  }

  getMember(username: string) {
    const member: Member = [...this.memberCache.values()]
    .reduce((arr , elem)=> arr.concat(elem.body), [])
    .find((m: Member) => m.username === username);
    
    if(member)
    {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl+ 'users/' + username);
  }
  
  updateMember(member: Member){
    return this.http.put(this.baseUrl +'users', member).pipe(
      // tap(() =>{
      //   this.members.update(members => members.map(m =>
      //     m.username === member.username ? member : m))
      // })
    )
  }

  setMainPhoto(photo: Photo)
  {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {}).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(m =>{
      //     if(m.photos.includes(photo)) {
      //       m.photoUrl = photo.url
      //     }
      //     return m;
      //   }))
      // })
    )
  }

  deletePhoto(photo: Photo){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id).pipe(
      // tap(() =>{
      //   this.members.update(members => members.map(
      //     m => {
      //       if(m.photos.includes(photo)){
      //         m.photos = m.photos.filter(x => x.id !== photo.id)
      //       }
      //       return m;
      //     }
      //   ))
      // })
    )
  }
}

