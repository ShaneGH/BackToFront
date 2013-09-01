
/// <reference path="Expression.ts" />

module WebExpressions {

        export class BlockExpression extends Expression {
            Expressions: Expression[];

            constructor(meta: Meta.BlockExpressionMeta) {
                super(meta);

                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Expressions",
                    inputConstructor: Array
                });

                this.Expressions = linq(meta.Expressions).Select(a => Expression.CreateExpression(a)).Result;
            }

            //EvalExpression(): CreateEvalExpression {
            //    var expressions = <CreateEvalExpression[]>linq(this.Expressions).Select(a => a.EvalExpression()).Result;
            //    if (!expressions.length) {
            //        return {
            //            Constants: new WebExpressions.Utils.Dictionary(),
            //            Expression: ""
            //        };
            //    }

            //    for (var i = 1, ii = expressions.length; i < ii; i++) {
            //        expressions[0].Constants.Merge(expressions[i].Constants);
            //    }

            //    return {
            //        Expression: linq(expressions).Select(a => a.Expression).Result.join(";\n"),
            //        Constants: expressions[0].Constants
            //    };
            //}

            _Compile(): ExpressionInvokerAction {
                var children = linq(this.Expressions).Select(a => a.Compile()).Result;
                return (ambientContext) => linq(children).Each(a => a(ambientContext));
            }
        }
    }
