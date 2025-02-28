var mainCheck = document.querySelector('.outer-check');
var innerCheck = document.querySelectorAll('.inner-check');

mainCheck.addEventListener('click', function () {
    if(this.checked){
        innerCheck.forEach(function (ie){
            ie.checked = true;
        });
    }else{
        innerCheck.forEach(function (ie){
            ie.checked = false;
        });
    }
});

innerCheck.forEach(function (ic) {
    ic.addEventListener('click', function() {
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
    