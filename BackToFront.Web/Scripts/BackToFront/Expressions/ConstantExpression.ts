/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        export class ConstantExpression extends Expression {
            constructor(meta: Meta.ConstantExpressionMeta) {
                super(meta);
            }

            //TODO
            _Compile(): Validation.ExpressionInvokerAction {
                return (namedArguments, context) => null;
            }
        }
    }
}