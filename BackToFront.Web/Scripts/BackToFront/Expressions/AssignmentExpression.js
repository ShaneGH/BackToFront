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
        var AssignmentExpression = (function (_super) {
            __extends(AssignmentExpression, _super);
            function AssignmentExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });
                switch(meta.Left.ExpressionType) {
                    case __BTF.Meta.ExpressionWrapperType.Parameter:
                        this.Left = null;
                        this.LeftProperty = (meta.Left).Name;
                        break;
                    case __BTF.Meta.ExpressionWrapperType.Member:
                        this.Left = __BTF.Expressions.Expression.CreateExpression((meta.Left).Expression);
                        this.LeftProperty = (meta.Left).MemberName;
                        break;
                    default:
                        throw "The left hand side of an assignment must be a parameter or a member";
                }
                this.Right = __BTF.Expressions.Expression.CreateExpression(meta.Right);
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
        })(Expressions.Expression);
        Expressions.AssignmentExpression = AssignmentExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
