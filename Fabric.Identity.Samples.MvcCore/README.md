# Fabric Identity & Authorization Setup
To run the ASP.NET Core MVC sample, you will need to set up a client in your local instances of Fabric.Identity and Fabric.Authorization.

## Fabric.Identity Client

### Prerequisites
* This document assumes you have Fabric.Identity and Fabric.Authorization installed locally.

* To run the sample as is, you will need an `ApiResource` registered with Fabric.Identity that has an `ApiScope` of `patientapi`. If this does not exist, then you will need to remove the `patientapi` scope on line 96 of `Startup.cs`.

### Setup
Assuming your Fabric.Identity instance is running at `http://localhost/identity`, you can run the following `curl` command to create the Fabric.Identity.Samples.CoreMvc client:

`curl -X POST -H "Content-Type: application/json" -H "Authorization: Bearer {access_token}" -d @{identity_client.json} http://localhost/identity/api/client`

where `{access_token}` is the JWT obtained per the instructions at [Retrieving an Access Token from Fabric.Identity](https://github.com/HealthCatalyst/Fabric.Identity/wiki/Retrieving-an-Access-Token-from-Fabric.Identity)

and `{identity_client.json}` is a file containing the following payload:

```
{
    "clientId": "fabric-mvc-core-sample",
    "clientName": "Fabric .NET MVC Core Sample",
    "allowedScopes": [
        "openid",
        "profile",
        "fabric.profile",
        "patientapi",
        "offline_access",
        "fabric/authorization.read",
        "fabric/authorization.write"
    ],
    "allowedGrantTypes": [
        "hybrid"
    ],
    "redirectUris": [
        "http://localhost:5002/signin-oidc"
    ],
    "postLogoutRedirectUris": [
        "http://localhost:5002/signout-callback-oidc"
    ],
    "allowOfflineAccess": true,
    "requireConsent": false
}
```

_Note the `redirectUris` and `postLogoutRedirectUris` assume the sample app is running at http://localhost:5002._

The `IdentityServerConfidentialClientSettings.ClientSecret` in `appSettings.json` should be set to the `clientSecret` contained in the JSON response of the `curl` command.

## Fabric.Authorization Client

Assuming your Fabric.Authorization instance is running at `http://localhost/authorization`, you can run the following `curl` command to create a Fabric.Authorization client:

`curl -X POST -H "Content-Type: application/json" -H "Authorization: Bearer {access_token}" -d @{auth_client.json} http://localhost/authorization/v1/clients`

where `{access_token}` is the JWT obtained per the instructions at [Retrieving an Access Token from Fabric.Identity](https://github.com/HealthCatalyst/Fabric.Identity/wiki/Retrieving-an-Access-Token-from-Fabric.Identity)

and `{auth_client.json}` is a file containing the following payload:

```
{
	"id": "fabric-mvc-core-sample",
	"name": "Fabric .NET MVC Core Sample",
	"topLevelSecurableItem": {"name": "fabric-mvc-core-sample"}	
}
```

## Running the Sample Application
Once you complete the steps above, you can execute the sample by running through Visual Studio debugger and navigating to `http://localhost:5002` or installing the application on your local IIS.