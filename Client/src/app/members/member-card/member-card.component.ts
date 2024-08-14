import { Component, computed, inject, input, OnInit } from '@angular/core';
import { Member } from '../../Models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';


@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent implements OnInit {
  
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);

  member = input.required<Member>();
  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id));
  isOnline = computed(() => this.presenceService.onlineUsers().includes(this.member().username));
  
  toggleLike(){
    this.likeService.toggleLike(this.member().id).subscribe({
      next:() => {
        if(this.hasLiked()){
          this.likeService.likeIds.update(ids=> ids.filter(x => x !== this.member().id))
        } else{
          this.likeService.likeIds.update(ids => [...ids, this.member().id])
        }
      }
    })
  }
  currentPhotoUrl?: string;

  ngOnInit(): void {
    //console.log(this.member().knownAs);
    console.log(this.member().age);

    this.setMainPhotoUrl();
  }

  setMainPhotoUrl(): void {
    const mainPhoto = this.member().photos.find(photo => photo.isMain);
    if (mainPhoto) {
      this.currentPhotoUrl = mainPhoto.url;
    }
}
}

