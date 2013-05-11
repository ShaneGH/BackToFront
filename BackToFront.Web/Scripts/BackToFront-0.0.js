var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
if(window["__BTF"] != null) {
    throw "BackToFront is defined already";
}
var __BTF;
(function (__BTF) {
    __BTF.Initialize = function (data) {
    };
    var TestClass = (function () {
        function TestClass() { }
        TestClass.prototype.Test = function () {
            return true;
        };
        return TestClass;
    })();
    __BTF.TestClass = TestClass;    
    (function (Expressions) {
        var Expression = (function () {
            function Expression(meta) {
                this.Required(meta, "NodeType", "ExpressionType");
                this.NodeType = meta.NodeType;
                this.ExpressionType = meta.ExpressionType;
            }
            Expression.prototype.Required = function (item) {
                var properties = [];
                for (var _i = 0; _i < (arguments.length - 1); _i++) {
                    properties[_i] = arguments[_i + 1];
                }
                if(item == null) {
                    throw "Item must have a value";
                }
                var failure = linq(properties).First(function (a) {
                    return item[a] == null;
                });
                if(failure == null) {
                    throw failure + " cannot be null";
                }
            };
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
                        return new BinaryExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Block:
                        return new BlockExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Conditional:
                        return new ConditionalExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Constant:
                        return new ConstantExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Default:
                        return new DefaultExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Member:
                        return new MemberExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.MethodCall:
                        return new MethodCallExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Parameter:
                        return new ParameterExpression(meta);
                    case __BTF.Meta.ExpressionWrapperType.Unary:
                        return new UnaryExpression(meta);
                }
                throw "Invalid expression type";
            };
            return Expression;
        })();
        Expressions.Expression = Expression;        
        var BinaryExpression = (function (_super) {
            __extends(BinaryExpression, _super);
            function BinaryExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Left", "Right");
                if(!BinaryExpression.OperatorDictionary[this.NodeType]) {
                    throw "Invalid Operator";
                }
                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            }
            BinaryExpression.OperatorDictionary = [];
            BinaryExpression.prototype._Compile = function () {
                var _this = this;
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return function (namedArguments, context) {
                    return BinaryExpression.OperatorDictionary[_this.NodeType](left(namedArguments, context), right(namedArguments, context));
                };
            };
            return BinaryExpression;
        })(Expression);
        Expressions.BinaryExpression = BinaryExpression;        
        var BlockExpression = (function (_super) {
            __extends(BlockExpression, _super);
            function BlockExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Expressions");
                this.Expressions = linq(meta.Expressions).Select(function (a) {
                    return Expression.CreateExpression(a);
                }).Result;
            }
            BlockExpression.prototype._Compile = function () {
                var children = linq(this.Expressions).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (namedArguments, context) {
                    return linq(children).Each(function (a) {
                        return a(namedArguments, context);
                    });
                };
            };
            return BlockExpression;
        })(Expression);
        Expressions.BlockExpression = BlockExpression;        
        var ConditionalExpression = (function (_super) {
            __extends(ConditionalExpression, _super);
            function ConditionalExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "IfTrue", "IfFalse", "Test");
                this.IfTrue = Expression.CreateExpression(meta.IfTrue);
                this.IfFalse = Expression.CreateExpression(meta.IfFalse);
                this.Test = Expression.CreateExpression(meta.Test);
            }
            ConditionalExpression.prototype._Compile = function () {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();
                return function (namedArguments, context) {
                    return test(namedArguments, context) ? ifTrue(namedArguments, context) : ifFalse(namedArguments, context);
                };
            };
            return ConditionalExpression;
        })(Expression);
        Expressions.ConditionalExpression = ConditionalExpression;        
        var ConstantExpression = (function (_super) {
            __extends(ConstantExpression, _super);
            function ConstantExpression(meta) {
                        _super.call(this, meta);
            }
            ConstantExpression.prototype._Compile = function () {
                return function (namedArguments, context) {
                    return null;
                };
            };
            return ConstantExpression;
        })(Expression);
        Expressions.ConstantExpression = ConstantExpression;        
        var DefaultExpression = (function (_super) {
            __extends(DefaultExpression, _super);
            function DefaultExpression(meta) {
                        _super.call(this, meta);
            }
            DefaultExpression.prototype._Compile = function () {
                return function (namedArguments, context) {
                    return null;
                };
            };
            return DefaultExpression;
        })(Expression);
        Expressions.DefaultExpression = DefaultExpression;        
        var MemberExpression = (function (_super) {
            __extends(MemberExpression, _super);
            function MemberExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Expression", "MemberName", "Test");
                this.Expression = Expression.CreateExpression(meta.Expression);
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
        })(Expression);
        Expressions.MemberExpression = MemberExpression;        
        var MethodCallExpression = (function (_super) {
            __extends(MethodCallExpression, _super);
            function MethodCallExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Object", "Arguments", "MethodName", "MethodFullName");
                this.Object = Expression.CreateExpression(meta.Object);
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return Expression.CreateExpression(a);
                }).Result;
                this.MethodName = meta.MethodName;
                this.MethodFullName = meta.MethodFullName;
            }
            MethodCallExpression.prototype._Compile = function () {
                throw "Not implemented";
            };
            return MethodCallExpression;
        })(Expression);
        Expressions.MethodCallExpression = MethodCallExpression;        
        var ParameterExpression = (function (_super) {
            __extends(ParameterExpression, _super);
            function ParameterExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Name");
                this.Name = meta.Name;
            }
            ParameterExpression.prototype._Compile = function () {
                var _this = this;
                return function (namedArguments, context) {
                    return namedArguments[_this.Name];
                };
            };
            return ParameterExpression;
        })(Expression);
        Expressions.ParameterExpression = ParameterExpression;        
        var UnaryExpression = (function (_super) {
            __extends(UnaryExpression, _super);
            function UnaryExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Operand");
                this.Operand = Expression.CreateExpression(meta.Operand);
            }
            UnaryExpression.OperatorDictionary = [];
            UnaryExpression.prototype._Compile = function () {
                var _this = this;
                var operand = this.Operand.Compile();
                return function (namedArguments, context) {
                    return UnaryExpression.OperatorDictionary[_this.NodeType](operand(namedArguments, context));
                };
            };
            return UnaryExpression;
        })(Expression);
        Expressions.UnaryExpression = UnaryExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
    (function (Validation) {
        var ExpressionInvoker = (function () {
            function ExpressionInvoker(Logic, AffectedProperties) {
                this.Logic = Logic;
                this.AffectedProperties = AffectedProperties;
            }
            return ExpressionInvoker;
        })();
        Validation.ExpressionInvoker = ExpressionInvoker;        
        var ValidationContext = (function () {
            function ValidationContext() { }
            ValidationContext.prototype.Break = function () {
                return false;
            };
            return ValidationContext;
        })();
        Validation.ValidationContext = ValidationContext;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));
