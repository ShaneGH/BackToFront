var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var ConditionalExpression = (function (_super) {
            __extends(ConditionalExpression, _super);
            function ConditionalExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "IfTrue",
                    inputType: "object"
                }, {
                    inputName: "IfFalse",
                    inputType: "object"
                }, {
                    inputName: "Test",
                    inputType: "object"
                });
                this.IfTrue = Expressions.Expression.CreateExpression(meta.IfTrue);
                this.IfFalse = Expressions.Expression.CreateExpression(meta.IfFalse);
                this.Test = Expressions.Expression.CreateExpression(meta.Test);
            }
            ConditionalExpression.prototype._Compile = function () {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();
                return function (namedArguments, context) {
                    return test(namedArguments, context) ? ifTrue(namedArguments, context) : ifFalse(namedArguments, context);
                };
            };
            return ConditionalExpression;
        })(Expressions.Expression);
        Expressions.ConditionalExpression = ConditionalExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
