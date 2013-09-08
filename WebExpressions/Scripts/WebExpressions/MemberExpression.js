/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var MemberExpressionBase = (function (_super) {
        __extends(MemberExpressionBase, _super);
        function MemberExpressionBase(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "MemberName",
                inputConstructor: String
            });

            this.MemberName = meta.MemberName;
        }
        MemberExpressionBase.prototype._Compile = function () {
            if (!MemberExpressionBase.PropertyRegex.test(this.MemberName)) {
                throw "Invalid property name: " + this.MemberName;
            }

            var name = this.MemberName;
            var expression = this._CompileMemberContext();
            return function (ambientContext) {
                return expression(ambientContext)[name];
            };
        };

        MemberExpressionBase.prototype._CompileMemberContext = function () {
            throw "This method is abstract";
        };
        MemberExpressionBase.PropertyRegex = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");
        return MemberExpressionBase;
    })(WebExpressions.Expression);
    WebExpressions.MemberExpressionBase = MemberExpressionBase;

    var MemberExpression = (function (_super) {
        __extends(MemberExpression, _super);
        function MemberExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expression",
                inputType: "object"
            });

            this.Expression = WebExpressions.Expression.CreateExpression(meta.Expression);
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
        MemberExpression.prototype._CompileMemberContext = function () {
            return this.Expression.Compile();
        };
        return MemberExpression;
    })(MemberExpressionBase);
    WebExpressions.MemberExpression = MemberExpression;

    var StaticMemberExpression = (function (_super) {
        __extends(StaticMemberExpression, _super);
        function StaticMemberExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Class",
                inputType: "string"
            });

            this.Class = StaticMemberExpression.SplitNamespace(meta.Class);
        }
        StaticMemberExpression.SplitNamespace = function (input) {
            var output = input.split(".");
            linq(output).Each(function (a) {
                if (!MemberExpressionBase.PropertyRegex.test(a))
                    throw "Invalid namespace part " + a;
            });

            return output;
        };

        StaticMemberExpression.prototype._CompileMemberContext = function () {
            var item = window;
            for (var i = 0, ii = this.Class.length; i < ii; i++) {
                item = item[this.Class[i]];
                if (item == undefined)
                    throw "Cannot evaluate member " + this.Class.join(".");
            }

            return function (ambientContext) {
                return item;
            };
        };
        return StaticMemberExpression;
    })(MemberExpressionBase);
    WebExpressions.StaticMemberExpression = StaticMemberExpression;
})(WebExpressions || (WebExpressions = {}));
