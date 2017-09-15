import { Component, OnInit } from '@angular/core';
import { AuthService } from '../shared/services/auth.service';
import { LoggingService } from '../shared/services/logging.service';

@Component({
  selector: 'app-navmenu',
  templateUrl: './navmenu.component.html',
  styleUrls: ['./navmenu.component.css']
})
export class NavmenuComponent {
    public isUserAuthenticated = false;
    public profile = {};
    public messages = [];
    constructor(private authService: AuthService, private loggingService: LoggingService) {
        authService.isUserAuthenticated().then(result => {
            this.isUserAuthenticated = result;
        });
        authService.getUser().then(result => {
            if (result) {
                this.profile = result.profile;
            }
        });
      this.messages = loggingService.messages;
    }

  
}
