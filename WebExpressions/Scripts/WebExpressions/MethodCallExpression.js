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
            this.Object = WebExpressions.Expression.CreateExpression(meta.Object);
            this.Arguments = linq(meta.Arguments).Select(function (a) {
                return WebExpressions.Expression.CreateExpression(a);
            }).Result;
            this.MethodName = meta.MethodName;
            this.MethodFullName = meta.MethodFullName;
        }
        MethodCallExpression.prototype.ToString = function () {
            if(!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }
            var args = linq(this.Arguments).Select(function (a) {
                return a.ToString();
            }).Result;
            var object = this.Object.ToString();
            var mthd = "o[\"" + this.MethodName + "\"]";
            return "(" + mthd + " ? " + mthd + " : ex.ns.MethodCallExpression.RegisteredMethods[\"" + this.MethodFullName + "\"])" + ".call(";
            return "(function (o) { return (" + mthd + " ? " + mthd + " : ex.ns.MethodCallExpression.RegisteredMethods[\"" + this.MethodFullName + "\"]).call(o, " + args.join(", ") + "); })(" + object + ")";
        };
        MethodCallExpression.prototype._Compile = function () {
            var _this = this;
            if(!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
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
                return (o[_this.MethodName] ? o[_this.MethodName] : MethodCallExpression.RegisteredMethods[_this.MethodFullName]).apply(o, params);
            };
        };
        MethodCallExpression.RegisteredMethods = {
        };
        return MethodCallExpression;
    })(WebExpressions.Expression);
    WebExpressions.MethodCallExpression = MethodCallExpression;    
})(WebExpressions || (WebExpressions = {}));
