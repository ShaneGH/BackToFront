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
        ConditionalExpression.prototype.EvalExpression = function () {
            if(this.IfTrue.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block || this.IfFalse.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block) {
                return this._ToBlockString();
            }
            return this._ToInlineString();
        };
        ConditionalExpression.prototype._ToInlineString = function () {
            var test = this.Test.EvalExpression();
            var ifTrue = this.IfTrue.EvalExpression();
            var ifFalse = this.IfFalse.EvalExpression();
            return {
                Constants: test.Constants.Merge(ifTrue.Constants).Merge(ifFalse.Constants),
                Expression: test.Expression + " ? " + ifTrue.Expression + " : " + ifFalse.Expression
            };
        };
        ConditionalExpression.prototype._ToBlockString = function () {
            var test = this.Test.EvalExpression();
            var ifTrue = this.IfTrue.EvalExpression();
            var ifFalse = this.IfFalse.EvalExpression();
            return {
                Constants: test.Constants.Merge(ifTrue.Constants).Merge(ifFalse.Constants),
                Expression: "if(" + test.Expression + ") { " + ifTrue.Expression + " } else { " + ifFalse.Expression + " }"
            };
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
