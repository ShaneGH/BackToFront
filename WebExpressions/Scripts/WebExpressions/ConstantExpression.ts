
/// <reference path="Expression.ts" />

module WebExpressions {

        export class ConstantExpression extends Expression {
            Value: any;

            constructor(meta: Meta.ConstantExpressionMeta) {
                super(meta);

                this.Value = meta.Value;
            }

            _Compile(): ExpressionInvokerAction {
                return (ambientContext) => this.Value;
            }
        }
    
}