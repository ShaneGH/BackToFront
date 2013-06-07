
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

            // TODO: register methods
            _Compile(): ExpressionInvokerAction {
                var name = this.MethodName;
                if (!WebExpressions.MemberExpression.PropertyRegex.test(name)) {
                    throw "Invalid method name: " + name;
                }

                var object = this.Object.Compile();
                var args = linq(this.Arguments).Select(a => a.Compile()).Result;
                return (ambientContext) => {
                    var o = object(ambientContext);
                    var params = linq(args).Select(a => a(ambientContext)).Result;

                    return o[name].apply(o, params);
                };
            }
        }
    
}