// Initialize notyf
let notyf = new Notyf({
    duration: 5000,
    position: {
        x: 'right',
        y: 'top',
    }
});

// Show error function using Notyf
function showError(message) {
    notyf.error(message);
}

// Show success function using Notyf
function showSuccess(message) {
    notyf.success(message);
}