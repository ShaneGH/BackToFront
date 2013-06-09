
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

            ToString(): string {
                return linq(this.Expressions).Select(a => a.ToString() + ";").Result.join("\n");
            }

            _Compile(): ExpressionInvokerAction {
                var children = linq(this.Expressions).Select(a => a.Compile()).Result;
                return (ambientContext) => linq(children).Each(a => a(ambientContext));
            }
        }
    }
