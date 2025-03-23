# Azure Deployment Script
param(
    [string]$ResourceGroupName = "BallroomBooking",
    [string]$Location = "eastus2",
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

# Create Resource Group (if it doesn't exist)
Write-Host "Checking Resource Group..."
$resourceGroup = az group show --name $ResourceGroupName | ConvertFrom-Json
if (!$resourceGroup) {
    Write-Host "Creating Resource Group..."
    az group create --name $ResourceGroupName --location $Location
}

# Try different regions for App Service Plan
$regions = @("eastus2", "westus2", "centralus", "southcentralus")
$planCreated = $false

foreach ($region in $regions) {
    Write-Host "Trying to create App Service Plan in $region..."
    try {
        az appservice plan create --name $AppServicePlanName --resource-group $ResourceGroupName --sku F1 --is-linux --location $region
        $planCreated = $true
        $Location = $region
        break
    }
    catch {
        Write-Host "Failed to create plan in $region. Trying next region..."
    }
}

if (-not $planCreated) {
    Write-Error "Failed to create App Service Plan in any region. Please check your subscription quota."
    exit 1
}

# Create Web App for API
Write-Host "Creating API Web App..."
az webapp create --name $ApiAppName --resource-group $ResourceGroupName --plan $AppServicePlanName --runtime "DOTNET:6.0"

# Create Static Web App for Frontend
Write-Host "Creating Static Web App..."
az staticwebapp create --name $WebAppName --resource-group $ResourceGroupName --location $Location

# Deploy API
Write-Host "Deploying API..."
cd BallroomManager.Api
dotnet clean
dotnet build -c Release
dotnet publish -c Release -o ./publish
cd publish
Compress-Archive -Path * -DestinationPath ../publish.zip -Force
az webapp deploy --resource-group $ResourceGroupName --name $ApiAppName --src-path ../publish.zip --type zip

# Deploy Frontend
Write-Host "Deploying Frontend..."
cd ../../BallroomManager.Web
az staticwebapp deploy --name $WebAppName --source .

Write-Host "Deployment completed!"
Write-Host "Frontend URL: https://$WebAppName.azurewebsites.net"
Write-Host "API URL: https://$ApiAppName.azurewebsites.net" 