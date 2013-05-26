
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        export class UnaryExpression extends Expression {
            
            private static OperatorDictionary: { (operand): any; }[] = (() => {
                var output: { (operand): any; }[] = [];

                // TODO: more (all) operators
                output[__BTF.Meta.ExpressionType.Convert] = (operand) => operand;
                output[__BTF.Meta.ExpressionType.Not] = (operand) => !operand;

                return output;
            })();

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
                return (ambientContext) => UnaryExpression.OperatorDictionary[this.NodeType](operand(ambientContext))
            }
        }
    }
}