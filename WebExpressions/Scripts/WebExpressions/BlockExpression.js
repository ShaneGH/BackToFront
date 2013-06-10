var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var BlockExpression = (function (_super) {
        __extends(BlockExpression, _super);
        function BlockExpression(meta) {
                _super.call(this, meta);
            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expressions",
                inputConstructor: Array
            });
            this.Expressions = linq(meta.Expressions).Select(function (a) {
                return WebExpressions.Expression.CreateExpression(a);
            }).Result;
        }
        BlockExpression.prototype.EvalExpression = function () {
            var expressions = linq(this.Expressions).Select(function (a) {
                return a.EvalExpression();
            }).Result;
            if(!expressions.length) {
                return {
                    Constants: new WebExpressions.Utils.Dictionary(),
                    Expression: ""
                };
            }
            for(var i = 1, ii = expressions.length; i < ii; i++) {
                expressions[0].Constants.Merge(expressions[i].Constants);
            }
            return {
                Expression: linq(expressions).Select(function (a) {
                    return a.Expression;
                }).Result.join(";\n"),
                Constants: expressions[0].Constants
            };
        };
        BlockExpression.prototype._Compile = function () {
            var children = linq(this.Expressions).Select(function (a) {
                return a.Compile();
            }).Result;
            return function (ambientContext) {
                return linq(children).Each(function (a) {
                    return a(ambientContext);
                });
            };
        };
        return BlockExpression;
    })(WebExpressions.Expression);
    WebExpressions.BlockExpression = BlockExpression;    
})(WebExpressions || (WebExpressions = {}));
