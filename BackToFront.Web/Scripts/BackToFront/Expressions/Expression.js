var __BTF;
(function (__BTF) {
    var Validation = __BTF.Validation;
    var Meta = __BTF.Meta;
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
            Expression.CreateExpression = function CreateExpression(meta) {
                switch(meta.ExpressionType) {
                    case __BTF.Meta.ExpressionWrapperType.Binary:
                        return new Expressions.BinaryExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Block:
                        return new Expressions.BlockExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Conditional:
                        return new Expressions.ConditionalExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Constant:
                        return new Expressions.ConstantExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Default:
                        return new Expressions.DefaultExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Member:
                        return new Expressions.MemberExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.MethodCall:
                        return new Expressions.MethodCallExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Parameter:
                        return new Expressions.ParameterExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Unary:
                        return new Expressions.UnaryExpression(meta);
                }
                throw "Invalid expression type";
            };
            return Expression;
        })();
        Expressions.Expression = Expression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
