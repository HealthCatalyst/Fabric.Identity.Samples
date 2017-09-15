import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { ConfigService } from './config.service';
import { Config } from '../../models/config';

@Injectable()
export class AuthService {
  userManager: UserManager; 
  configSettings: Config;
  identityClientSettings: any;

  constructor(private configService: ConfigService) {
    this.configSettings = configService.config;
    var self = this;

    const clientSettings: any = {
      authority: this.configSettings.authority,
      client_id: 'fabric-angularsample',
      redirect_uri: this.configSettings.redirectUri,
      post_logout_redirect_uri: this.configSettings.postLogoutRedirectUri,
      response_type: 'id_token token',
      scope: this.configSettings.scope,  
      silent_redirect_uri: this.configSettings.silentRedirectUri,
      automaticSilentRenew: true,    
      filterProtocolClaims: true,
      loadUserInfo: true
    };

    this.userManager = new UserManager(clientSettings);

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

