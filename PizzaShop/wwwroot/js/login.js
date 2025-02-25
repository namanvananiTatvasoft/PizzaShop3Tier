// input validation

document.getElementById('Login-Form').addEventListener('submit', function(event) {
    var emailInput = document.getElementById('LoginEmail');
    var passwordInput = document.getElementById('LoginPassword');
    var emailErrorMsg = document.getElementById('email-error-msg');
    var passwordErrorMsg = document.getElementById('password-error-msg');
    
    var isValid = true;

    if (emailInput.value.trim() === '') {
        emailErrorMsg.textContent = 'Email is required.';
        isValid = false;
    } else {
        emailErrorMsg.textContent = '';
    }

    if (passwordInput.value.trim() === '') {
        passwordErrorMsg.textContent = 'Password is required.';
        isValid = false;
    } else {
        passwordErrorMsg.textContent = '';
    }

    if (isValid) {
        console.log("Hi");
    }else{
        event.preventDefault();
    }
    
});

document.getElementById('LoginEmail').addEventListener('input', function() {
    var emailErrorMsg = document.getElementById('email-error-msg');
    emailErrorMsg.textContent = '';
});

document.getElementById('LoginPassword').addEventListener('input', function() {
    var emailPswMsg = document.getElementById('password-error-msg');
    emailPswMsg.textContent = '';
});



function showPassword() {
    var x = document.getElementById("LoginPassword");
    var eye = document.getElementById("eye");
    if (x.type === "password") {
      x.type = "text";
      eye.classList.remove("bi-eye-slash");
      eye.classList.add("bi-eye");
    } else {
      x.type = "password";
      eye.classList.remove("bi-eye");
      eye.classList.add("bi-eye-slash");
    }
}

