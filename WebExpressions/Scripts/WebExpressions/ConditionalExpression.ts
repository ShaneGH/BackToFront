
/// <reference path="Expression.ts" />

module WebExpressions {

        export class ConditionalExpression extends Expression {
            IfTrue: Expression;
            IfFalse: Expression;
            Test: Expression;

            constructor(meta: Meta.ConditionalExpressionMeta) {
                super(meta);

                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "IfTrue",
                    inputType: "object"
                }, {
                    inputName: "IfFalse",
                    inputType: "object"
                }, {
                    inputName: "Test",
                    inputType: "object"
                });

                this.IfTrue = Expression.CreateExpression(meta.IfTrue);
                this.IfFalse = Expression.CreateExpression(meta.IfFalse);
                this.Test = Expression.CreateExpression(meta.Test);
            }

            _Compile(): ExpressionInvokerAction {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();

                return (ambientContext) => test(ambientContext) ? ifTrue(ambientContext) : ifFalse(ambientContext);
            }
        }
    }
