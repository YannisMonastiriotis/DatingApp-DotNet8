import { Component, inject, input, OnInit } from '@angular/core';
import { Message } from '../../Models/message';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-members-messages',
  standalone: true,
  imports: [TimeagoModule],
  templateUrl: './members-messages.component.html',
  styleUrl: './members-messages.component.css'
})
export class MembersMessagesComponent implements OnInit {
  
  private messageService = inject(MessageService);
  
  username = input.required<string>();
  messages = input.required<Message[]>();
  ngOnInit(): void {
   
   }

}
