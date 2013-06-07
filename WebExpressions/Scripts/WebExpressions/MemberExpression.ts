
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
                    inputType: "object"
                }, {
                    inputName: "MemberName",
                    inputConstructor: String
                });

                this.Expression = Expression.CreateExpression(meta.Expression);
                this.MemberName = meta.MemberName;
            }

            // TODO: not throwing null exceptions
            _Compile(): ExpressionInvokerAction {
                if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
                    throw "Invalid property name: " + this.MemberName;
                }

                var name = this.MemberName;
                var expression = this.Expression.Compile();
                return (ambientContext) => expression(ambientContext)[name];
            }
        }
    
}