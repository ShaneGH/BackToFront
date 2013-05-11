var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
if(window["__BTF"] != null) {
    throw "BackToFront is defined already";
}
var M = __BTF.Meta;
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
                for(var i = 0, ii = properties.length; i < ii; i++) {
                    if(item[properties[i]] == null) {
                        throw properties[i] + " cannot be null";
                    }
                }
            };
            Expression.prototype.Compile = function () {
                throw "Invalid operation";
            };
            Expression.CreateExpression = function CreateExpression(meta) {
                switch(meta.ExpressionType) {
                    case M.ExpressionWrapperType.Binary:
                        return new BinaryExpression(meta);
                    case M.ExpressionWrapperType.Block:
                        return new BlockExpression(meta);
                    case M.ExpressionWrapperType.Conditional:
                        return new ConditionalExpression(meta);
                    case M.ExpressionWrapperType.Constant:
                        return new ConstantExpression(meta);
                    case M.ExpressionWrapperType.Default:
                        return new DefaultExpression(meta);
                    case M.ExpressionWrapperType.Member:
                        return new MemberExpression(meta);
                    case M.ExpressionWrapperType.MethodCall:
                        return new MethodCallExpression(meta);
                    case M.ExpressionWrapperType.Parameter:
                        return new ParameterExpression(meta);
                    case M.ExpressionWrapperType.Unary:
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
                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            }
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
            return ConditionalExpression;
        })(Expression);
        Expressions.ConditionalExpression = ConditionalExpression;        
        var ConstantExpression = (function (_super) {
            __extends(ConstantExpression, _super);
            function ConstantExpression(meta) {
                        _super.call(this, meta);
            }
            return ConstantExpression;
        })(Expression);
        Expressions.ConstantExpression = ConstantExpression;        
        var DefaultExpression = (function (_super) {
            __extends(DefaultExpression, _super);
            function DefaultExpression(meta) {
                        _super.call(this, meta);
            }
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
            return ParameterExpression;
        })(Expression);
        Expressions.ParameterExpression = ParameterExpression;        
        var UnaryExpression = (function (_super) {
            __extends(UnaryExpression, _super);
            function UnaryExpression(meta) {
                        _super.call(this, meta);
                this.Required(meta, "Expression", "MemberName", "Test");
                this.Operand = Expression.CreateExpression(meta.Operand);
            }
            return UnaryExpression;
        })(Expression);
        Expressions.UnaryExpression = UnaryExpression;        
        var ExpressionInvoker = (function () {
            function ExpressionInvoker() { }
            return ExpressionInvoker;
        })();
        Expressions.ExpressionInvoker = ExpressionInvoker;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));
