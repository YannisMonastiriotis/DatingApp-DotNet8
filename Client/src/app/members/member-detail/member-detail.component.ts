import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../Models/member';
import {TabsModule} from 'ngx-bootstrap/tabs'
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule,GalleryModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {
  private memberService = inject(MembersService)
  private route = inject(ActivatedRoute)
  member?: Member;
  images: GalleryItem[] =[];
  currentPhotoUrl?: string;
  
  ngOnInit(): void {
    console.log('ngongintmemberdetails')
    this.loadMember()
    this.setMainPhotoUrl();
  }
  loadMember(){
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: member=>{
        this.member = member;
        member.photos.map(p =>{
          this.images.push(new ImageItem({src:p.url, thumb: p.url}))
        })

      }
         
    })
  }

  
  setMainPhotoUrl(): void {
    const mainPhoto = this.member?.photos.find(photo => photo.isMain);
    if (mainPhoto) {
      this.currentPhotoUrl = mainPhoto.url;
    }
  }
}
