
/// <reference path="Expression.ts" />

module WebExpressions {

    export class ParameterExpression extends Expression {
        Name: string;

        constructor(meta: Meta.ParameterExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Name",
                inputConstructor: String
            });

            this.Name = meta.Name;
        }

        ToString(): string {
            return this.Name;
        }

        _Compile(): ExpressionInvokerAction {
            return ambientContext => ambientContext[this.Name];
        }
    }
}