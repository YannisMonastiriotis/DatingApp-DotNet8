import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../Models/member';
import {TabDirective, TabsetComponent, TabsModule} from 'ngx-bootstrap/tabs'
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MembersMessagesComponent } from "../members-messages/members-messages.component";
import { Message } from '../../Models/message';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MembersMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs?: TabsetComponent
  private memberService = inject(MembersService)
  private route = inject(ActivatedRoute)
  member?: Member;
  images: GalleryItem[] =[];
  currentPhotoUrl?: string;
  activeTab?: TabDirective;
  messages: Message[] = [];
  private messageService = inject(MessageService);
  
  ngOnInit(): void {
    //console.log('ngongintmemberdetails')
    this.loadMember()
    this.setMainPhotoUrl();

    this.route.queryParams.subscribe({
      next: params=>{
        params['tab'] && this.selectTab(params['tab'])
      }
    })
  }

  selectTab(heading: string){
    if(this.memberTabs){
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading)
      if(messageTab){
        messageTab.active = true;
      }
    }
  }
  onTabActivated(data:TabDirective){
    this.activeTab = data;
   if( this.activeTab?.heading ==='Messages' && this.messages.length ===0 && this.member){
    this.messageService.getMessageThread(this.member.username).subscribe({
      next:messages =>this.messages = messages
    })
  }
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
