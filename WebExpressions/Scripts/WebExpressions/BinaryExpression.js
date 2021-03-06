/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var Meta = WebExpressions.Meta;

    var BinaryExpression = (function (_super) {
        __extends(BinaryExpression, _super);
        function BinaryExpression(meta) {
            _super.call(this, meta);

            if (!BinaryExpression.OperatorDictionary[this.NodeType])
                throw "##" + "Invalid Operator";

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Left",
                inputType: "object"
            }, {
                inputName: "Right",
                inputType: "object"
            });

            this.Left = WebExpressions.Expression.CreateExpression(meta.Left);
            this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
        }
        BinaryExpression.prototype._Compile = function () {
            var _this = this;
            if (!BinaryExpression.OperatorStringDictionary[this.NodeType]) {
                throw "Invalid expression type";
            }

            var left = this.Left.Compile();
            var right = this.Right.Compile();
            return function (ambientContext) {
                return BinaryExpression.OperatorDictionary[_this.NodeType](left(ambientContext), right(ambientContext));
            };
        };
        BinaryExpression.OperatorDictionary = (function () {
            var output = [];

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Add] = function (left, right) {
                return left + right;
            };
            output[WebExpressions.Meta.ExpressionType.AndAlso] = function (left, right) {
                return left && right;
            };
            output[WebExpressions.Meta.ExpressionType.Divide] = function (left, right) {
                return left / right;
            };

            //TODO: is this the right equals?
            output[WebExpressions.Meta.ExpressionType.Equal] = function (left, right) {
                return left === right;
            };
            output[WebExpressions.Meta.ExpressionType.GreaterThan] = function (left, right) {
                return left > right;
            };
            output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = function (left, right) {
                return left >= right;
            };
            output[WebExpressions.Meta.ExpressionType.LessThan] = function (left, right) {
                return left < right;
            };
            output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = function (left, right) {
                return left <= right;
            };
            output[WebExpressions.Meta.ExpressionType.Multiply] = function (left, right) {
                return left * right;
            };
            output[WebExpressions.Meta.ExpressionType.NotEqual] = function (left, right) {
                return left !== right;
            };
            output[WebExpressions.Meta.ExpressionType.OrElse] = function (left, right) {
                return left || right;
            };
            output[WebExpressions.Meta.ExpressionType.Subtract] = function (left, right) {
                return left - right;
            };

            return output;
        })();

        BinaryExpression.OperatorStringDictionary = (function () {
            var output = [];

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Add] = " + ";
            output[WebExpressions.Meta.ExpressionType.AndAlso] = " && ";
            output[WebExpressions.Meta.ExpressionType.Divide] = " / ";

            //TODO: is this the right equals?
            output[WebExpressions.Meta.ExpressionType.Equal] = " === ";
            output[WebExpressions.Meta.ExpressionType.GreaterThan] = " > ";
            output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = " >= ";
            output[WebExpressions.Meta.ExpressionType.LessThan] = " < ";
            output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = " <= ";
            output[WebExpressions.Meta.ExpressionType.Multiply] = " * ";
            output[WebExpressions.Meta.ExpressionType.NotEqual] = " !== ";
            output[WebExpressions.Meta.ExpressionType.OrElse] = " || ";
            output[WebExpressions.Meta.ExpressionType.Subtract] = " - ";

            return output;
        })();
        return BinaryExpression;
    })(WebExpressions.Expression);
    WebExpressions.BinaryExpression = BinaryExpression;
})(WebExpressions || (WebExpressions = {}));
