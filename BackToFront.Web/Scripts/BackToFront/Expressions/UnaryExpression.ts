
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        export class UnaryExpression extends Expression {

            private static OperatorDictionary: { (operand): any; }[] = [];

            Operand: Expression;

            constructor(meta: Meta.UnaryExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Operand",
                    inputType: "object"
                });

                this.Operand = Expression.CreateExpression(meta.Operand);
            }

            _Compile(): Validation.ExpressionInvokerAction {
                var operand = this.Operand.Compile();
                return (namedArguments, context) => UnaryExpression.OperatorDictionary[this.NodeType](operand(namedArguments, context))
            }
        }
    }
}