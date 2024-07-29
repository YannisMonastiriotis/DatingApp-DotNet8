import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../Models/member';
import { MemberCardComponent } from "../member-card/member-card.component";

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit{

   memberService = inject(MembersService);


  ngOnInit() : void{
    //console.log('on init load members');
    if(this.memberService.members().length === 0)
    {
      this.loadMembers()

    }
  }

  loadMembers() {
    //console.log('in load members');
    this.memberService.getMembers();
  }
}
