var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var InvocationExpression = (function (_super) {
            __extends(InvocationExpression, _super);
            function InvocationExpression(meta) {
                        _super.call(this, meta);
            }
            InvocationExpression.prototype._Compile = function () {
                return function (ambientContext) {
                    return null;
                };
            };
            return InvocationExpression;
        })(Expressions.Expression);
        Expressions.InvocationExpression = InvocationExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
