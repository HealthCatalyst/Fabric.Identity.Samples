# Fabric.Identity.Samples

The samples in this project demostrate how to integrate with the Fabric.Identity service for authentication and authorization.

## Platform

These samples are built using various technologies including:

+ .NET Core 1.1
+ Nancy
+ Angular2

## How to build and run the samples:

+ [Install .NET Core 1.1](https://www.microsoft.com/net/core#windowsvs2017)
+ [Install Node and NPM](https://docs.npmjs.com/getting-started/installing-node)
+ [Install the Angular CLI](https://github.com/angular/angular-cli)
+ [Ensure you have the Fabric.Idenity service running locally](https://github.com/HealthCatalyst/Fabric.Identity)
+ Clone or download the repo
+ Run `.\runSamples.ps1` from Powershell

## Sample Details

### MVC Sample
This is an ASP .NET Core MVC project that is setup to use the Fabric.Identity service to perform authentication and authorization. Authentication is done using Hybrid flow.
The web server for this application listens on port 5002.

### Angular Sample
This is an Angular2 application built with the angular cli and is setup to leverage the Fabric.Identity service to perform authentication and authorization. Authentication is done using Implicit flow.
The web server for this application listens on port 4200.

### API Sample
This is a .NET Core Nancy project set up as an API that is restricted to use by only client that have valid access tokens provided by Fabric.Identity.
The web server for this application listens on port 5003.

