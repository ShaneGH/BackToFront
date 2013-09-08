/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var ParameterExpression = (function (_super) {
        __extends(ParameterExpression, _super);
        function ParameterExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Name",
                inputConstructor: String
            });

            this.Name = meta.Name;
        }
        ParameterExpression.prototype._Compile = function () {
            var _this = this;
            return function (ambientContext) {
                return ambientContext[_this.Name];
            };
        };
        return ParameterExpression;
    })(WebExpressions.Expression);
    WebExpressions.ParameterExpression = ParameterExpression;
})(WebExpressions || (WebExpressions = {}));
