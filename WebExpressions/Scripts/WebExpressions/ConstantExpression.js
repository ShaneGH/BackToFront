var __extends = this.__extends || function (d, b) {
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
        return ConstantExpression;
    })(WebExpressions.Expression);
    WebExpressions.ConstantExpression = ConstantExpression;    
})(WebExpressions || (WebExpressions = {}));
