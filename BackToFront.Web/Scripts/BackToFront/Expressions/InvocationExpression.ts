
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        export class InvocationExpression extends Expression {
            Expression: Expression;
            Arguments: Expression[];

            constructor(meta: Meta.InvocationExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object"
                }, {
                    inputName: "Arguments",
                    inputConstructor: Array
                });

                this.Expression = Expression.CreateExpression(meta.Expression);
                this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
            }

            _Compile(): Validation.ExpressionInvokerAction {

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
}