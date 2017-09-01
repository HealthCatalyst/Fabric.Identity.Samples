import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavmenuComponent } from './navmenu/navmenu.component';
import { ViewpatientComponent } from './viewpatient/viewpatient.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';

import { AuthGuardService } from './shared/services/auth-guard.service';
import { AuthService } from './shared/services/auth.service';
import { FabricAuthService } from './shared/services/fabric-auth.service';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';

import { Ng2SmartTableModule } from 'ng2-smart-table';

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
      Ng2SmartTableModule,
      RouterModule.forRoot([
        { path: '', canActivate: [AuthGuardService],  children: [
                { path: '', children: [
                    { path: '', redirectTo: 'home', pathMatch: 'full' },
                    { path: 'home', component: HomeComponent },
                    { path: 'viewpatient', component: ViewpatientComponent },
                    { path: 'unauthorized', component: UnauthorizedComponent },
                    { path: 'login', component: LoginComponent },
                    { path: 'logout', component: LogoutComponent }
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
      DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  
}
