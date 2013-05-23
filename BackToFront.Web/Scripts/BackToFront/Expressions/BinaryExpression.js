var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    var Validation = __BTF.Validation;
    var Meta = __BTF.Meta;
    (function (Expressions) {
        var BinaryExpression = (function (_super) {
            __extends(BinaryExpression, _super);
            function BinaryExpression(meta) {
                        _super.call(this, meta);
                if(!BinaryExpression.OperatorDictionary[this.NodeType]) {
                    throw "##" + "Invalid Operator";
                }
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });
                this.Left = Expressions.Expression.CreateExpression(meta.Left);
                this.Right = Expressions.Expression.CreateExpression(meta.Right);
            }
            BinaryExpression.OperatorDictionary = (function () {
                var output = [];
                output[__BTF.Meta.ExpressionType.Add] = function (left, right) {
                    return left + right;
                };
                output[__BTF.Meta.ExpressionType.AndAlso] = function (left, right) {
                    return left && right;
                };
                output[__BTF.Meta.ExpressionType.Divide] = function (left, right) {
                    return left / right;
                };
                output[__BTF.Meta.ExpressionType.GreaterThan] = function (left, right) {
                    return left > right;
                };
                output[__BTF.Meta.ExpressionType.GreaterThanOrEqual] = function (left, right) {
                    return left >= right;
                };
                output[__BTF.Meta.ExpressionType.LessThan] = function (left, right) {
                    return left < right;
                };
                output[__BTF.Meta.ExpressionType.LessThanOrEqual] = function (left, right) {
                    return left <= right;
                };
                output[__BTF.Meta.ExpressionType.Multiply] = function (left, right) {
                    return left * right;
                };
                output[__BTF.Meta.ExpressionType.OrElse] = function (left, right) {
                    return left || right;
                };
                output[__BTF.Meta.ExpressionType.Subtract] = function (left, right) {
                    return left - right;
                };
                return output;
            })();
            BinaryExpression.prototype._Compile = function () {
                var _this = this;
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return function (namedArguments, context) {
                    return BinaryExpression.OperatorDictionary[_this.NodeType](left(namedArguments, context), right(namedArguments, context));
                };
            };
            return BinaryExpression;
        })(Expressions.Expression);
        Expressions.BinaryExpression = BinaryExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
