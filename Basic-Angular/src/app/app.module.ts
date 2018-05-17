import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { AuthenticationGuard } from 'src/app/authentication.guard';
import { AuthenticationService } from 'src/app/authentication.service';
import { HomeComponent } from './home/home.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot([
      {path: '', canActivate: [AuthenticationGuard], children:[
        {path: '', redirectTo: 'home', pathMatch: 'full'},
        {path: 'home', component: HomeComponent }
      ]}
    ])
  ],
  providers: [
    AuthenticationService,
    AuthenticationGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
