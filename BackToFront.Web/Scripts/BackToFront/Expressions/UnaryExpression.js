var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var UnaryExpression = (function (_super) {
            __extends(UnaryExpression, _super);
            function UnaryExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Operand",
                    inputType: "object"
                });
                this.Operand = Expressions.Expression.CreateExpression(meta.Operand);
            }
            UnaryExpression.OperatorDictionary = (function () {
                var output = [];
                output[__BTF.Meta.ExpressionType.Convert] = function (operand) {
                    return operand;
                };
                output[__BTF.Meta.ExpressionType.Not] = function (operand) {
                    return !operand;
                };
                return output;
            })();
            UnaryExpression.prototype._Compile = function () {
                var _this = this;
                var operand = this.Operand.Compile();
                return function (ambientContext) {
                    return UnaryExpression.OperatorDictionary[_this.NodeType](operand(ambientContext));
                };
            };
            return UnaryExpression;
        })(Expressions.Expression);
        Expressions.UnaryExpression = UnaryExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
