﻿// Fetch the parking data from the backend
async function fetchParkingData() {
    try {
        const response = await fetch('http://localhost:5000/list');
        const parkingData = await response.json();
        
        displayParkingData(parkingData);
    } catch (error) {
        console.error('Error fetching parking data:', error);
    }
}

// Convert milliseconds to a human-readable time format (hours, minutes, seconds)
function formatTime(milliseconds) {
    const totalSeconds = Math.floor(milliseconds / 1000);
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;

    return `${hours} hours, ${minutes} minutes, ${seconds} seconds`;
}

// Display parking data in the table
function displayParkingData(data) {
    const tableBody = document.querySelector('#parkingTable tbody');
    tableBody.innerHTML = ''; // Clear existing rows

    data.forEach((space, index) => {
        const row = document.createElement('tr');
        row.appendChild(createCell(index + 1)); // Space Number
        row.appendChild(createStatusCell(space.IsTaken)); // Space Status
        row.appendChild(createVehicleCell(space)); // Vehicle Info
        row.appendChild(createBikesCell(space.Bikes)); // Bikes Info
        row.appendChild(createTimeSpanCell(space)); // Time Span
        tableBody.appendChild(row);
    });
}

// Helper function to create table cells
function createCell(content) {
    const cell = document.createElement('td');
    cell.textContent = content;
    return cell;
}

// Create cell for space status (Occupied or Available)
function createStatusCell(isTaken) {
    const cell = document.createElement('td');
    cell.textContent = isTaken ? 'Occupied' : 'Available';
    cell.classList.add(isTaken ? 'occupied' : 'available');
    return cell;
}

// Create cell for vehicle information
function createVehicleCell(space) {
    const cell = document.createElement('td');
    if (space.Vehicle) {
        cell.innerHTML = `
            <div class="vehicle">
                <h2>${space.VehicleType}</h2>
                <strong>Plate:</strong> ${space.Vehicle.LicensePlate}<br>
                <strong>Color:</strong> ${space.Vehicle.Color}<br>
                <strong>Time:</strong> ${formatTime(space.Vehicle.sw.ElapsedMilliseconds)}
            </div>
        `;
    } else {
        cell.textContent = 'No vehicle';
    }
    return cell;
}

// Create cell for bikes information
function createBikesCell(bikes) {
    const cell = document.createElement('td');
    if (bikes && bikes.length > 0) {
        bikes.forEach(bike => {
            if (bike) {
                cell.innerHTML += `
                    <div class="bike">
                        <strong>Make:</strong> ${bike.Make}<br>
                        <strong>Plate:</strong> ${bike.LicensePlate}<br>
                        <strong>Color:</strong> ${bike.Color}<br>
                        <strong>Time:</strong> ${formatTime(bike.sw.ElapsedMilliseconds)}
                    </div>
                `;
            } else {
                cell.innerHTML += '<div class="bike">No bike</div>';
            }
        });
    } else {
        cell.textContent = 'No bikes';
    }
    return cell;
}

// Create cell for time span, showing elapsed time for vehicle and bikes
function createTimeSpanCell(space) {
    const cell = document.createElement('td');
    if (space.Vehicle) {
        cell.innerHTML = `
            <span class="time-span">${formatTime(space.Vehicle.sw.ElapsedMilliseconds)}</span><br>
            <span class="time-span">Total: ${space.Vehicle.Total}</span>
        `;
        console.log(space.Vehicle.Total)
    } else if (space.Bikes && space.Bikes.length > 0) {
        space.Bikes.forEach(bike => {
            if (bike) {
                cell.innerHTML += `
                    <span class="time-span">${bike.LicensePlate}: ${formatTime(bike.sw.ElapsedMilliseconds)}</span><br>
                    <span class="time-span">Total: ${bike.Total}</span><br>
                `;
                console.log(bike.Total)
            }
        });
    } else {
        cell.textContent = 'N/A';
    }
    return cell;
}

// Initialize the page by fetching the data and setting up periodic refresh
fetchParkingData(); // Initial load

// Set up periodic refresh every 5 seconds (adjust time as needed)
const refreshInterval = setInterval(fetchParkingData, 1000);

// Clear the interval when the page is unloaded to prevent memory leaks
window.addEventListener('beforeunload', () => clearInterval(refreshInterval));
