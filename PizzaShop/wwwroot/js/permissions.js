var mainCheck = document.querySelector('.outer-check');
var innerCheck = document.querySelectorAll('.inner-check');

mainCheck.addEventListener('change', function () {
    if(this.checked){
        innerCheck.forEach(function (ie){
            ie.checked = true;
        });
    }else{
        innerCheck.forEach(function (ie){
            ie.checked = false;
        });
    }

    innerCheck.forEach(function (ic) {
        var row = $(ic).closest("tr");  // Use `ic` here, not `this`

        // If isEnable is unchecked, uncheck other checkboxes
        if (!$(ic).prop("checked")) {
            row.find(".can-view input, .can-add-edit input, .can-delete input").prop("checked", false);
        }

        // If any checkbox is checked, check the `mainCheck` box
        // if ($(ic).prop("checked") && !mainCheck.checked) {
        //     mainCheck.checked = true;
        // }
    });
});

innerCheck.forEach(function (ic) {
    ic.addEventListener('change', function() {
        if (!this.checked) {
            mainCheck.checked = false;
        }else{
            var mc = true;
            innerCheck.forEach(function(ie) {
                if(!ie.checked){
                    mc = false;
                }
            });
            
            if(mc){
                mainCheck.checked = true;
            }
        }
    })
})


$(".inner-check").change(function () {
    // Get the row this checkbox belongs to
    var row = $(this).closest("tr");

    // If isEnable is unchecked, uncheck other checkboxes
    if (!$(this).prop("checked")) {
        row.find(".can-view input, .can-add-edit input, .can-delete input").prop("checked", false);
    }
});

$(".form-check-input").change(function () {
    // Get the row this checkbox belongs to
    var row = $(this).closest("tr");

    // If isEnable is unchecked and one of the others is checked, check isEnable
    if (row.find(".inner-check").prop("checked") === false) {
        if (row.find(".can-view input:checked, .can-add-edit input:checked, .can-delete input:checked").length > 0) {
            row.find(".inner-check").prop("checked", true);
        }
    }

    var eo = true;

    innerCheck.forEach(function (ic) {
        if (!ic.checked) {
            eo = false;
            console.log("Hi");
        }
    });

    if (eo) {
        mainCheck.checked = true;
    }


});

document.addEventListener('DOMContentLoaded', function () {
    var mainCheck = document.querySelector('.outer-check');
    var innerCheck = document.querySelectorAll('.inner-check');
    var eo = true;

    innerCheck.forEach(function (ic) {
        if (!ic.checked) {
            eo = false;
            console.log("Hi");
        }
    });

    if (eo) {
        mainCheck.checked = true;
    }
});
