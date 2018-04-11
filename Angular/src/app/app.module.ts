import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ErrorHandler } from '@angular/core';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavmenuComponent } from './navmenu/navmenu.component';
import { ViewpatientComponent } from './viewpatient/viewpatient.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';

import { AuthGuardService } from './shared/services/auth-guard.service';
import { AuthService } from './shared/services/auth.service';
import { ConfigService } from './shared/services/config.service';
import { FabricAuthService } from './shared/services/fabric-auth.service';
import { LoggingService } from './shared/services/logging.service';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';

import { Ng2SmartTableModule } from 'ng2-smart-table';
import { CustomErrorService } from './shared/services/custom-error.service';
import { httpInterceptorProviders } from './shared/interceptors';

export function loadConfig(config: ConfigService) {
    return () => config.loadConfig();
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavmenuComponent,
    ViewpatientComponent,
    UnauthorizedComponent,
    LoginComponent,
    LogoutComponent
  ],
  imports: [
      BrowserModule,
      FormsModule,
      HttpModule,
      HttpClientModule,
      Ng2SmartTableModule,
      RouterModule.forRoot([
        { path: '', canActivate: [AuthGuardService],  children: [
                { path: '', children: [
                    { path: '', redirectTo: 'home', pathMatch: 'full' },
                    { path: 'home', component: HomeComponent },
                    { path: 'viewpatient', component: ViewpatientComponent },
                    { path: 'unauthorized', component: UnauthorizedComponent },
                    { path: 'login', component: LoginComponent },
                    { path: 'logout', component: LogoutComponent },
                    { path: 'accesscontrol',  loadChildren: './access-control-lazy-loader#AccessControlLazyLoader' }
                ]
                }
            ]
        }
      ])
  ],
  providers: [
      AuthGuardService,
      AuthService,
      FabricAuthService,
      DatePipe,
      ConfigService,
      {
        provide: APP_INITIALIZER, 
        useFactory: loadConfig, 
        deps: [ConfigService],
        multi: true
      },
      LoggingService,
      {provide: ErrorHandler, useClass: CustomErrorService},
      httpInterceptorProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  
}
