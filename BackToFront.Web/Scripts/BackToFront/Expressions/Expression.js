var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var E = __BTF.Expressions;
        var Validation = __BTF.Validation;
        var Meta = __BTF.Meta;
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
            Expression.CreateExpression = function CreateExpression(meta) {
                switch(meta.ExpressionType) {
                    case Meta.ExpressionWrapperType.Binary:
                        return new E.BinaryExpression(meta);
                    case Meta.ExpressionWrapperType.Block:
                        return new E.BlockExpression(meta);
                    case Meta.ExpressionWrapperType.Conditional:
                        return new E.ConditionalExpression(meta);
                    case Meta.ExpressionWrapperType.Constant:
                        return new E.ConstantExpression(meta);
                    case Meta.ExpressionWrapperType.Default:
                        return new E.DefaultExpression(meta);
                    case Meta.ExpressionWrapperType.Member:
                        return new E.MemberExpression(meta);
                    case Meta.ExpressionWrapperType.MethodCall:
                        return new E.MethodCallExpression(meta);
                    case Meta.ExpressionWrapperType.Parameter:
                        return new E.ParameterExpression(meta);
                    case Meta.ExpressionWrapperType.Unary:
                        return new E.UnaryExpression(meta);
                }
                throw "Invalid expression type";
            };
            return Expression;
        })();
        Expressions.Expression = Expression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
