/// <reference path="../../ref/linq.d.ts" />
/// <reference path="../../ref/jquery.d.ts" />
/// <reference path="../../ref/jquery.validation.d.ts" />
/// <reference path="../MetaClasses.ts" />
/// <reference path="../Sanitizer.ts" />

/// <reference path="Expression.ts" />

module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        ///<summary>Special case for binary assignments</summary>
        export class AssignmentExpression extends Expression {

            Left: Expression;
            LeftProperty: string;
            Right: Expression;

            constructor(meta: Meta.BinaryExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });

                switch (meta.Left.ExpressionType) {
                    case __BTF.Meta.ExpressionWrapperType.Parameter:
                        this.Left = null;
                        this.LeftProperty = (<Meta.ParameterExpressionMeta>meta.Left).Name;
                        break;
                    case __BTF.Meta.ExpressionWrapperType.Member:
                        this.Left = __BTF.Expressions.Expression.CreateExpression((<Meta.MemberExpressionMeta>meta.Left).Expression);
                        this.LeftProperty = (<Meta.MemberExpressionMeta>meta.Left).MemberName;
                        break;
                    default:
                        throw "The left hand side of an assignment must be a parameter or a member";
                }

                this.Right = __BTF.Expressions.Expression.CreateExpression(meta.Right);
            };

            _Compile(): Validation.ExpressionInvokerAction {
                var left = this.Left ? this.Left.Compile() : (context) => context;
                var right = this.Right.Compile();
                return (ambientContext) => left(ambientContext)[this.LeftProperty] = right(ambientContext);
            };
        }
    }
}