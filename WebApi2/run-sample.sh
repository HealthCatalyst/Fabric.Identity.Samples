#!/bin/bash

if [ $1 ]; then
    sampleapiclientsecret=$1
fi

if ! [ $sampleapiclientsecret ]; then
    echo "You must enter the sample api client secret to run the sample. It is used to get an access token from Fabric.Identity in order to call the sample API."
    exit 1
fi

identitybaseurl=http://localhost:5001

cd ../Fabric.Identity.InteractiveConsole

cat > appsettings.json << EOF
{
  "clientid": "fabric-test-client",
  "scope": "openid profile fabric.profile sample-api offline_access",
  "port": 5002,
  "authority": "http://localhost:5001/",
  "hostBaseUri": "http://localhost" 
}
EOF

dotnet run

cd ../WebApi2

echo ""
read -p "Start the Fabric.Identity.Samples.WebAPI2 sample in Visual Studio now, then press [Enter] to continue."
echo ""
sleep 3

# get access token for sample api client
echo "getting access token for sample api client..."
accesstokenresponse=$(curl $identitybaseurl/connect/token --data "client_id=sample-api-client&grant_type=client_credentials&scope=sample-api" --data-urlencode "client_secret=$sampleapiclientsecret")
echo $accesstokenresponse
accesstoken=$(echo $accesstokenresponse | grep -oP '(?<="access_token":")[^"]*')
echo ""

echo "attempting get on http://localhost:42099/api/values with no access token"
curl -f http://localhost:42099/api/values
echo ""

echo "attempting get on http://localhost:42099/api/values with access token"
curl -f -H "Authorization: Bearer $accesstoken" http://localhost:42099/api/values
echo ""
echo ""

echo "attempting get on http://localhost:42099/api/values/sample-api-client with access token"
curl -f -H "Authorization: Bearer $accesstoken" http://localhost:42099/api/values/sample-api-client
echo ""
echo ""

read -p "Enter the user access_token to call the API with: " useraccesstoken
echo ""

echo "attempting get on http://localhost:42099/api/delegation/sample-api-client with user's access token"
curl -f -H "Authorization: Bearer $useraccesstoken" http://localhost:42099/api/delegation/sample-api-client
echo ""