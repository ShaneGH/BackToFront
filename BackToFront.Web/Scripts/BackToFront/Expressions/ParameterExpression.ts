
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

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
                return (namedArguments, context) => namedArguments[this.Name];
            }
        }
    }
}