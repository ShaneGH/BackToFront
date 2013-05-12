var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var Expression = (function () {
            function Expression(meta) {
                __BTF.Sanitizer.Require(meta, {
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
                if(!this._Compiled) {
                    var compiled = this._Compile();
                    this._Compiled = function (item, context) {
                        if(!context.Break()) {
                            compiled(item, context);
                        }
                    };
                }
                return this._Compiled;
            };
            Expression.prototype._Compile = function () {
                throw "Invalid operation";
            };
            Expression.prototype.GetAffectedProperties = function () {
                return [];
            };
            Expression.ExpressionConstructorDictionary = (function () {
                var dictionary = {
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Binary] = function (meta) {
                    return new __BTF.Expressions.BinaryExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Block] = function (meta) {
                    return new __BTF.Expressions.BlockExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Conditional] = function (meta) {
                    return new __BTF.Expressions.ConditionalExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Constant] = function (meta) {
                    return new __BTF.Expressions.ConstantExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Default] = function (meta) {
                    return new __BTF.Expressions.DefaultExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Member] = function (meta) {
                    return new __BTF.Expressions.MemberExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.MethodCall] = function (meta) {
                    return new __BTF.Expressions.MethodCallExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Parameter] = function (meta) {
                    return new __BTF.Expressions.ParameterExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Unary] = function (meta) {
                    return new __BTF.Expressions.UnaryExpression(meta);
                };
                dictionary[__BTF.Meta.ExpressionWrapperType.Invocation] = function (meta) {
                    return new __BTF.Expressions.InvocationExpression(meta);
                };
                return dictionary;
            })();
            Expression.CreateExpression = function CreateExpression(meta) {
                if(Expression.ExpressionConstructorDictionary[meta.ExpressionType]) {
                    return Expression.ExpressionConstructorDictionary[meta.ExpressionType](meta);
                }
                throw "Invalid expression type";
            };
            return Expression;
        })();
        Expressions.Expression = Expression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
