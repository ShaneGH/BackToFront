/// <reference path="Expression.ts" />
/// <reference path="Utils/Dictionary.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var ConstantExpression = (function (_super) {
        __extends(ConstantExpression, _super);
        function ConstantExpression(meta) {
            _super.call(this, meta);

            this.Value = meta.Value;
        }
        ConstantExpression.prototype._Compile = function () {
            var _this = this;
            return function (ambientContext) {
                return _this.Value;
            };
        };
        ConstantExpression.ConstantParameter = "__constants";

        ConstantExpression.GenerateConstantId = (function () {
            var id = 0;
            return function () {
                return "constant-" + (++id);
            };
        })();
        return ConstantExpression;
    })(WebExpressions.Expression);
    WebExpressions.ConstantExpression = ConstantExpression;
})(WebExpressions || (WebExpressions = {}));
