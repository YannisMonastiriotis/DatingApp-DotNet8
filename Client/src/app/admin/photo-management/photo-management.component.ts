import { Component, inject, NgModule, OnInit } from '@angular/core';
import { Photo } from '../../Models/photo';
import { AdminService } from '../../_services/admin.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';


@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css'],
 
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = []; // Array to hold the photos
  private adminService = inject(AdminService)
  private snackBar = inject(MatSnackBar)

  ngOnInit(): void {
    this.loadPhotosForApproval();
  }

  // Load photos for approval
  loadPhotosForApproval(): void {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photos: Photo[]) => {
        this.photos = photos;
      },
      error: (err :string) => {
        console.error('Failed to load photos:', err);
      }
    });
  }

  approvePhoto(photoId: number): void {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        this.snackBar.open('Photo approved successfully', 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          horizontalPosition: 'right'
        });
        this.loadPhotosForApproval(); 
      },
      error: (err: string) => {
        this.snackBar.open('Failed to approve photo: ' + err, 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          horizontalPosition: 'right'
        });
      }
    });
  }

  rejectPhoto(photoId: number): void {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        this.snackBar.open('Photo rejected successfully', 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          horizontalPosition: 'right'
        });
        this.loadPhotosForApproval(); 
      },
      error: (err: string) => {
        this.snackBar.open('Failed to reject photo: ' + err, 'Close', {
          duration: 3000,
          verticalPosition: 'top',
          horizontalPosition: 'right'
        });
      }
    });
  }
}
