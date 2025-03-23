// API Configuration
const API_BASE_URL = 'https://ballroombookingapi.azurewebsites.net/api';

// DOM Elements
const ballroomList = document.getElementById('ballroom-list');
const detailsModal = new bootstrap.Modal(document.getElementById('detailsModal'));
const bookingModal = new bootstrap.Modal(document.getElementById('bookingModal'));

// Ballroom Card Template
function createBallroomCard(ballroom) {
    return `
        <div class="col-md-4">
            <div class="ballroom-card">
                <img src="${ballroom.imageUrl}" alt="${ballroom.name}">
                <div class="ballroom-info">
                    <h3>${ballroom.name}</h3>
                    <p>${ballroom.description}</p>
                    <div class="ballroom-features">
                        <div class="feature-icon">
                            <i class="fas fa-users"></i>
                            <span>${ballroom.capacity} guests</span>
                        </div>
                        <div class="feature-icon">
                            <i class="fas fa-ruler-combined"></i>
                            <span>${ballroom.size}</span>
                        </div>
                    </div>
                    <button class="btn btn-primary w-100" onclick="showDetails(${ballroom.id})">View Details</button>
                </div>
            </div>
        </div>
    `;
}

// API Functions
async function fetchBallrooms() {
    try {
        const response = await fetch(`${API_BASE_URL}/ballrooms`);
        if (!response.ok) throw new Error('Failed to fetch ballrooms');
        const ballrooms = await response.json();
        displayBallrooms(ballrooms);
    } catch (error) {
        console.error('Error fetching ballrooms:', error);
        showError('Failed to load ballrooms. Please try again later.');
    }
}

async function fetchBallroomDetails(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/ballrooms/${id}`);
        if (!response.ok) throw new Error('Failed to fetch ballroom details');
        return await response.json();
    } catch (error) {
        console.error('Error fetching ballroom details:', error);
        showError('Failed to load ballroom details. Please try again later.');
        return null;
    }
}

async function submitBooking() {
    const form = document.getElementById('booking-form');
    const formData = new FormData(form);
    
    const bookingData = {
        ballroomId: parseInt(document.getElementById('ballroom-id').value),
        customerName: formData.get('name'),
        customerEmail: formData.get('email'),
        customerPhone: formData.get('phone'),
        eventDate: formData.get('event-date'),
        eventType: formData.get('event-type'),
        guestCount: parseInt(formData.get('guests')),
        specialRequests: formData.get('special-requests')
    };

    try {
        const response = await fetch(`${API_BASE_URL}/bookings`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(bookingData),
        });
        
        if (!response.ok) throw new Error('Failed to submit booking');
        
        const result = await response.json();
        showSuccess('Booking submitted successfully!');
        bookingModal.hide();
        form.reset();
    } catch (error) {
        console.error('Error submitting booking:', error);
        showError('Failed to submit booking. Please try again later.');
    }
}

// UI Functions
function displayBallrooms(ballrooms) {
    ballroomList.innerHTML = ballrooms.map(createBallroomCard).join('');
}

async function showDetails(ballroomId) {
    const ballroom = await fetchBallroomDetails(ballroomId);
    if (!ballroom) return;

    // Update modal content
    document.getElementById('modal-image').src = ballroom.imageUrl;
    document.getElementById('modal-name').textContent = ballroom.name;
    document.getElementById('modal-description').textContent = ballroom.description;
    document.getElementById('modal-capacity').textContent = `${ballroom.capacity} guests`;
    document.getElementById('modal-size').textContent = ballroom.size;

    // Store ballroom ID for booking
    document.getElementById('ballroom-id').value = ballroomId;

    // Show modal
    detailsModal.show();
}

function showBookingForm() {
    detailsModal.hide();
    bookingModal.show();
}

// Utility Functions
function showSuccess(message) {
    // You can replace this with a better notification system
    alert(message);
}

function showError(message) {
    // You can replace this with a better notification system
    alert(message);
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    fetchBallrooms();
}); 