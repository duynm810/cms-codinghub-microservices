// Initialize notyfConfig
let notyfConfig = new Notyf({
    duration: 5000,
    position: {
        x: 'right',
        y: 'top',
    }
});

// Show error function using Notyf
function showErrorNotification(message) {
    notyfConfig.error(message);
}

// Show success function using Notyf
function showSuccessNotification(message) {
    notyfConfig.success(message);
}