
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
                inputType: "object"
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

            this.Object = Expression.CreateExpression(meta.Object);
            this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
            this.MethodName = meta.MethodName;
            this.MethodFullName = meta.MethodFullName;
        }

        ToString(): string {
            if (!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }

            var args = <string[]>linq(this.Arguments).Select(a => a.ToString()).Result;
            var object = this.Object.ToString();

            var mthd = "o[\"" + this.MethodName + "\"]";
            return "(" + mthd + " ? " + mthd + " : ex.ns.MethodCallExpression.RegisteredMethods[\"" + this.MethodFullName + "\"])"
                + ".call(";

            return "(function (o) { return (" + mthd + " ? " + mthd + " : ex.ns.MethodCallExpression.RegisteredMethods[\"" + this.MethodFullName + "\"]).call(o, " + args.join(", ") + "); })(" + object + ")";
        }

        // TODO: register methods
        _Compile(): ExpressionInvokerAction {
            if (!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }

            var object = this.Object.Compile();
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