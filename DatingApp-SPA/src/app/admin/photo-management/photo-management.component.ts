import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { Photo } from 'src/app/_models/photo';
import { Observable } from 'rxjs';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  constructor(private userService: UserService, private alertify: AlertifyService) { }
  photo: any;

  ngOnInit() {
    this.getPhotoforApproval();

  }

  getPhotoforApproval(){

    this.userService.getPhotoForApproval().subscribe( phot => {
     this.photo = phot;
    }, error => {
      console.log(error);
    });

  }

  ApprovePhotosForUser(publiccId: string){
    this.userService.ApprovePhotosForUser(publiccId).subscribe(next => {
      this.alertify.success('photo approved Successfully');
      this.photo.splice(this.photo.findIndex(p => p.publiccId === publiccId), 1);
    }, error => {
      this.alertify.error(error);
    });
  }

  disApprovePhotosForUser(publiccId: string){
    this.userService.disapprovePhotosForUser(publiccId).subscribe(next => {
      this.alertify.success('photo disaprroved');
      this.photo.splice(this.photo.findIndex(p => p.publiccId === publiccId), 1);
    }, error => {
      this.alertify.error(error);
    });
  }

}
