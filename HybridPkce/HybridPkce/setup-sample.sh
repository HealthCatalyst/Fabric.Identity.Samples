﻿#!/bin/bash
identitybaseurl=http://localhost/identity

# register registration api
echo "registering Fabric.Registration..."
registrationapiresponse=$(curl -X POST -H "Content-Type: application/json" -d "{ \"name\": \"registration-api\", \"userClaims\": [\"name\", \"email\", \"role\", \"groups\"], \"scopes\": [{ \"name\": \"fabric/identity.manageresources\"}, { \"name\": \"fabric/identity.read\"}, { \"name\": \"fabric/identity.searchusers\"}]}" $identitybaseurl/api/apiresource)
echo $registrationapiresponse
echo ""

# register the installer client
echo "registering Fabric.Installer..."
installerresponse=$(curl -X POST -H "Content-Type: application/json" -d "{ \"clientId\": \"fabric-installer\", \"clientName\": \"Fabric Installer\", \"requireConsent\": false, \"allowedGrantTypes\": [\"client_credentials\"], \"allowedScopes\": [\"fabric/identity.manageresources\", \"fabric/identity.read\", \"fabric/authorization.read\", \"fabric/authorization.write\", \"fabric/authorization.manageclients\"]}" $identitybaseurl/api/client)
echo $installerresponse
installersecret=$(echo $installerresponse | grep -oP '(?<="clientSecret":")[^"]*')
echo ""

# get access token for installer
echo "getting access token for installer..."
accesstokenresponse=$(curl $identitybaseurl/connect/token --data "client_id=fabric-installer&grant_type=client_credentials&scope=fabric/identity.manageresources" --data-urlencode "client_secret=$installersecret")
echo $accesstokenresponse
accesstoken=$(echo $accesstokenresponse | grep -oP '(?<="access_token":")[^"]*')
echo ""

# register hybrid-pkce-android-sample client
echo "registering Hybrid PKCE Android Sample Client..."
sampleclientresponse=$(curl -X POST -H "Content-Type: application/json" -H "Authorization: Bearer $accesstoken" -d "{ \"clientId\": \"hybrid-pkce-android-sample\", \"clientName\": \"Hybrid PKCE Android Sample\", \"requireConsent\": false, \"allowOfflineAccess\": true, \"allowedGrantTypes\": [\"hybrid\"], \"requirePkce\": true, \"requireClientSecret\": false, \"redirectUris\": [\"xamarinformsclients://callback\"], \"allowedScopes\": [\"openid\", \"profile\", \"email\", \"offline_access\", \"fabric/identity.manageresources\"]}" $identitybaseurl/api/client)
echo sampleclientresponse
sampleclientsecret=$(echo $sampleclientresponse | grep -oP '(?<="clientSecret":")[^"]*')
echo ""

echo "The Fabric.Installer client secret is:"
echo "\"installerSecret\":\"$installersecret\""
echo "You need this secret if you want to register additional API resources or clients."
echo ""

echo "The Fabric Hybrid PKCE Android Sample Client secret is:"
echo "\"sampleclientsecret\":\"$sampleclientsecret\""
echo "You need this secret to get an access token on behalf of this client."
echo ""