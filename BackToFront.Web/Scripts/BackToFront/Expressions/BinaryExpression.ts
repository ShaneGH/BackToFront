/// <reference path="../../ref/linq.d.ts" />
/// <reference path="../../ref/jquery.d.ts" />
/// <reference path="../../ref/jquery.validation.d.ts" />
/// <reference path="../MetaClasses.ts" />
/// <reference path="../Sanitizer.ts" />

/// <reference path="Expression.ts" />
/// <reference path="../Validation/ValidationContext.ts" />

module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        export class BinaryExpression extends Expression {
            Left: Expression;
            Right: Expression;

            private static OperatorDictionary: { (left, right): any; }[] = [];

            constructor(meta: Meta.BinaryExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });

                if (!BinaryExpression.OperatorDictionary[this.NodeType])
                    throw "Invalid Operator";

                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            }

            _Compile(): Validation.ExpressionInvokerAction {
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return (namedArguments, context) => BinaryExpression.OperatorDictionary[this.NodeType](left(namedArguments, context), right(namedArguments, context))
            }
        }
    }
}