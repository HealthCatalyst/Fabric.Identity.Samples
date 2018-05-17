import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/authentication.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  profile: any = {};
  name: string;

  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.authenticationService.getUser().then(result => {
      if (result) {
          this.profile = result.profile;      
          this.name = this.profile.name;    
      }
    });
  }

}
