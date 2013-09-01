var __extends = this.__extends || function (d, b) {
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
        DefaultExpression.prototype._Compile = function () {
            return function (ambientContext) {
                return null;
            };
        };
        return DefaultExpression;
    })(WebExpressions.Expression);
    WebExpressions.DefaultExpression = DefaultExpression;    
})(WebExpressions || (WebExpressions = {}));
