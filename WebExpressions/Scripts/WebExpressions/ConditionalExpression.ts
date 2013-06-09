
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

            ToString(): string {
                if (this.IfTrue.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block ||
                    this.IfFalse.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block) {
                    return this._ToBlockString();
                }

                return this._ToInlineString();
            }

            private _ToInlineString(): string {
                return this.Test.ToString() + " ? " + this.IfTrue.ToString() + " : " + this.IfFalse.ToString();

            }

            private _ToBlockString(): string {
                return "if(" + this.Test.ToString() + ") { " +
                    this.IfTrue.ToString() +
                    " } else { " +
                    this.IfFalse.ToString() +
                    " }";
            }

            _Compile(): ExpressionInvokerAction {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();

                return (ambientContext) => test(ambientContext) ? ifTrue(ambientContext) : ifFalse(ambientContext);
            }
        }
    }
