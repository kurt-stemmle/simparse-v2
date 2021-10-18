dotnet publish -c Release
gcloud app deploy .\Simparse\bin\Release\netcoreapp3.1\publish\app.yaml