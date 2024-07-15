//#region INITIALIZE

let notyfConfig = new Notyf({
    duration: 5000,
    position: {
        x: 'right',
        y: 'top',
    }
});

//#endregion

//#region ERROR

function showErrorNotification(message) {
    notyfConfig.error(message);
}

//#endregion

//#region SUCCESS

function showSuccessNotification(message) {
    notyfConfig.success(message);
}

//#endregion