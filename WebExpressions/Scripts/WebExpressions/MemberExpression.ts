
/// <reference path="Expression.ts" />

module WebExpressions {

    export interface MemberExpressionRegex {
        Property: RegExp;
        IndexedProperty: RegExp;
    }

    export class MemberExpressionBase extends Expression {

        static PropertyRegex: RegExp = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");

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
            if (!MemberExpressionBase.PropertyRegex.test(this.MemberName)) {
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

        // TODO: replace . with [] and watch for injection
        //EvalExpression(): CreateEvalExpression {
        //    throw "Not implemented, need to split into static and non static member references";

        //    if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
        //        throw "Invalid property name: " + this.MemberName;
        //    }

        //    var expression = this.Expression.EvalExpression();

        //    return {
        //        Expression: expression.Expression + "." + this.MemberName,
        //        Constants: expression.Constants
        //    };
        //};

        _CompileMemberContext(): ExpressionInvokerAction {
            return this.Expression.Compile();
        }
    }

    export class StaticMemberExpression extends MemberExpressionBase {

        Class: string[];

        constructor(meta: Meta.StaticMemberExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Class",
                inputType: "string"
            });

            this.Class = StaticMemberExpression.SplitNamespace(meta.Class);
        }

        static SplitNamespace(input: string): string[] {

            var output = input.split(".");
            linq(output).Each(a => {
                if (!MemberExpressionBase.PropertyRegex.test(a))
                    throw "Invalid namespace part " + a;
            });

            return output;
        }

        _CompileMemberContext(): ExpressionInvokerAction {

            var item = window;
            for (var i = 0, ii = this.Class.length; i < ii; i++) {
                item = item[this.Class[i]];
                if (item == undefined)
                    throw "Cannot evaluate member " + this.Class.join(".");
            }

            return (ambientContext) => item;
        }
    }
}