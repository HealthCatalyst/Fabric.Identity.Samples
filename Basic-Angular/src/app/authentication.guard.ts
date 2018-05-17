import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from 'src/app/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationGuard implements CanActivate {
  constructor(private authenticationService: AuthenticationService){}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.authenticationService.isUserAuthenticated().then((result) => {
            if (result) {
                return true;
            } else {               
                this.authenticationService.login();
                return false;
            }
        });
  }
}
