
/// <reference path="Expression.ts" />

module WebExpressions {

        export class DefaultExpression extends Expression {
            constructor(meta: Meta.ExpressionMeta) {
                super(meta);
            }

            //EvalExpression(): CreateEvalExpression {
            //    return {
            //        Expression: "",
            //        Constants: new WebExpressions.Utils.Dictionary()
            //    };
            //}

            //TODO
            _Compile(): ExpressionInvokerAction {
                return (ambientContext) => null;
            }
        }
    }