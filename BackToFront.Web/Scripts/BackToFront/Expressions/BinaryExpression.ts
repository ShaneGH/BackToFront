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

            private static OperatorDictionary: { (left, right): any; }[] = (() => {
                var output: { (left, right): any; }[] = [];

                // TODO: more (all) operators
                output[__BTF.Meta.ExpressionType.Add] = (left, right) => left + right;
                output[__BTF.Meta.ExpressionType.AndAlso] = (left, right) => left && right;
                output[__BTF.Meta.ExpressionType.Divide] = (left, right) => left / right;
                output[__BTF.Meta.ExpressionType.GreaterThan] = (left, right) => left > right;
                output[__BTF.Meta.ExpressionType.GreaterThanOrEqual] = (left, right) => left >= right;
                output[__BTF.Meta.ExpressionType.LessThan] = (left, right) => left < right;
                output[__BTF.Meta.ExpressionType.LessThanOrEqual] = (left, right) => left <= right;
                output[__BTF.Meta.ExpressionType.Multiply] = (left, right) => left * right;
                output[__BTF.Meta.ExpressionType.OrElse] = (left, right) => left || right;
                output[__BTF.Meta.ExpressionType.Subtract] = (left, right) => left - right;

                return output;
            })();

            constructor(meta: Meta.BinaryExpressionMeta) {
                super(meta);

                if (!BinaryExpression.OperatorDictionary[this.NodeType])
                    throw "##" + "Invalid Operator";

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });

                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            };

            _Compile(): Validation.ExpressionInvokerAction {
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return (ambientContext) => BinaryExpression.OperatorDictionary[this.NodeType](left(ambientContext), right(ambientContext))
            };
        }
    }
}