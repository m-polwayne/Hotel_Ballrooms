const config = {
    development: {
        apiUrl: 'http://localhost:5244'
    },
    production: {
        apiUrl: 'https://ballroomBookingApi.azurewebsites.net'
    }
};

const environment = window.location.hostname === 'localhost' ? 'development' : 'production';
const apiBaseUrl = config[environment].apiUrl;

export { apiBaseUrl }; 