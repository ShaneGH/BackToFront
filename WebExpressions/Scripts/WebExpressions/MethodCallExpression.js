var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var MethodCallExpression = (function (_super) {
        __extends(MethodCallExpression, _super);
        function MethodCallExpression(meta) {
                _super.call(this, meta);
            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Object",
                inputType: "object",
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
            this.Object = meta.Object ? WebExpressions.Expression.CreateExpression(meta.Object) : null;
            this.Arguments = linq(meta.Arguments).Select(function (a) {
                return WebExpressions.Expression.CreateExpression(a);
            }).Result;
            this.MethodName = meta.MethodName;
            this.MethodFullName = meta.MethodFullName;
        }
        MethodCallExpression.prototype._Compile = function () {
            var _this = this;
            if(!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }
            var object = this.Object ? this.Object.Compile() : function (ctxt) {
                return window;
            };
            var args = linq(this.Arguments).Select(function (a) {
                return a.Compile();
            }).Result;
            return function (ambientContext) {
                var o = object(ambientContext);
                var params = linq(args).Select(function (a) {
                    return a(ambientContext);
                }).Result;
                return (o[_this.MethodName] ? o[_this.MethodName] : MethodCallExpression.RegisteredMethods[_this.MethodFullName]).apply(o, params);
            };
        };
        MethodCallExpression.RegisteredMethods = {
        };
        return MethodCallExpression;
    })(WebExpressions.Expression);
    WebExpressions.MethodCallExpression = MethodCallExpression;    
})(WebExpressions || (WebExpressions = {}));
