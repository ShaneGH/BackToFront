
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        export class ParameterExpression extends Expression {
            Name: string;

            constructor(meta: Meta.ParameterExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Name",
                    inputConstructor: String
                });

                this.Name = meta.Name;
            }

            _Compile(): Validation.ExpressionInvokerAction {
                return ambientContext => ambientContext[this.Name];
            }
        }
    }
}