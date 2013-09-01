
/// <reference path="Expression.ts" />

module WebExpressions {

        export class InvocationExpression extends Expression {
            Expression: Expression;
            Arguments: Expression[];

            constructor(meta: Meta.InvocationExpressionMeta) {
                super(meta);

                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object"
                }, {
                    inputName: "Arguments",
                    inputConstructor: Array
                });

                this.Expression = Expression.CreateExpression(meta.Expression);
                this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
            }

            //EvalExpression(): CreateEvalExpression {
            //    var expression = this.Expression.EvalExpression();
            //    var args = linq(this.Arguments).Select(a => a.EvalExpression());
            //    linq(args).Each(a => expression.Constants.Merge(a.Constants));

            //    return {
            //        Constants: expression.Constants,
            //        Expression: expression.Expression + "(" + linq(args).Select(a => a.Expression).Result.join(", ") + ")"
            //    };
            //}

            _Compile(): ExpressionInvokerAction {

                var expresion = this.Expression.Compile();
                var args = linq(this.Arguments).Select(a => a.Compile()).Result;
                return (ambientContext) => {
                    var e = expresion(ambientContext);
                    var params = linq(args).Select(a => a(ambientContext)).Result;

                    return e.apply(ambientContext, params);
                };
            }
        
    }
}