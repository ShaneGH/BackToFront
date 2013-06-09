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
        ConstantExpression.prototype.ToString = function () {
            return ConstantExpression.ToString(this.Value);
        };
        ConstantExpression.prototype._Compile = function () {
            var _this = this;
            return function (ambientContext) {
                return _this.Value;
            };
        };
        ConstantExpression.ToString = function ToString(value) {
            if(value === null) {
                return "null";
            } else if(value === undefined) {
                return "undefined";
            } else if(ConstantExpression.RegisteredConverters.ContainsKey(this.Value.constructor)) {
                return ConstantExpression.RegisteredConverters.Value(value.constructor)(value);
            } else {
                return null;
            }
        };
        ConstantExpression.RegisteredConverters = (function () {
            var dic = new WebExpressions.Utils.Dictionary();
            dic.Add(String, function (a) {
                return "\"" + a + "\"";
            });
            dic.Add(Number, function (a) {
                return a.toString();
            });
            dic.Add(Boolean, function (a) {
                return a.toString();
            });
            dic.Add(Array, function (a) {
                return "[" + linq(a).Select(function (a) {
                    return ConstantExpression.ToString(a);
                }).Result.join(", ") + "]";
            });
            return dic;
        })();
        return ConstantExpression;
    })(WebExpressions.Expression);
    WebExpressions.ConstantExpression = ConstantExpression;    
})(WebExpressions || (WebExpressions = {}));
