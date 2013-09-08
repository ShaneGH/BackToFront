
/// <reference path="Expression.ts" />

module WebExpressions {

    export interface MemberExpressionRegex {
        Property: RegExp;
        IndexedProperty: RegExp;
    }

    export class MemberExpressionBase extends Expression {
        
        MemberName: string;

        constructor(meta: Meta.MemberExpressionMetaBase) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "MemberName",
                inputConstructor: String
            });

            this.MemberName = meta.MemberName;
        }

        _Compile(): ExpressionInvokerAction {
            if (!WebExpressions.Utils.CustomClassHandler.PropertyRegex.test(this.MemberName)) {
                throw "Invalid property name: " + this.MemberName;
            }

            var name = this.MemberName;
            var expression = this._CompileMemberContext();
            return (ambientContext) => expression(ambientContext)[name];
        }

        _CompileMemberContext(): ExpressionInvokerAction {
            throw "This method is abstract";
        }
    }

    export class MemberExpression extends MemberExpressionBase {

        Expression: Expression;

        constructor(meta: Meta.MemberExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expression",
                inputType: "object"
            });

            this.Expression = Expression.CreateExpression(meta.Expression);
        }

        _CompileMemberContext(): ExpressionInvokerAction {
            return this.Expression.Compile();
        }
    }

    export class StaticMemberExpression extends MemberExpressionBase {

        Class: string;

        constructor(meta: Meta.StaticMemberExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Class",
                inputType: "string"
            });

            this.Class = meta.Class;
        }

        _CompileMemberContext(): ExpressionInvokerAction {

            var item = WebExpressions.Utils.CustomClassHandler.GetClass(this.Class);
            return (ambientContext) => item;
        }
    }
}