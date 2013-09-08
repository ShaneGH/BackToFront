
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

        // TODO: register methods
        _Compile(): ExpressionInvokerAction {
            if (!WebExpressions.MemberExpressionBase.PropertyRegex.test(this.MethodName)) {
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