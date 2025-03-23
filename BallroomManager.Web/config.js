const config = {
    development: {
        apiUrl: 'http://localhost:5244/api'
    },
    production: {
        apiUrl: 'https://ballrooms-api.azurewebsites.net/api'
    }
};

const environment = window.location.hostname.includes('localhost') ? 'development' : 'production';
export const apiBaseUrl = 'http://localhost:5244'; 