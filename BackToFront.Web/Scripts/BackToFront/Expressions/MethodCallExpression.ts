
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        export class MethodCallExpression extends Expression {
            Object: Expression;
            Arguments: Expression[];
            MethodName: string;
            MethodFullName: string;

            constructor(meta: Meta.MethodCallExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
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
            _Compile(): Validation.ExpressionInvokerAction {
                var name = this.MethodName;
                if (!__BTF.Expressions.MemberExpression.PropertyRegex.test(name)) {
                    throw "Invalid method name: " + name;
                }

                var object = this.Object.Compile();
                var args = linq(this.Arguments).Select(a => a.Compile()).Result;
                return (namedParameters, context) => {
                    var o = object(namedParameters, context);
                    var params = linq(args).Select(a => a(namedParameters, context)).Result;

                    return o[name].apply(o, params);
                };
            }
        }
    }
}