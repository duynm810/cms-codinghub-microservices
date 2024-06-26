function confirmLogout(url) {
    Swal.fire({
        title: 'Are you sure you want to logout?',
        text: "You will be logged out from your account.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, logout',
        cancelButtonText: 'Cancel',
        customClass: {
            popup: 'small-swal-popup'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}

function showConfirmAlert(title, text, confirmButtonText, cancelButtonText, onConfirm) {
    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmButtonText,
        cancelButtonText: cancelButtonText,
        customClass: {
            popup: 'small-swal-popup'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            onConfirm();
        }
    });
}