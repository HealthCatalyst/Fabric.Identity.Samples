import { Injectable } from '@angular/core';
import { UserManager, User, Log } from 'oidc-client';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  userManager: UserManager;

  constructor() { 
    var self = this;

    const clientSettings: any = {
      authority: 'https://{fabric-identity-url}',
      client_id: 'sample-application',
      redirect_uri: 'http://localhost:4200/oidc-callback.html',
      post_logout_redirect_uri: 'http://localhost:4200',
      response_type: 'id_token token',
      scope: 'openid profile fabric.profile',  
      silent_redirect_uri: 'http://localhost:4200/silent.html',
      automaticSilentRenew: true,    
      filterProtocolClaims: true,
      loadUserInfo: true
    };

    this.userManager = new UserManager(clientSettings);    

    this.userManager.events.addAccessTokenExpiring(function(){      
      console.log("access token expiring");
    });

    this.userManager.events.addSilentRenewError(function(e){
      console.log("silent renew error: " + e.message);
    });

    this.userManager.events.addAccessTokenExpired(function () {
      console.log("access token expired");    
      //when access token expires logout the user
      self.logout();
    });  
  }

  login() {
    var self = this;
    this.userManager.signinRedirect().then(() => {
      console.log("signin redirect done");
    }).catch(err => {
      console.error(err);
    });
  }

  logout() {
    this.userManager.signoutRedirect();
  }

  isUserAuthenticated() {
    var self = this;
    return this.userManager.getUser().then(function (user) {
      if (user) {
        console.log("signin redirect done. ");
        console.log(user.profile);
        return true;
      } else {
        console.log("User is not logged in");
        return false;
      }
    });
  }

  getUser(): Promise<User> {
    return this.userManager.getUser();
  }
}
