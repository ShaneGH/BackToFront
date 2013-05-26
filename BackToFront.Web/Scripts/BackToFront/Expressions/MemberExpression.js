var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var MemberExpression = (function (_super) {
            __extends(MemberExpression, _super);
            function MemberExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object"
                }, {
                    inputName: "MemberName",
                    inputConstructor: String
                });
                this.Expression = Expressions.Expression.CreateExpression(meta.Expression);
                this.MemberName = meta.MemberName;
            }
            MemberExpression.PropertyRegex = new RegExp("^[a-zA-Z][a-zA-Z0-9]*$");
            MemberExpression.prototype._Compile = function () {
                if(!MemberExpression.PropertyRegex.test(this.MemberName)) {
                    throw "Invalid property name: " + this.MemberName;
                }
                var name = this.MemberName;
                var expression = this.Expression.Compile();
                return function (ambientContext) {
                    return expression(ambientContext)[name];
                };
            };
            return MemberExpression;
        })(Expressions.Expression);
        Expressions.MemberExpression = MemberExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
