var __BTF;
(function (__BTF) {
    (function (Validation) {
        var ValidationContext = (function () {
            function ValidationContext() { }
            ValidationContext.prototype.Break = function () {
                return false;
            };
            return ValidationContext;
        })();
        Validation.ValidationContext = ValidationContext;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));
