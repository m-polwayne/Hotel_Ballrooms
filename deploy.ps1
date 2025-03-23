# Azure Deployment Script
param(
    [string]$ResourceGroupName = "BallroomBooking",
    [string]$Location = "eastus",
    [string]$AppServicePlanName = "BallroomBookingPlan",
    [string]$WebAppName = "BallroomBookingWeb",
    [string]$ApiAppName = "BallroomBookingApi"
)

# Check if Azure CLI is installed
if (!(Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Error "Azure CLI is not installed. Please install it from https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
}

# Check if logged in to Azure
$account = az account show | ConvertFrom-Json
if (!$account) {
    Write-Host "Please log in to Azure using 'az login'"
    exit 1
}

# Create Resource Group
Write-Host "Creating Resource Group..."
az group create --name $ResourceGroupName --location $Location

# Create App Service Plan
Write-Host "Creating App Service Plan..."
az appservice plan create --name $AppServicePlanName --resource-group $ResourceGroupName --sku F1 --is-linux

# Create Web App for API
Write-Host "Creating API Web App..."
az webapp create --name $ApiAppName --resource-group $ResourceGroupName --plan $AppServicePlanName --runtime "DOTNET|6.0"

# Create Static Web App for Frontend
Write-Host "Creating Static Web App..."
az staticwebapp create --name $WebAppName --resource-group $ResourceGroupName --location $Location

# Deploy API
Write-Host "Deploying API..."
cd BallroomManager.Api
dotnet publish -c Release
cd bin/Release/net6.0/publish
Compress-Archive -Path * -DestinationPath ../publish.zip -Force
az webapp deployment source config-zip --resource-group $ResourceGroupName --name $ApiAppName --src ../publish.zip

# Deploy Frontend
Write-Host "Deploying Frontend..."
cd ../../../../
cd BallroomManager.Web
az staticwebapp deploy --name $WebAppName --source .

Write-Host "Deployment completed!"
Write-Host "Frontend URL: https://$WebAppName.azurewebsites.net"
Write-Host "API URL: https://$ApiAppName.azurewebsites.net" 