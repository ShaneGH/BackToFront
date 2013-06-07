
/// <reference path="Expression.ts" />

module WebExpressions {

        export class UnaryExpression extends Expression {

            private static OperatorDictionary: { (operand): any; }[] = (() => {
                var output: { (operand): any; }[] = [];

                // TODO: more (all) operators
                output[WebExpressions.Meta.ExpressionType.Convert] = (operand) => operand;
                output[WebExpressions.Meta.ExpressionType.Not] = (operand) => !operand;

                return output;
            })();

            Operand: Expression;

            constructor(meta: Meta.UnaryExpressionMeta) {
                super(meta);

                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Operand",
                    inputType: "object"
                });

                this.Operand = Expression.CreateExpression(meta.Operand);
            }

            _Compile(): ExpressionInvokerAction {
                var operand = this.Operand.Compile();
                return (ambientContext) => UnaryExpression.OperatorDictionary[this.NodeType](operand(ambientContext))
            }
        }
    
}