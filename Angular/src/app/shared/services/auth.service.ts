import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';


const clientSettings: any = {
  authority: 'http://localhost:5001/',
  client_id: 'fabric-angularsample',
  redirect_uri: 'http://localhost:4200/oidc-callback.html',
  post_logout_redirect_uri: 'http://localhost:4200',
  response_type: 'id_token token',
  scope: 'openid profile fabric.profile patientapi fabric/authorization.read fabric/authorization.write',  
  silent_redirect_uri: 'http://localhost:4200/silent.html',
  automaticSilentRenew: true,
  //silentRequestTimeout:10000,

  filterProtocolClaims: true,
  loadUserInfo: true
};

@Injectable()
export class AuthService {
  userManager: UserManager = new UserManager(clientSettings);
  
  constructor() {
    var self = this;
    this.userManager.events.addAccessTokenExpiring(function(){
      console.log("access token expiring");
    });

    this.userManager.events.addSilentRenewError(function(e){
      console.log("silent renew error", e.message);
    });

    this.userManager.events.addAccessTokenExpired(function () {
      console.log("access token expired");    
      //when access token expires logout the user
      self.logout();
    });  
   }


  login() {
    this.userManager.signinRedirect().then(() => {
      console.log("signin redirect done");
    }).catch(err => {
      console.log(err);
    });
  }

  logout() {
    this.userManager.signoutRedirect();
  }

  handleSigninRedirectCallback() {
    this.userManager.signinRedirectCallback().then(user => {
      if (user) {
        console.log("Logged in", user.profile);
      } else {
        console.log("could not log user in");
      }
    }).catch(e => {
      console.error(e);
    });
  }

  getUser(): Promise<User> {
    return this.userManager.getUser();
  }

  isUserAuthenticated() {
    return this.userManager.getUser().then(function (user) {
      if (user) {
        console.log("User logged in", user.profile);
        return true;
      } else {
        console.log("User is not logged in");
        return false;
      }
    });
  }
  

}

