# Restore pacakges and build
cd Mvc
dotnet restore
dotnet build
cd ../API
dotnet restore
dotnet build
cd ../Angular
npm install
cd ../Mvc

# Start samples
Start-Process dotnet run
cd ../API
Start-Process dotnet run
cd ../Angular
ng serve
cd ..