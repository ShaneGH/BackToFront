
/// <reference path="Expression.ts" />
/// <reference path="Utils/Dictionary.ts" />

module WebExpressions {

    export class ConstantExpression extends Expression {
        Value: any;

        // const
        static ConstantParameter = "__constants";

        static GenerateConstantId: { (): string; } = (function () {
            var id = 0;
            return function () {
                return "constant-" + (++id);
            };
        })();

        constructor(meta: Meta.ConstantExpressionMeta) {
            super(meta);

            this.Value = meta.Value;
        }

        //EvalExpression(): CreateEvalExpression {

        //    var accessor = ConstantExpression.GenerateConstantId();
        //    var constant = new WebExpressions.Utils.Dictionary();
        //    constant.Add(accessor, this.Value);
        //    return {
        //        Constants: constant,
        //        Expression: ConstantExpression.ConstantParameter + "[" + accessor + "]"
        //    };
        //}

        _Compile(): ExpressionInvokerAction {
            return (ambientContext) => this.Value;
        }
    }
}