
/// <reference path="Expression.ts" />

module WebExpressions {

    ///<summary>Special case for binary assignments</summary>
    export class AssignmentExpression extends Expression {

        Left: Expression;
        LeftProperty: string;
        Right: Expression;

        constructor(meta: Meta.BinaryExpressionMeta) {
            super(meta);

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
                    this.LeftProperty = (<Meta.ParameterExpressionMeta>meta.Left).Name;
                    break;
                case WebExpressions.Meta.ExpressionWrapperType.Member:
                    this.Left = WebExpressions.Expression.CreateExpression((<Meta.MemberExpressionMeta>meta.Left).Expression);
                    this.LeftProperty = (<Meta.MemberExpressionMeta>meta.Left).MemberName;
                    break;
                default:
                    throw "The left hand side of an assignment must be a parameter or a member";
            }

            this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
        };

        ToString(): string {
            // TODO: replace . with [] and watch for injection
            return (this.Left ? this.Left.ToString() + "." : "") + this.LeftProperty + " = " + this.Right.ToString();
        }

        _Compile(): ExpressionInvokerAction {
            var left = this.Left ? this.Left.Compile() : (context) => context;
            var right = this.Right.Compile();
            return (ambientContext) => left(ambientContext)[this.LeftProperty] = right(ambientContext);
        };
    }
}
