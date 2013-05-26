var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var ConstantExpression = (function (_super) {
            __extends(ConstantExpression, _super);
            function ConstantExpression(meta) {
                        _super.call(this, meta);
                this.Value = meta.Value;
            }
            ConstantExpression.prototype._Compile = function () {
                var _this = this;
                return function (ambientContext) {
                    return _this.Value;
                };
            };
            return ConstantExpression;
        })(Expressions.Expression);
        Expressions.ConstantExpression = ConstantExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
