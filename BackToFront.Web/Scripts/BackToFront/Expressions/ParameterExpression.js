var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    var Validation = __BTF.Validation;
    var Meta = __BTF.Meta;
    (function (Expressions) {
        var ParameterExpression = (function (_super) {
            __extends(ParameterExpression, _super);
            function ParameterExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Name",
                    inputConstructor: String
                });
                this.Name = meta.Name;
            }
            ParameterExpression.prototype._Compile = function () {
                var _this = this;
                return function (namedArguments, context) {
                    return namedArguments[_this.Name];
                };
            };
            return ParameterExpression;
        })(Expressions.Expression);
        Expressions.ParameterExpression = ParameterExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
