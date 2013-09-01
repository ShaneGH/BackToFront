/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var UnaryExpression = (function (_super) {
        __extends(UnaryExpression, _super);
        function UnaryExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Operand",
                inputType: "object"
            });

            this.Operand = WebExpressions.Expression.CreateExpression(meta.Operand);
        }
        //EvalExpression(): CreateEvalExpression {
        //    var operand = this.Operand.EvalExpression();
        //    return {
        //        Expression: "(" + UnaryExpression.OperatorStringDictionary[this.NodeType](operand.Expression) + ")",
        //        Constants: operand.Constants
        //    };
        //}
        UnaryExpression.prototype._Compile = function () {
            var _this = this;
            var operand = this.Operand.Compile();
            return function (ambientContext) {
                return UnaryExpression.OperatorDictionary[_this.NodeType](operand(ambientContext));
            };
        };
        UnaryExpression.OperatorDictionary = (function () {
            var output = [];

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Convert] = function (operand) {
                return operand;
            };
            output[WebExpressions.Meta.ExpressionType.Not] = function (operand) {
                return !operand;
            };

            return output;
        })();

        UnaryExpression.OperatorStringDictionary = (function () {
            var output = [];

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Convert] = function (operand) {
                return operand;
            };
            output[WebExpressions.Meta.ExpressionType.Not] = function (operand) {
                return "!" + operand;
            };

            return output;
        })();
        return UnaryExpression;
    })(WebExpressions.Expression);
    WebExpressions.UnaryExpression = UnaryExpression;
})(WebExpressions || (WebExpressions = {}));
