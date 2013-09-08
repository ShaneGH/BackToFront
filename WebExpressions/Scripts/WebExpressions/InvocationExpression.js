/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var InvocationExpression = (function (_super) {
        __extends(InvocationExpression, _super);
        function InvocationExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expression",
                inputType: "object"
            }, {
                inputName: "Arguments",
                inputConstructor: Array
            });

            this.Expression = WebExpressions.Expression.CreateExpression(meta.Expression);
            this.Arguments = linq(meta.Arguments).Select(function (a) {
                return WebExpressions.Expression.CreateExpression(a);
            }).Result;
        }
        InvocationExpression.prototype._Compile = function () {
            var expresion = this.Expression.Compile();
            var args = linq(this.Arguments).Select(function (a) {
                return a.Compile();
            }).Result;
            return function (ambientContext) {
                var e = expresion(ambientContext);
                var params = linq(args).Select(function (a) {
                    return a(ambientContext);
                }).Result;

                return e.apply(ambientContext, params);
            };
        };
        return InvocationExpression;
    })(WebExpressions.Expression);
    WebExpressions.InvocationExpression = InvocationExpression;
})(WebExpressions || (WebExpressions = {}));
