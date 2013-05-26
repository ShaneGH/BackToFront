/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        export class ConstantExpression extends Expression {
            Value: any;

            constructor(meta: Meta.ConstantExpressionMeta) {
                super(meta);

                this.Value = meta.Value;
            }

            _Compile(): Validation.ExpressionInvokerAction {
                return (ambientContext) => this.Value;
            }
        }
    }
}