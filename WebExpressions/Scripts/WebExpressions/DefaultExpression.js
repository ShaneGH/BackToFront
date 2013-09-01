/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var DefaultExpression = (function (_super) {
        __extends(DefaultExpression, _super);
        function DefaultExpression(meta) {
            _super.call(this, meta);
        }
        //EvalExpression(): CreateEvalExpression {
        //    return {
        //        Expression: "",
        //        Constants: new WebExpressions.Utils.Dictionary()
        //    };
        //}
        //TODO
        DefaultExpression.prototype._Compile = function () {
            return function (ambientContext) {
                return null;
            };
        };
        return DefaultExpression;
    })(WebExpressions.Expression);
    WebExpressions.DefaultExpression = DefaultExpression;
})(WebExpressions || (WebExpressions = {}));
