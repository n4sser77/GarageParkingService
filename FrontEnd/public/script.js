

//SUBMITTING LICENSEPLATE
async function handleFormSubmit(event) {
    event.preventDefault(); // Prevent the form from submitting the default way

    const form = event.target;
    const licensePlate = document.getElementById("licensePlate").value;

    // Send the form data as a POST request
    const response = await fetch(form.action, {
        method: "POST",
        headers: {
            "Content-Type": "text/plain" // Send the license plate as plain text
        },
        body: licensePlate
    });

    // Get the response message
    const message = await response.text();
    alert(message); // Display message in an alert popup
}


// Fetch the parking data from the backend
async function fetchParkingData() {
    try {
        const response = await fetch('http://localhost:5000/api/list');
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
            <span class="time-span">Total: ${space.Vehicle.Total} SEK</span>
        `;
        //console.log(space.Vehicle.Total)
    } else if (space.Bikes && space.Bikes.length > 0) {
        space.Bikes.forEach(bike => {
            if (bike) {
                cell.innerHTML += `
                    <span class="time-span">${bike.LicensePlate}: ${formatTime(bike.sw.ElapsedMilliseconds)}</span><br>
                    <span class="time-span">Total: ${bike.Total} SEK</span><br>
                `;
                // console.log(bike.Total)
            }
        });
    } else {
        cell.textContent = 'N/A';
    }
    return cell;
}

async function isGarageFull() {
    try {
        const response = await fetch('http://localhost:5000/api/isfull');
        const data = await response.json();  // assuming the response is a boolean
        const element = document.getElementById("FREE");

        console.log(data);  // Check the response in the console

        if (data) {
            element.textContent = "Garage is full";  // Update text content
        } else {
            element.textContent = "Garage is not full";  // Update text content
        }

        // Force a reflow/repaint
        element.offsetHeight;  // Accessing this property forces a reflow
    } catch (error) {
        console.error('Error fetching parking data:', error);
    }
}

async function GetTotalSales() {
    try {
        const res = await fetch('http://localhost:5000/api/total-sales');
        const data = await res.json();
        const element = document.getElementById("sales");

        element.textContent += (data + " SEK");
        
            console.log(data);
    } catch (error) {
        console.error('Error fetching parking total sales:', error);
    }
}


// Initialize the page by fetching the data and setting up periodic refresh
fetchParkingData(); // Initial load

isGarageFull();

GetTotalSales();

// Set up periodic refresh every 5 seconds (adjust time as needed)
const refreshInterval = setInterval(fetchParkingData, 1000);

// Clear the interval when the page is unloaded to prevent memory leaks
window.addEventListener('beforeunload', () => clearInterval(refreshInterval));

