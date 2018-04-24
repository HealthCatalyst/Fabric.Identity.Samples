import { NgModule, Injectable } from '@angular/core';
import { AccessControlModule, IAccessControlConfigService } from '@healthcatalyst/fabric-access-control-ui';
import { IDataChangedEventArgs } from '@healthcatalyst/fabric-access-control-ui/src/app/models/changedDataEventArgs.model';
import { Exception } from '@healthcatalyst/fabric-access-control-ui/src/app/models/exception.model';
import { Subject } from 'rxjs/Subject';
import { Config } from './app.module';

@Injectable()
class AccessControlConfig implements IAccessControlConfigService {
    dataChanged: Subject<IDataChangedEventArgs> = new Subject<IDataChangedEventArgs>();
    errorRaised: Subject<Exception> = new Subject<Exception>();

    clientId = 'atlas';
    identityProvider = 'windows';
    grain = 'dos';
    securableItem = 'datamarts';
    fabricAuthApiUrl = this.config.AuthUrl;
    fabricExternalIdpSearchApiUrl = this.config.IdpSearchUrl;

    constructor(private config: Config) {
        this.dataChanged.subscribe((eventArgs: IDataChangedEventArgs) => {
            // tslint:disable-next-line:no-console
            console.log(`Data changed: ${JSON.stringify(eventArgs)}`);
        });

        this.errorRaised.subscribe((eventArgs: Exception) => {
            // tslint:disable-next-line:no-console
            console.log(`Error: ${JSON.stringify(eventArgs)}`);
        });
    }
}


@NgModule({
  imports: [
    AccessControlModule.forRoot(AccessControlConfig)
  ]
})
export class AccessControlLazyLoader {}
