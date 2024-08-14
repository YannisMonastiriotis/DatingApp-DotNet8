import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../Models/member';
import {TabDirective, TabsetComponent, TabsModule} from 'ngx-bootstrap/tabs'
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MembersMessagesComponent } from "../members-messages/members-messages.component";
import { Message } from '../../Models/message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MembersMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit, OnDestroy {

 

  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent
  presenceService = inject(PresenceService)
  private route = inject(ActivatedRoute)
  private accountService= inject(AccountService)
  private router = inject(Router)
  member: Member = {} as Member;
  images: GalleryItem[] =[];
  currentPhotoUrl?: string;
  activeTab?: TabDirective;
  private messageService = inject(MessageService);

  ngOnInit(): void {
    //console.log('ngongintmemberdetails')
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p =>{
          this.images.push(new ImageItem({src:p.url, thumb: p.url}))
        })
      }
    })


    this.route.paramMap.subscribe({
      next: _ => this.onRouteParamsChange()
    })
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

  onRouteParamsChange(){
    const user = this.accountService.currentUser();
    if(!user){
      return;
    }

    if(this.messageService.hubConnection?.state === HubConnectionState.Connected &&
      this.activeTab?.heading === 'Messages') {
        this.messageService.hubConnection.stop().then(() =>{
          this.messageService.createHubConnection(user, this.member.username);
        })

    }
  }

  onTabActivated(data:TabDirective){
    this.activeTab = data;
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {tab: this.activeTab.heading},
      queryParamsHandling:'merge'
    })
   if( this.activeTab?.heading ==='Messages' && this.member){
    const user = this.accountService.currentUser();
    if(!user){
      return;
    }
    this.messageService.createHubConnection(user,this.member.username);
  }else{
    this.messageService.stopHubConnection();
  }
}

ngOnDestroy(): void {
  this.messageService.stopHubConnection();
}
 
  
  setMainPhotoUrl(): void {
    const mainPhoto = this.member?.photos.find(photo => photo.isMain);
    if (mainPhoto) {
      this.currentPhotoUrl = mainPhoto.url;
    }
  }
}
