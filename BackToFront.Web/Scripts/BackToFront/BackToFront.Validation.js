var __BTF;
(function (__BTF) {
    (function (Validation) {
        var ExpressionInvoker = (function () {
            function ExpressionInvoker(Logic, AffectedProperties) {
                this.Logic = Logic;
                this.AffectedProperties = AffectedProperties;
            }
            return ExpressionInvoker;
        })();
        Validation.ExpressionInvoker = ExpressionInvoker;        
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
