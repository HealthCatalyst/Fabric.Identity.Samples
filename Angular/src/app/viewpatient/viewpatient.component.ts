import { Component, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { AuthService } from '../shared/services/auth.service';
import { User } from 'oidc-client';

import { FabricAuthService } from '../shared/services/fabric-auth.service'

@Component({
  selector: 'app-viewpatient',
  templateUrl: './viewpatient.component.html',
  styleUrls: ['./viewpatient.component.css']
})
export class ViewpatientComponent implements OnInit {
    public patientDetails: PatientDetails[];
    public errorMessage: string;
    public authenticatedUser: User;
    public permissions: string[];
    public canEditPatient: boolean;
    public hasViewPermission: boolean;

    constructor(private http: Http, private authService: AuthService, private fabricAuthService: FabricAuthService) {
        
    }

    setUserHasViewPermission(permissions: string[]){
        var self = this;
        permissions.forEach(function(permission){
            if(permission.indexOf('viewpatient') > -1)
            {
                self.hasViewPermission = true;
            }
        });
    }

    ngOnInit() {
        this.fabricAuthService.getPermissionsForUser()
            .then(userPermissions => {
                var permissionsObject = <UserPermissions>userPermissions;
                this.permissions = permissionsObject.permissions; 
                this.setUserHasViewPermission(this.permissions);
                return this.permissions;
            }).then((permissions) =>{
                
                if(!permissions.includes('app/fabric-angularsample.viewpatient'))
                    return Promise.resolve();
                
                this.canEditPatient = permissions.includes('app/fabric-angularsample.editpatient');

                this.authService.getUser().then(user => {
                this.authenticatedUser = user;
                let authHeaders = new Headers();
                authHeaders.append('Authorization', 'Bearer ' + user.access_token);
                authHeaders.append('Content-Type', 'application/json');

                let options = new RequestOptions({ headers: authHeaders });

                this.http.get('http://localhost:5003/patients/123', options).subscribe(
                    result => { this.patientDetails = result.json() as PatientDetails[]; },
                    error => { this.errorMessage = <any> error }
                );
            })
        
        })       
    }   

    
}

interface UserPermissions{
    permissions: string[],
    requestedGrain: string,
    requestedSecurableItem: string
}

interface PatientDetails {
    firstName: string;
    lastName: string;
    dateOfBirth: Date;
    requestingUserClaims: Claims[]
}

interface Claims {
    type: string;
    value: string;
}
