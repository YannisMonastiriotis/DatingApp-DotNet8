import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../Models/User';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {

  private adminService = inject(AdminService);

  users: User[]= []
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  private modalService = inject(BsModalService);
  ngOnInit(): void {
    this.getUserWithRoles();
  }

  openRolesModal(user: User){
    const initialState:ModalOptions ={
      class:'modal-lg',
      initialState:{
        title:'User roles',
        username:user.username,
        selectedRoles:[...user.roles],
        availableRoles:['Admin', 'Moderator', 'Member'],
        users: this.users,
        rolesUpdated: false
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);
    this.bsModalRef.onHide?.subscribe({
      next: () =>{
        if(this.bsModalRef.content && this.bsModalRef.content.rolesUpdated){
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminService.updateUserRoles(user.username, selectedRoles).subscribe({
            next: roles => user.roles = roles
          })
        }
      }
    })
  }

  getUserWithRoles(){
    this.adminService.getUserWithRoles().subscribe({
      next: users => this.users  = users
    })
  }
}
