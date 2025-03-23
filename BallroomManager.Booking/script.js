import { apiBaseUrl } from './config.js';

// Make functions globally available
window.showDetails = showDetails;
window.showBookingForm = showBookingForm;
window.submitBooking = submitBooking;

let selectedBallroom = null;

// Function to get appropriate image URL
function getImageUrl(ballroom) {
    if (!ballroom.imageUrl) {
        // Return a default image URL when no image is available
        return 'https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=800&q=80';
    }
    
    // Extract the filename from either a full URL or just the filename
    let filename = ballroom.imageUrl;
    if (ballroom.imageUrl.includes('/')) {
        filename = ballroom.imageUrl.split('/').pop();
    }
    
    // In development, always use the local API endpoint
    if (window.location.hostname === 'localhost') {
        return `${apiBaseUrl}/ballrooms/images/${filename}`;
    }
    
    return ballroom.imageUrl;
}

document.addEventListener("DOMContentLoaded", async () => {
    // Wait for Bootstrap to be available
    if (typeof bootstrap === 'undefined') {
        console.error('Bootstrap is not loaded');
        return;
    }

    // Set minimum date for event date input to today
    const today = new Date().toISOString().split('T')[0];
    document.getElementById("event-date").min = today;

    // Initialize all Bootstrap modals
    const modals = document.querySelectorAll('.modal');
    modals.forEach(modal => {
        new bootstrap.Modal(modal);
    });

    // Fetch ballrooms after everything is initialized
    await fetchBallrooms();
});

async function fetchBallrooms() {
    try {
        console.log('Fetching ballrooms from:', apiBaseUrl);
        const response = await fetch(`${apiBaseUrl}/ballrooms`);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('API Error:', errorText);
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const ballrooms = await response.json();
        console.log('Ballrooms data:', ballrooms);
        
        if (!Array.isArray(ballrooms)) {
            console.error('Invalid ballrooms data:', ballrooms);
            throw new Error('Invalid data format received from API');
        }
        
        displayBallrooms(ballrooms);
    } catch (error) {
        console.error('Error fetching ballrooms:', error);
        showAlert('Error loading ballrooms. Please try again later. ' + error.message, 'danger');
    }
}

function displayBallrooms(ballrooms) {
    const ballroomList = document.getElementById("ballroom-list");
    ballroomList.innerHTML = "";

    if (ballrooms.length === 0) {
        ballroomList.innerHTML = `
            <div class="col-12 text-center">
                <h3>No ballrooms available</h3>
                <p>Please check back later.</p>
            </div>
        `;
        return;
    }

    ballrooms.forEach(ballroom => {
        console.log('Processing ballroom:', ballroom);
        const imageUrl = getImageUrl(ballroom);
        const div = document.createElement("div");
        div.className = "col-md-6 col-lg-4 mb-4";
        div.innerHTML = `
            <div class="card h-100">
                <img src="${imageUrl}" 
                     class="card-img-top" 
                     alt="${ballroom.name}" 
                     style="height: 200px; object-fit: cover;">
                <div class="card-body">
                    <h5 class="card-title">${ballroom.name}</h5>
                    <p class="card-text">
                        <i class="fas fa-users"></i> Capacity: ${ballroom.capacity} guests<br>
                        <i class="fas fa-ruler-combined"></i> Size: ${ballroom.dimesions || 'Dimensions not available'}
                    </p>
                    <button class="btn btn-primary w-100" onclick="showDetails(${JSON.stringify(ballroom).replace(/"/g, '&quot;')})">
                        View Details
                    </button>
                </div>
            </div>
        `;
        ballroomList.appendChild(div);
    });
}

function formatDimensions(ballroom) {
    console.log('Formatting dimensions for:', ballroom.name, 'Dimensions:', ballroom.dimesions);
    
    // Return the dimesions string directly if available (note the spelling)
    if (typeof ballroom.dimesions === 'string' ) {
        return ballroom.dimesions;
    }
    
    // If we have length and width, format them to match the same format
    if (ballroom.length && ballroom.width) {
        return `${ballroom.length}m x ${ballroom.width}m`;
    }
    
    return ballroom.dimesions;
}

function showDetails(ballroom) {
    selectedBallroom = ballroom;
    
    // Update modal content with fallback image handling
    const modalImage = document.getElementById("modal-image");
    modalImage.src = getImageUrl(ballroom);
    document.getElementById("modal-name").textContent = ballroom.name;
    document.getElementById("modal-description").textContent = ballroom.description || 'No description available';
    document.getElementById("modal-capacity").textContent = `${ballroom.capacity} guests`;
    document.getElementById("modal-size").textContent = formatDimensions(ballroom);

    // Show the modal
    const detailsModal = document.getElementById('detailsModal');
    const bsModal = bootstrap.Modal.getOrCreateInstance(detailsModal);
    bsModal.show();
}

function showBookingForm() {
    if (!selectedBallroom) {
        showAlert('Please select a ballroom first.', 'warning');
        return;
    }

    // Hide details modal
    const detailsModal = document.getElementById('detailsModal');
    const bsDetailsModal = bootstrap.Modal.getInstance(detailsModal);
    if (bsDetailsModal) {
        bsDetailsModal.hide();
    }

    // Set up booking form
    document.getElementById("ballroom-id").value = selectedBallroom.id;
    document.getElementById("guests").max = selectedBallroom.capacity;
    document.getElementById("guests").placeholder = `Max ${selectedBallroom.capacity} guests`;

    // Show booking modal
    const bookingModal = document.getElementById('bookingModal');
    const bsBookingModal = bootstrap.Modal.getOrCreateInstance(bookingModal);
    bsBookingModal.show();
}

async function submitBooking() {
    const form = document.getElementById("booking-form");
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const guestCount = parseInt(document.getElementById("guests").value);
    if (guestCount > selectedBallroom.capacity) {
        showAlert(`Maximum capacity for ${selectedBallroom.name} is ${selectedBallroom.capacity} guests.`, 'warning');
        return;
    }

    const booking = {
        ballroomId: selectedBallroom.id,
        customerName: document.getElementById("name").value,
        customerEmail: document.getElementById("email").value,
        customerPhone: document.getElementById("phone").value,
        eventDate: document.getElementById("event-date").value,
        eventType: document.getElementById("event-type").value,
        guestCount: guestCount,
        specialRequests: document.getElementById("special-requests").value
    };

    try {
        const submitButton = document.querySelector('#bookingModal .btn-primary');
        submitButton.disabled = true;
        submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';

        console.log('Submitting booking:', booking);
        const response = await fetch(`${apiBaseUrl}/booking`, {
            method: "POST",
            headers: { 
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            body: JSON.stringify(booking)
        });

        console.log('Booking response status:', response.status);
        const responseText = await response.text();
        console.log('Booking response:', responseText);

        if (response.ok) {
            const bookingModal = bootstrap.Modal.getInstance(document.getElementById('bookingModal'));
            bookingModal.hide();
            showAlert('Booking request submitted successfully!', 'success');
            form.reset();
            fetchBallrooms(); // Refresh the ballroom list
        } else {
            showAlert(`Error submitting booking: ${responseText}`, 'danger');
        }
    } catch (error) {
        console.error('Error submitting booking:', error);
        showAlert('Error submitting booking. Please try again.', 'danger');
    } finally {
        const submitButton = document.querySelector('#bookingModal .btn-primary');
        submitButton.disabled = false;
        submitButton.innerHTML = 'Confirm Booking';
    }
}

function showAlert(message, type = 'info') {
    const alertsContainer = document.getElementById('alerts-container') || document.createElement('div');
    if (!document.getElementById('alerts-container')) {
        alertsContainer.id = 'alerts-container';
        alertsContainer.style.position = 'fixed';
        alertsContainer.style.top = '20px';
        alertsContainer.style.right = '20px';
        alertsContainer.style.zIndex = '9999';
        document.body.appendChild(alertsContainer);
    }

    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    alertsContainer.appendChild(alertDiv);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv && alertDiv.parentElement) {
            alertDiv.remove();
        }
    }, 5000);
}