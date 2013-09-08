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
            if (!WebExpressions.Utils.CustomClassHandler.PropertyRegex.test(this.MemberName)) {
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

            this.Class = meta.Class;
        }
        StaticMemberExpression.prototype._CompileMemberContext = function () {
            var item = WebExpressions.Utils.CustomClassHandler.GetClass(this.Class);
            return function (ambientContext) {
                return item;
            };
        };
        return StaticMemberExpression;
    })(MemberExpressionBase);
    WebExpressions.StaticMemberExpression = StaticMemberExpression;
})(WebExpressions || (WebExpressions = {}));
