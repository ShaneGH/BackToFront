var __extends = this.__extends || function (d, b) {
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
        UnaryExpression.OperatorDictionary = (function () {
            var output = [];
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
            output[WebExpressions.Meta.ExpressionType.Convert] = function (operand) {
                return operand;
            };
            output[WebExpressions.Meta.ExpressionType.Not] = function (operand) {
                return "!" + operand;
            };
            return output;
        })();
        UnaryExpression.prototype.EvalExpression = function () {
            var operand = this.Operand.EvalExpression();
            return {
                Expression: "(" + UnaryExpression.OperatorStringDictionary[this.NodeType](operand.Expression) + ")",
                Constants: operand.Constants
            };
        };
        UnaryExpression.prototype._Compile = function () {
            var _this = this;
            var operand = this.Operand.Compile();
            return function (ambientContext) {
                return UnaryExpression.OperatorDictionary[_this.NodeType](operand(ambientContext));
            };
        };
        return UnaryExpression;
    })(WebExpressions.Expression);
    WebExpressions.UnaryExpression = UnaryExpression;    
})(WebExpressions || (WebExpressions = {}));
