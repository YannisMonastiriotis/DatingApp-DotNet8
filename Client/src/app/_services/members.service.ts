import { HttpClient } from '@angular/common/http';
import { inject, Injectable, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../Models/member';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  
  constructor() {
    console.log('Base API URL:', this.baseUrl);
  }


  getMembers(){
    //console.log('in get members');
    return this.http.get<Member[]>(this.baseUrl +'users');
  }

  getMember(username: string) {
    //console.log('in get member');
    return this.http.get<Member>(this.baseUrl+ 'users/' + username);
  }
  
  
}

