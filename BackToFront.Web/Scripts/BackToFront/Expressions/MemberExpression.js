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
            MemberExpression.RegexValues = (function () {
                var index = "\\[[0-9]+\\]";
                var indexedProperty = "[_a-zA-Z][_a-zA-Z0-9]*(" + index + ")?";
                return {
                    IndexedProperty: new RegExp(index + "$"),
                    Property: new RegExp("^" + indexedProperty + "$")
                };
            })();
            MemberExpression.prototype._Compile = function () {
                var _this = this;
                var expression = this.Expression.Compile();
                return function (namedArguments, context) {
                    if(!MemberExpression.RegexValues.Property.test(_this.MemberName)) {
                        throw "Invalid property name: " + _this.MemberName;
                    }
                    var base = expression(namedArguments, context);
                    if(MemberExpression.RegexValues.IndexedProperty.test(_this.MemberName)) {
                        var property = _this.MemberName.substr(0, _this.MemberName.indexOf("[") - 1);
                        var index = MemberExpression.RegexValues.Property.exec(_this.MemberName)[0];
                        index = parseInt(index.substring(1, index.length - 1));
                        base = base[property];
                        if(base == null) {
                            return null;
                        }
                        return base[index];
                    }
                    return base[_this.MemberName];
                };
            };
            return MemberExpression;
        })(Expressions.Expression);
        Expressions.MemberExpression = MemberExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
