// Get the API base URL from config
const config = {
    development: {
        apiUrl: 'http://localhost:5244'
    },
    production: {
        apiUrl: 'https://ballrooms-api.azurewebsites.net'
    }
};

const environment = window.location.hostname.includes('localhost') ? 'development' : 'production';
const apiBaseUrl = config[environment].apiUrl;

const ballroomUrl = `${apiBaseUrl}/api/Ballroom`;
const ballroomsDiv = document.getElementById('ballrooms');
const addForm = document.getElementById('add-ballroom-form');
const alertContainer = document.getElementById('alert-container');

document.addEventListener('DOMContentLoaded', () => {
    fetchBallrooms();
    document.getElementById('add-ballroom-form').addEventListener('submit', handleAddBallroom);
});

async function fetchBallrooms() {
    try {
        console.log('Fetching ballrooms from:', ballroomUrl);
        const response = await fetch(ballroomUrl);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const text = await response.text();
        console.log('Received response:', text);
        
        if (!text) {
            throw new Error('Empty response received');
        }
        
        const ballrooms = JSON.parse(text);
        
        ballroomsDiv.innerHTML = '';
        
        if (ballrooms.length === 0) {
            ballroomsDiv.innerHTML = `
                <div class="col-12">
                    <div class="alert alert-info" role="alert">
                        No ballrooms found. Add your first ballroom using the form above!
                    </div>
                </div>
            `;
            return;
        }

        ballrooms.forEach(ballroom => {
            const ballroomElement = document.createElement('div');
            ballroomElement.className = 'col-md-6 col-lg-4 mb-4';
            ballroomElement.innerHTML = `
                <div class="ballroom-card">
                    <img src="${ballroom.imageUrl || 'https://via.placeholder.com/300x200?text=No+Image'}" 
                         alt="${ballroom.name}" 
                         class="ballroom-image mb-3">
                    <h3 class="h5">${ballroom.name}</h3>
                    <p class="text-muted">Capacity: ${ballroom.capacity} people</p>
                    <p class="text-muted">Dimensions: ${ballroom.dimesions}</p>
                    <p>${ballroom.description || 'No description available'}</p>
                    <div class="d-flex justify-content-between align-items-center">
                        <span class="badge ${ballroom.isAvailable ? 'bg-success' : 'bg-danger'}">
                            ${ballroom.isAvailable ? 'Available' : 'Not Available'}
                        </span>
                        <div class="btn-group">
                            <button class="btn btn-sm btn-outline-primary" onclick="editBallroom(${ballroom.id})">Edit</button>
                            <button class="btn btn-sm btn-outline-danger" onclick="deleteBallroom(${ballroom.id})">Delete</button>
                        </div>
                    </div>
                </div>
                <form id="edit-form-${ballroom.id}" class="edit-form" style="display: none;">
                    <div class="ballroom-card mt-3">
                        <h4 class="h6 mb-3">Edit Ballroom</h4>
                        <div class="mb-3">
                            <label class="form-label">Name:</label>
                            <input type="text" class="form-control" name="name" value="${ballroom.name}" required>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description:</label>
                            <textarea class="form-control" name="description">${ballroom.description || ''}</textarea>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Capacity:</label>
                            <input type="number" class="form-control" name="capacity" value="${ballroom.capacity}" required>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Dimensions:</label>
                            <input type="text" class="form-control" name="dimesions" value="${ballroom.dimesions}" required>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">New Image:</label>
                            <input type="file" class="form-control" name="image" accept="image/*">
                        </div>
                        <div class="mb-3 form-check">
                            <input type="checkbox" class="form-check-input" name="isAvailable" ${ballroom.isAvailable ? 'checked' : ''}>
                            <label class="form-check-label">Available</label>
                        </div>
                        <div class="d-flex gap-2">
                            <button type="submit" class="btn btn-primary">Save Changes</button>
                            <button type="button" class="btn btn-secondary" onclick="cancelEdit(${ballroom.id})">Cancel</button>
                        </div>
                    </div>
                </form>
            `;
            ballroomsDiv.appendChild(ballroomElement);

            const editForm = document.getElementById(`edit-form-${ballroom.id}`);
            editForm.addEventListener('submit', (event) => handleEditSubmit(event, ballroom.id));
        });
    } catch (error) {
        console.error('Error fetching ballrooms:', error);
        ballroomsDiv.innerHTML = `
            <div class="col-12">
                <div class="alert alert-danger" role="alert">
                    Error loading ballrooms. Please try again later. (${error.message})
                </div>
            </div>
        `;
    }
}

async function handleAddBallroom(event) {
    event.preventDefault();

    const formData = new FormData(addForm);
    
    try {
        const response = await fetch(ballroomUrl, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            addForm.reset();
            await fetchBallrooms();
            showAlert('Ballroom added successfully!', 'success');
        } else {
            const errorData = await response.text();
            console.error('Error adding ballroom:', errorData);
            showAlert(`Error adding ballroom: ${errorData || response.statusText}`, 'danger');
        }
    } catch (error) {
        console.error('Error adding ballroom:', error);
        showAlert('Error adding ballroom. Please try again.', 'danger');
    }
}

async function deleteBallroom(id) {
    if (!confirm('Are you sure you want to delete this ballroom?')) {
        return;
    }

    try {
        const response = await fetch(`${ballroomUrl}/${id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await fetchBallrooms();
            showAlert('Ballroom deleted successfully!', 'success');
        } else {
            const errorData = await response.text();
            console.error('Error deleting ballroom:', errorData);
            showAlert(`Error deleting ballroom: ${errorData || response.statusText}`, 'danger');
        }
    } catch (error) {
        console.error('Error deleting ballroom:', error);
        showAlert('Error deleting ballroom. Please try again.', 'danger');
    }
}

function editBallroom(id) {
    const editForm = document.getElementById(`edit-form-${id}`);
    if (editForm) {
        // Hide all other edit forms
        document.querySelectorAll('.edit-form').forEach(form => {
            if (form.id !== `edit-form-${id}`) {
                form.style.display = 'none';
            }
        });
        editForm.style.display = 'block';
    }
}

function cancelEdit(id) {
    const editForm = document.getElementById(`edit-form-${id}`);
    if (editForm) {
        editForm.style.display = 'none';
    }
}

async function handleEditSubmit(event, id) {
    event.preventDefault();
    const form = event.target;
    const formData = new FormData(form);
    formData.set('isAvailable', formData.get('isAvailable') === 'on');
    formData.append('id', id);

    try {
        const response = await fetch(`${ballroomUrl}/${id}`, {
            method: 'PUT',
            body: formData
        });

        if (response.ok) {
            await fetchBallrooms();
            showAlert('Ballroom updated successfully!', 'success');
        } else {
            const errorData = await response.text();
            console.error('Error updating ballroom:', errorData);
            showAlert(`Error updating ballroom: ${errorData || response.statusText}`, 'danger');
        }
    } catch (error) {
        console.error('Error updating ballroom:', error);
        showAlert('Error updating ballroom. Please try again.', 'danger');
    }
}

function showAlert(message, type) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    // Clear existing alerts
    while (alertContainer.firstChild) {
        alertContainer.removeChild(alertContainer.firstChild);
    }

    // Add new alert
    alertContainer.appendChild(alertDiv);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode === alertContainer) {
            alertContainer.removeChild(alertDiv);
        }
    }, 5000);
}

// Make functions globally available
window.editBallroom = editBallroom;
window.deleteBallroom = deleteBallroom;
window.cancelEdit = cancelEdit;

// Initial load
fetchBallrooms();