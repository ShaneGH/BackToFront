
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

        //EvalExpression(): CreateEvalExpression {
        //    return {
        //        Expression: this.Name,
        //        Constants: new WebExpressions.Utils.Dictionary()
        //    }
        //}

        _Compile(): ExpressionInvokerAction {
            return ambientContext => ambientContext[this.Name];
        }
    }
}