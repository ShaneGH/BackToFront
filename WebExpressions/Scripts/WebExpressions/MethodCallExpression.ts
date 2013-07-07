
/// <reference path="Expression.ts" />

module WebExpressions {

    export class MethodCallExpression extends Expression {
        Object: Expression;
        Arguments: Expression[];
        MethodName: string;
        MethodFullName: string;

        constructor(meta: Meta.MethodCallExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Object",
                inputType: "object",
                // if static member
                allowNull: true
            }, {
                inputName: "Arguments",
                inputConstructor: Array
            }, {
                inputName: "MethodName",
                inputConstructor: String
            }, {
                inputName: "MethodFullName",
                inputConstructor: String
            });

            this.Object = meta.Object ? Expression.CreateExpression(meta.Object) : null;
            this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
            this.MethodName = meta.MethodName;
            this.MethodFullName = meta.MethodFullName;
        }

        EvalExpression(): CreateEvalExpression {

            throw "Not implemented, need to split into static and non static method calls";

            if (!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }

            var args = <string[]>linq(this.Arguments).Select(a => a.EvalExpression()).Result;
            var object = this.Object ? this.Object.EvalExpression() : { Expression: "window", Constants: new WebExpressions.Utils.Dictionary() };
            linq(args).Each(a => object.Constants.Merge(a.Constants));

            var mthd = "__o[\"" + this.MethodName + "\"]";

            return {
                Expression: "(function (__o) { return (" + mthd + " ? " + mthd + " : ex.ns.MethodCallExpression.RegisteredMethods[\"" + this.MethodFullName + "\"]).call(__o, " + args.join(", ") + "); })(" + object.Expression + ")",
                Constants: object.Constants
            };
        }

        // TODO: register methods
        _Compile(): ExpressionInvokerAction {
            if (!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }

            //TODO: unit test (ctxt) => window
            var object = this.Object ? this.Object.Compile() : (ctxt) => window;
            var args = linq(this.Arguments).Select(a => a.Compile()).Result;
            return (ambientContext) => {
                var o = object(ambientContext);
                var params = linq(args).Select(a => a(ambientContext)).Result;

                return (o[this.MethodName] ? o[this.MethodName] : MethodCallExpression.RegisteredMethods[this.MethodFullName]).apply(o, params);
            };
        }

        static RegisteredMethods: Object = {};
    }
}