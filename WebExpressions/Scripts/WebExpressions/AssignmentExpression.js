var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var AssignmentExpression = (function (_super) {
        __extends(AssignmentExpression, _super);
        function AssignmentExpression(meta) {
                _super.call(this, meta);
            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Left",
                inputType: "object"
            }, {
                inputName: "Right",
                inputType: "object"
            });
            switch(meta.Left.ExpressionType) {
                case WebExpressions.Meta.ExpressionWrapperType.Parameter:
                    this.Left = null;
                    this.LeftProperty = (meta.Left).Name;
                    break;
                case WebExpressions.Meta.ExpressionWrapperType.Member:
                    this.Left = WebExpressions.Expression.CreateExpression((meta.Left).Expression);
                    this.LeftProperty = (meta.Left).MemberName;
                    break;
                default:
                    throw "The left hand side of an assignment must be a parameter or a member";
            }
            this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
        }
        AssignmentExpression.prototype.EvalExpression = function () {
            var right = this.Right.EvalExpression();
            var left = this.Left ? this.Left.EvalExpression() : null;
            if(left) {
                right.Constants.Merge(left.Constants);
            }
            return {
                Expression: "(" + (left ? left.Expression + "." : "") + this.LeftProperty + " = " + right.Expression + ")",
                Constants: right.Constants
            };
        };
        AssignmentExpression.prototype._Compile = function () {
            var _this = this;
            var left = this.Left ? this.Left.Compile() : function (context) {
                return context;
            };
            var right = this.Right.Compile();
            return function (ambientContext) {
                return left(ambientContext)[_this.LeftProperty] = right(ambientContext);
            };
        };
        return AssignmentExpression;
    })(WebExpressions.Expression);
    WebExpressions.AssignmentExpression = AssignmentExpression;    
})(WebExpressions || (WebExpressions = {}));
