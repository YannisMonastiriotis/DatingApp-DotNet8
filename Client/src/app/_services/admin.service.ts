import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../Models/User';
import { Photo } from '../Models/photo';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient)

  getUserWithRoles(){
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string,roles: string[]){
    return this.http.post<string[]>(this.baseUrl+ 'admin/edit-roles/' + username + '?roles=' + roles, {})
  }

  getPhotosForApproval(){
    return this.http.get<Photo[]>(this.baseUrl +'admin/photos-to-moderate');
  }

  approvePhoto(photoId: number){
    console.log("PHOTO ID APPROVED : " + photoId)
    return this.http.post(this.baseUrl +'admin/approve-photo/' + photoId, {})
  }

  rejectPhoto(photoId: number){
    console.log("PHOTO ID  REJECT: " + photoId)

    return this.http.post(this.baseUrl +'admin/reject-photo/' + photoId , {})
  }

}
