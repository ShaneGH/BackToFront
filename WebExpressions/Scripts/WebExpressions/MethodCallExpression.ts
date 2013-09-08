
/// <reference path="Expression.ts" />

module WebExpressions {

    export class MethodCallExpressionBase extends Expression {
        Arguments: Expression[];
        MethodName: string;

        constructor(meta: Meta.MethodCallExpressionMetaBase) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Arguments",
                    inputConstructor: Array
                }, {
                    inputName: "MethodName",
                    inputConstructor: String
                });

            this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
            this.MethodName = meta.MethodName;
        }

        _Compile(): ExpressionInvokerAction {
            if (!WebExpressions.Utils.CustomClassHandler.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }

            var name = this.MethodName;
            var object = this._CompileMethodCallContext();
            var args = linq(this.Arguments).Select(a => a.Compile()).Result;

            return (ambientContext) => {
                var params = linq(args).Select(a => a(ambientContext)).Result;
                var o = object(ambientContext);
                return o[this.MethodName].apply(o, params);
            };
        }

        _CompileMethodCallContext(): ExpressionInvokerAction {
            throw "This method is abstract";
        }
    }

    export class MethodCallExpression extends MethodCallExpressionBase {
        Object: Expression;

        constructor(meta: Meta.MethodCallExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Object",
                inputType: "object"
            });

            this.Object = Expression.CreateExpression(meta.Object);
        }

        _CompileMethodCallContext(): ExpressionInvokerAction {
            return this.Object.Compile();
        }

        static RegisteredMethods = {};
    }

    export class StaticMethodCallExpression extends MethodCallExpressionBase {
        Class: string[];

        constructor(meta: Meta.StaticMethodCallExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Class",
                inputType: "string"
            });

            this.Class = WebExpressions.Utils.CustomClassHandler.SplitNamespace(meta.Class);
        }

        _CompileMethodCallContext(): ExpressionInvokerAction {

            return (item) => WebExpressions.Utils.CustomClassHandler.GetClass(this.Class);
        }
    }
}