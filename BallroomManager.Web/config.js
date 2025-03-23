const config = {
    development: {
        apiUrl: 'http://localhost:5244/api'
    },
    production: {
        apiUrl: '' // This will be filled in after Azure deployment
    }
};

const environment = window.location.hostname === 'localhost' ? 'development' : 'production';
export const apiBaseUrl = config[environment].apiUrl; 