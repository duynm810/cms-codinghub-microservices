
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