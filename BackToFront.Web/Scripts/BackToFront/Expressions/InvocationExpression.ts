
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        //TODO
        export class InvocationExpression extends Expression {
            constructor(meta: Meta.ConditionalExpressionMeta) {
                super(meta);
            }

            _Compile(): Validation.ExpressionInvokerAction {

                return (ambientContext) => null;
            }
        }
    }
}