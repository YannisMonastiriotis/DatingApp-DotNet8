import { Component, input, OnInit } from '@angular/core';
import { Member } from '../../Models/member';
import { RouterLink } from '@angular/router';


@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent implements OnInit {
  



  member = input.required<Member>();

  currentPhotoUrl?: string;

  ngOnInit(): void {
    //console.log(this.member().knownAs);
    console.log(this.member().username);
    this.setMainPhotoUrl();
  }

  setMainPhotoUrl(): void {
    const mainPhoto = this.member().photos.find(photo => photo.isMain);
    if (mainPhoto) {
      this.currentPhotoUrl = mainPhoto.url;
    }
}
}

