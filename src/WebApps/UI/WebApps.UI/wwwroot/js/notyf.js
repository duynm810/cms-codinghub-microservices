// Initialize notyf
let notyf = new Notyf({
    duration: 5000,
    position: {
        x: 'right',
        y: 'top',
    }
});

// Show error function using Notyf
function showErrorNotification(message) {
    notyf.error(message);
}

// Show success function using Notyf
function showSuccessNotification(message) {
    notyf.success(message);
}