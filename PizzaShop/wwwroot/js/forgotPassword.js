// input validation

document.getElementById('Forgot-Form').addEventListener('submit', function(event) {
    var emailInput = document.getElementById('ForgotEmail');
    var emailErrorMsg = document.getElementById('email-error-msg');
    
    var isValid = true;

    if (emailInput.value.trim() === '') {
        emailErrorMsg.textContent = 'Valid Email is required.';
        isValid = false;
    } else {
        emailErrorMsg.textContent = '';
    }

    if (!isValid) {
        event.preventDefault();
    }
});