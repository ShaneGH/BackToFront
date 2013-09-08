
/// <reference path="Expression.ts" />

module WebExpressions {

        export class DefaultExpression extends Expression {
            constructor(meta: Meta.ExpressionMeta) {
                super(meta);
            }

            //TODO
            _Compile(): ExpressionInvokerAction {
                return (ambientContext) => null;
            }
        }
    }