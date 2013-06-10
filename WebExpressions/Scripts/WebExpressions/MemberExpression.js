var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var MemberExpression = (function (_super) {
        __extends(MemberExpression, _super);
        function MemberExpression(meta) {
                _super.call(this, meta);
            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expression",
                inputType: "object"
            }, {
                inputName: "MemberName",
                inputConstructor: String
            });
            this.Expression = WebExpressions.Expression.CreateExpression(meta.Expression);
            this.MemberName = meta.MemberName;
        }
        MemberExpression.PropertyRegex = new RegExp("^[a-zA-Z][a-zA-Z0-9]*$");
        MemberExpression.prototype.EvalExpression = function () {
            if(!MemberExpression.PropertyRegex.test(this.MemberName)) {
                throw "Invalid property name: " + this.MemberName;
            }
            var expression = this.Expression.EvalExpression();
            return {
                Expression: expression.Expression + "." + this.MemberName,
                Constants: expression.Constants
            };
        };
        MemberExpression.prototype._Compile = function () {
            if(!MemberExpression.PropertyRegex.test(this.MemberName)) {
                throw "Invalid property name: " + this.MemberName;
            }
            var name = this.MemberName;
            var expression = this.Expression.Compile();
            return function (ambientContext) {
                return expression(ambientContext)[name];
            };
        };
        return MemberExpression;
    })(WebExpressions.Expression);
    WebExpressions.MemberExpression = MemberExpression;    
})(WebExpressions || (WebExpressions = {}));
