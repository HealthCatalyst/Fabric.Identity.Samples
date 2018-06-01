#!/bin/bash
identitybaseurl=http://localhost:5001

docker stop api-sample-identity
docker rm api-sample-identity

docker pull healthcatalyst/fabric.identity

docker run -d --name api-sample-identity \
	-p 5001:5001 \
	-e "HostingOptions__StorageProvider=InMemory" \
	-e "IssuerUri=$identitybaseurl" \
	-e "IDENTITYSERVERCONFIDENTIALCLIENTSETTINGS__AUTHORITY=$identitybaseurl" \
	healthcatalyst/fabric.identity
echo "started identity"
sleep 3

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

# register sample api
echo "registering Sample API..."
sampleapiresponse=$(curl -X POST -H "Content-Type: application/json" -H "Authorization: Bearer $accesstoken" -d "{ \"name\": \"sample-api\", \"userClaims\": [\"name\", \"email\", \"role\", \"groups\"], \"scopes\": [{ \"name\": \"sample-api\"}]}" $identitybaseurl/api/apiresource)
echo $sampleapiresponse
echo ""

# register sample api client
echo "registering Sample API Client..."
sampleapiclientresponse=$(curl -X POST -H "Content-Type: application/json" -H "Authorization: Bearer $accesstoken" -d "{ \"clientId\": \"sample-api-client\", \"clientName\": \"Sample API Client\", \"requireConsent\": false, \"allowedGrantTypes\": [\"client_credentials\"], \"allowedScopes\": [\"sample-api\", \"fabric/identity.manageresources\"]}" $identitybaseurl/api/client)
echo $sampleapiclientresponse
sampleapiclientsecret=$(echo $sampleapiclientresponse | grep -oP '(?<="clientSecret":")[^"]*')
echo ""

echo "The Fabric.Installer client secret is:"
echo "\"installerSecret\":\"$installersecret\""
echo "You need this secret if you want to register additional API resources or clients."
echo ""

echo "The Sample API Client secret is:"
echo "\"sampleapiclientsecret\":\"$sampleapiclientsecret\""
echo "You need this secret to get an access token on behalf of this client."
echo ""