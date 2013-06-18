
/// <reference path="Expression.ts" />

module WebExpressions {

        export interface MemberExpressionRegex {
            Property: RegExp;
            IndexedProperty: RegExp;
        }

        export class MemberExpression extends Expression {

            static PropertyRegex: RegExp = new RegExp("^[a-zA-Z][a-zA-Z0-9]*$");

            Expression: Expression;
            MemberName: string;

            constructor(meta: Meta.MemberExpressionMeta) {
                super(meta);

                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object",
                    // static member
                    allowNull: true
                }, {
                    inputName: "MemberName",
                    inputConstructor: String
                });

                this.Expression = meta.Expression ? Expression.CreateExpression(meta.Expression) : null;
                this.MemberName = meta.MemberName;
            }

            // TODO: replace . with [] and watch for injection
            EvalExpression(): CreateEvalExpression {
                throw "Not implemented, need to split into static and non static member references";

                if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
                    throw "Invalid property name: " + this.MemberName;
                }

                var expression = this.Expression.EvalExpression();

                return {
                    Expression: expression.Expression + "." + this.MemberName,
                    Constants: expression.Constants
                };
            }

            // TODO: not throwing null exceptions
            _Compile(): ExpressionInvokerAction {
                if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
                    throw "Invalid property name: " + this.MemberName;
                }

                if (this.Expression) {
                    var name = this.MemberName;
                    var expression = this.Expression.Compile();
                    return (ambientContext) => expression(ambientContext)[name];
                } else {
                    throw "Not implemented exception";
                }
            }
        }
    
}