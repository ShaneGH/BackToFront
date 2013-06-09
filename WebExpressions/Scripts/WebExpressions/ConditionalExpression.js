var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var ConditionalExpression = (function (_super) {
        __extends(ConditionalExpression, _super);
        function ConditionalExpression(meta) {
                _super.call(this, meta);
            WebExpressions.Sanitizer.Require(meta, {
                inputName: "IfTrue",
                inputType: "object"
            }, {
                inputName: "IfFalse",
                inputType: "object"
            }, {
                inputName: "Test",
                inputType: "object"
            });
            this.IfTrue = WebExpressions.Expression.CreateExpression(meta.IfTrue);
            this.IfFalse = WebExpressions.Expression.CreateExpression(meta.IfFalse);
            this.Test = WebExpressions.Expression.CreateExpression(meta.Test);
        }
        ConditionalExpression.prototype.ToString = function () {
            if(this.IfTrue.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block || this.IfFalse.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block) {
                return this._ToBlockString();
            }
            return this._ToInlineString();
        };
        ConditionalExpression.prototype._ToInlineString = function () {
            return this.Test.ToString() + " ? " + this.IfTrue.ToString() + " : " + this.IfFalse.ToString();
        };
        ConditionalExpression.prototype._ToBlockString = function () {
            return "if(" + this.Test.ToString() + ") { " + this.IfTrue.ToString() + " } else { " + this.IfFalse.ToString() + " }";
        };
        ConditionalExpression.prototype._Compile = function () {
            var test = this.Test.Compile();
            var ifTrue = this.IfTrue.Compile();
            var ifFalse = this.IfFalse.Compile();
            return function (ambientContext) {
                return test(ambientContext) ? ifTrue(ambientContext) : ifFalse(ambientContext);
            };
        };
        return ConditionalExpression;
    })(WebExpressions.Expression);
    WebExpressions.ConditionalExpression = ConditionalExpression;    
})(WebExpressions || (WebExpressions = {}));
