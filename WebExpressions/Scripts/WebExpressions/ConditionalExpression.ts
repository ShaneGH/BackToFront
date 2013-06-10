
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
                //TODO: can IfFalse be null?
                this.IfFalse = Expression.CreateExpression(meta.IfFalse);
                this.Test = Expression.CreateExpression(meta.Test);
            }

            EvalExpression(): CreateEvalExpression {
                if (this.IfTrue.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block ||
                    this.IfFalse.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block) {
                    return this._ToBlockString();
                }

                return this._ToInlineString();
            }

            // TODO: can ifFalse be null
            private _ToInlineString(): CreateEvalExpression {
                var test = this.Test.EvalExpression();
                var ifTrue = this.IfTrue.EvalExpression();
                var ifFalse = this.IfFalse.EvalExpression();

                return {
                    Constants: test.Constants.Merge(ifTrue.Constants).Merge(ifFalse.Constants),
                    Expression: test.Expression + " ? " + ifTrue.Expression + " : " + ifFalse.Expression
                };
            }

            // TODO: can ifFalse be null
            private _ToBlockString(): CreateEvalExpression {
                var test = this.Test.EvalExpression();
                var ifTrue = this.IfTrue.EvalExpression();
                var ifFalse = this.IfFalse.EvalExpression();

                return {
                    Constants: test.Constants.Merge(ifTrue.Constants).Merge(ifFalse.Constants),
                    Expression: "if(" + test.Expression + ") { " +
                        ifTrue.Expression +
                        " } else { " +
                        ifFalse.Expression +
                        " }"
                };
            }

            _Compile(): ExpressionInvokerAction {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();

                return (ambientContext) => test(ambientContext) ? ifTrue(ambientContext) : ifFalse(ambientContext);
            }
        }
    }
