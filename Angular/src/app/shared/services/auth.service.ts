import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { ConfigService } from './config.service';
import { Config } from '../../models/config';
import { LoggingService } from './logging.service';

@Injectable()
export class AuthService {
  userManager: UserManager; 
  configSettings: Config;
  identityClientSettings: any;

  constructor(private configService: ConfigService, private loggingService: LoggingService) {
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
      loggingService.log("access token expiring");
    });

    this.userManager.events.addSilentRenewError(function(e){
      loggingService.log("silent renew error: " + e.message);
    });

    this.userManager.events.addAccessTokenExpired(function () {
      loggingService.log("access token expired");    
      //when access token expires logout the user
      self.logout();
    });  
   }


  login() {
    var self = this;
    this.userManager.signinRedirect().then(() => {
      self.loggingService.log("signin redirect done");
    }).catch(err => {
      self.loggingService.error(err);
    });
  }

  logout() {
    this.userManager.signoutRedirect();
  }

  handleSigninRedirectCallback() {
    var self = this;
    this.userManager.signinRedirectCallback().then(user => {
      if (user) {
        self.loggingService.log("Logged in: " + JSON.stringify(user.profile));
      } else {
        self.loggingService.log("could not log user in");
      }
    }).catch(e => {
      self.loggingService.error(e);
    });
  }

  getUser(): Promise<User> {
    return this.userManager.getUser();
  }

  isUserAuthenticated() {
    var self = this;
    return this.userManager.getUser().then(function (user) {
      if (user) {
        self.loggingService.log("logging service - signin redirect done: " + JSON.stringify(user.profile));
        return true;
      } else {
        self.loggingService.log("User is not logged in");
        return false;
      }
    });
  }
  

}

