var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var BlockExpression = (function (_super) {
            __extends(BlockExpression, _super);
            function BlockExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expressions",
                    inputConstructor: Array
                });
                this.Expressions = linq(meta.Expressions).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
            }
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
        })(Expressions.Expression);
        Expressions.BlockExpression = BlockExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
