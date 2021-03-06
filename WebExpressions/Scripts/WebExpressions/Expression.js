/// <reference path="../ref/linq.d.ts" />
/// <reference path="MetaClasses.ts" />
/// <reference path="Sanitizer.ts" />
/// <reference path="AssignmentExpression.ts" />
/// <reference path="BinaryExpression.ts" />
/// <reference path="BlockExpression.ts" />
/// <reference path="ConditionalExpression.ts" />
/// <reference path="ConstantExpression.ts" />
/// <reference path="DefaultExpression.ts" />
/// <reference path="InvocationExpression.ts" />
/// <reference path="MemberExpression.ts" />
/// <reference path="MethodCallExpression.ts" />
/// <reference path="NewExpression.ts" />
/// <reference path="ParameterExpression.ts" />
/// <reference path="UnaryExpression.ts" />
var WebExpressions;
(function (WebExpressions) {
    var Expression = (function () {
        function Expression(meta) {
            WebExpressions.Sanitizer.Require(meta, {
                inputName: "NodeType",
                inputConstructor: Number
            }, {
                inputName: "ExpressionType",
                inputConstructor: Number
            });

            this.NodeType = meta.NodeType;
            this.ExpressionType = meta.ExpressionType;
        }
        Expression.prototype.Compile = function () {
            if (!this._Compiled) {
                this._Compiled = this._Compile();
            }

            return this._Compiled;
        };

        // abstract
        Expression.prototype._Compile = function () {
            throw "Invalid operation";
        };

        Expression.prototype.GetAffectedProperties = function () {
            return [];
        };

        Expression.CreateExpression = function (meta) {
            if (meta.NodeType === WebExpressions.Meta.ExpressionType.Assign && meta.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Binary) {
                return new WebExpressions.AssignmentExpression(meta);
            }

            if (Expression.ExpressionConstructorDictionary[meta.ExpressionType])
                return Expression.ExpressionConstructorDictionary[meta.ExpressionType](meta);

            throw "Invalid expression type";
        };
        Expression.ExpressionConstructorDictionary = (function () {
            var dictionary = {};
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Binary] = function (meta) {
                return new WebExpressions.BinaryExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Block] = function (meta) {
                return new WebExpressions.BlockExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Conditional] = function (meta) {
                return new WebExpressions.ConditionalExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Constant] = function (meta) {
                return new WebExpressions.ConstantExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Default] = function (meta) {
                return new WebExpressions.DefaultExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Member] = function (meta) {
                return new WebExpressions.MemberExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.StaticMember] = function (meta) {
                return new WebExpressions.StaticMemberExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.MethodCall] = function (meta) {
                return new WebExpressions.MethodCallExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.StaticMethodCall] = function (meta) {
                return new WebExpressions.StaticMethodCallExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Parameter] = function (meta) {
                return new WebExpressions.ParameterExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Unary] = function (meta) {
                return new WebExpressions.UnaryExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Invocation] = function (meta) {
                return new WebExpressions.InvocationExpression(meta);
            };
            dictionary[WebExpressions.Meta.ExpressionWrapperType.New] = function (meta) {
                return new WebExpressions.NewExpression(meta);
            };

            return dictionary;
        })();
        return Expression;
    })();
    WebExpressions.Expression = Expression;
})(WebExpressions || (WebExpressions = {}));
