/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var MethodCallExpressionBase = (function (_super) {
        __extends(MethodCallExpressionBase, _super);
        function MethodCallExpressionBase(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Arguments",
                inputConstructor: Array
            }, {
                inputName: "MethodName",
                inputConstructor: String
            });

            this.Arguments = linq(meta.Arguments).Select(function (a) {
                return WebExpressions.Expression.CreateExpression(a);
            }).Result;
            this.MethodName = meta.MethodName;
        }
        MethodCallExpressionBase.prototype._Compile = function () {
            var _this = this;
            if (!WebExpressions.Utils.CustomClassHandler.PropertyRegex.test(this.MethodName)) {
                throw "Invalid method name: " + this.MethodName;
            }

            var name = this.MethodName;
            var object = this._CompileMethodCallContext();
            var args = linq(this.Arguments).Select(function (a) {
                return a.Compile();
            }).Result;

            return function (ambientContext) {
                var params = linq(args).Select(function (a) {
                    return a(ambientContext);
                }).Result;
                var o = object(ambientContext);
                return o[_this.MethodName].apply(o, params);
            };
        };

        MethodCallExpressionBase.prototype._CompileMethodCallContext = function () {
            throw "This method is abstract";
        };
        return MethodCallExpressionBase;
    })(WebExpressions.Expression);
    WebExpressions.MethodCallExpressionBase = MethodCallExpressionBase;

    var MethodCallExpression = (function (_super) {
        __extends(MethodCallExpression, _super);
        function MethodCallExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Object",
                inputType: "object"
            });

            this.Object = WebExpressions.Expression.CreateExpression(meta.Object);
        }
        MethodCallExpression.prototype._CompileMethodCallContext = function () {
            return this.Object.Compile();
        };

        MethodCallExpression.RegisteredMethods = {};
        return MethodCallExpression;
    })(MethodCallExpressionBase);
    WebExpressions.MethodCallExpression = MethodCallExpression;

    var StaticMethodCallExpression = (function (_super) {
        __extends(StaticMethodCallExpression, _super);
        function StaticMethodCallExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Class",
                inputType: "string"
            });

            this.Class = WebExpressions.Utils.CustomClassHandler.SplitNamespace(meta.Class);
        }
        StaticMethodCallExpression.prototype._CompileMethodCallContext = function () {
            var _this = this;
            return function (item) {
                return WebExpressions.Utils.CustomClassHandler.GetClass(_this.Class);
            };
        };
        return StaticMethodCallExpression;
    })(MethodCallExpressionBase);
    WebExpressions.StaticMethodCallExpression = StaticMethodCallExpression;
})(WebExpressions || (WebExpressions = {}));
