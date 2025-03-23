import { apiBaseUrl } from './config.js';

const apiUrl = `${apiBaseUrl}/ballrooms`;

const ballroomsDiv = document.getElementById('ballrooms');
const addForm = document.getElementById('add-ballroom-form');

async function fetchBallrooms() {
    try {
        const response = await fetch(apiUrl);
        const ballrooms = await response.json();

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
                    Error loading ballrooms. Please try again later.
                </div>
            </div>
        `;
    }
}

addForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const formData = new FormData(addForm);
    formData.set('isAvailable', formData.get('isAvailable') === 'on');

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            addForm.reset();
            await fetchBallrooms();
            showAlert('success', 'Ballroom added successfully!');
        } else {
            const errorData = await response.text();
            console.error('Error adding ballroom:', errorData);
            showAlert('danger', `Error adding ballroom: ${errorData || response.statusText}`);
        }
    } catch (error) {
        console.error('Error adding ballroom:', error);
        showAlert('danger', 'Error adding ballroom. Please try again.');
    }
});

async function deleteBallroom(id) {
    if (!confirm('Are you sure you want to delete this ballroom?')) {
        return;
    }

    try {
        const response = await fetch(`${apiUrl}/${id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await fetchBallrooms();
            showAlert('success', 'Ballroom deleted successfully!');
        } else {
            const errorData = await response.text();
            console.error('Error deleting ballroom:', errorData);
            showAlert('danger', `Error deleting ballroom: ${errorData || response.statusText}`);
        }
    } catch (error) {
        console.error('Error deleting ballroom:', error);
        showAlert('danger', 'Error deleting ballroom. Please try again.');
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
        const response = await fetch(`${apiUrl}/${id}`, {
            method: 'PUT',
            body: formData
        });

        if (response.ok) {
            await fetchBallrooms();
            showAlert('success', 'Ballroom updated successfully!');
        } else {
            const errorData = await response.text();
            console.error('Error updating ballroom:', errorData);
            showAlert('danger', `Error updating ballroom: ${errorData || response.statusText}`);
        }
    } catch (error) {
        console.error('Error updating ballroom:', error);
        showAlert('danger', 'Error updating ballroom. Please try again.');
    }
}

function showAlert(type, message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    document.querySelector('.container').insertBefore(alertDiv, document.querySelector('.form-container'));

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        alertDiv.remove();
    }, 5000);
}

// Initial load
fetchBallrooms();