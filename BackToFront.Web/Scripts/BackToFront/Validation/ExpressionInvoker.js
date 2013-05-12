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
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));
