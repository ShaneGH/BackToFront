var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var MethodCallExpression = (function (_super) {
            __extends(MethodCallExpression, _super);
            function MethodCallExpression(meta) {
                        _super.call(this, meta);
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
                this.Object = Expressions.Expression.CreateExpression(meta.Object);
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
                this.MethodName = meta.MethodName;
                this.MethodFullName = meta.MethodFullName;
            }
            MethodCallExpression.prototype._Compile = function () {
                var name = this.MethodName;
                if(!__BTF.Expressions.MemberExpression.PropertyRegex.test(name)) {
                    throw "Invalid method name: " + name;
                }
                var object = this.Object.Compile();
                var args = linq(this.Arguments).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    var o = object(ambientContext);
                    var params = linq(args).Select(function (a) {
                        return a(ambientContext);
                    }).Result;
                    return o[name].apply(o, params);
                };
            };
            return MethodCallExpression;
        })(Expressions.Expression);
        Expressions.MethodCallExpression = MethodCallExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
