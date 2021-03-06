/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    ///<summary>Special case for binary assignments</summary>
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

            switch (meta.Left.ExpressionType) {
                case WebExpressions.Meta.ExpressionWrapperType.Parameter:
                    this.Left = null;
                    this.LeftProperty = (meta.Left).Name;
                    break;
                case WebExpressions.Meta.ExpressionWrapperType.StaticMember:
                case WebExpressions.Meta.ExpressionWrapperType.Member:
                    this.Left = WebExpressions.Expression.CreateExpression((meta.Left).Expression);
                    this.LeftProperty = (meta.Left).MemberName;
                    break;
                default:
                    throw "The left hand side of an assignment must be a parameter or a member";
            }

            this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
        }
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
