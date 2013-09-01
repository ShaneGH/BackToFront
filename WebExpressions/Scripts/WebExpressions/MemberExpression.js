/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var MemberExpression = (function (_super) {
        __extends(MemberExpression, _super);
        function MemberExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expression",
                inputType: "object",
                // if static member
                allowNull: true
            }, {
                inputName: "MemberName",
                inputConstructor: String
            });

            this.Expression = meta.Expression ? WebExpressions.Expression.CreateExpression(meta.Expression) : null;
            this.MemberName = meta.MemberName;
        }
        // TODO: replace . with [] and watch for injection
        //EvalExpression(): CreateEvalExpression {
        //    throw "Not implemented, need to split into static and non static member references";
        //    if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
        //        throw "Invalid property name: " + this.MemberName;
        //    }
        //    var expression = this.Expression.EvalExpression();
        //    return {
        //        Expression: expression.Expression + "." + this.MemberName,
        //        Constants: expression.Constants
        //    };
        //};
        MemberExpression.prototype._Compile = function () {
            if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
                throw "Invalid property name: " + this.MemberName;
            }

            if (this.Expression) {
                var name = this.MemberName;
                var expression = this.Expression.Compile();
                return function (ambientContext) {
                    return expression(ambientContext)[name];
                };
            } else {
                throw "Not implemented exception";
            }
        };
        MemberExpression.PropertyRegex = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");
        return MemberExpression;
    })(WebExpressions.Expression);
    WebExpressions.MemberExpression = MemberExpression;
})(WebExpressions || (WebExpressions = {}));
