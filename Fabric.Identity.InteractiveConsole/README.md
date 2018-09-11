# HOW TO USE

## Step one setup:

1. clone the git repo: https://github.com/HealthCatalyst/Fabric.Identity.Samples
2. edit appsettings.json
3. set the property 'authority' to the identity server

## Step two run the application

1. Set up VS 2017.  Make sure it is up to date.
2. Copy the fabric-installer secret from the passwords file
3. Paste that in the command line
4. There will be a prompt for the browser.
  a. For the browser, make sure it does not auto log in otherwise you will not be able to log in as another account.
5. Login to the user and password you want to get a refresh token.
6. Use the refresh token in the acceptance test.