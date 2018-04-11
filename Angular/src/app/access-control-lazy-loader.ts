import { NgModule } from "@angular/core";
import { AccessControlModule } from '@healthcatalyst/fabric-access-control-ui';

const accesscontrolConfig = {
  clientId: 'fabric-angularsample',
  identityProvider: 'windows',
  grain: 'dos',
  securableItem: 'datamarts',
  fabricAuthApiUrl: 'http://localhost/authorization/v1',
  fabricExternalIdpSearchApiUrl: 'http://localhost:5009/v1',
  dataChangeEvent(eventArgs) {
  }
};

@NgModule({
  imports: [
    AccessControlModule.forRoot(accesscontrolConfig)
  ]
})
export class AccessControlLazyLoader {}