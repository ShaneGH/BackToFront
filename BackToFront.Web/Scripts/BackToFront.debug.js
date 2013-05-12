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
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    var Validation = __BTF.Validation;
    var Meta = __BTF.Meta;
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
})(__BTF || (__BTF = {}));


var __BTF;
(function (__BTF) {
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



/*
    Basics:
        create a linq object using:
            var x = new Linq([...]); --or--
            var x = linq([...]);

        each linq function returns a linq object which has only one property: "Result"

        lambdas can be passed in in three ways:
            var x = linq([...]).Select("a => a.property"); --or--
            var x = linq([...]).Select("(a) => a.property"); --or--
            var x = linq([...]).Select("a => { return a.property; }"); --or--
            var x = linq([...]).Select(function(a) { return a.property; });
        
    Common mistakes:
        --the return value is not an array, the array is the Result property
        var myArray = linq([...]).Select("a => a.property"); --SHOULD BE--
        var myArray = linq([...]).Select("a => a.property").Result;
        
        --in this case the system sees a bracketed lambda function and compiles it incorrectly
        var myArray = linq([...]).Select("a => { prop: a.property }").Result; --SHOULD BE--
        var myArray = linq([...]).Select("a => { return { prop: a.property }; }").Result;
        
        --a bracketed lambda must have a return statement
        var myArray = linq([...]).Select("a => { a.property }").Result; --SHOULD BE--
        var myArray = linq([...]).Select("a => { return a.property }").Result;

    Functions:
        Aggregate("(a, b) => DO AGGREGATION HERE")
        Aggregate(), equivelant to Aggregate("(a,b) => linq(a).Union(b).Result;")
        Any("a => a === something")
        Contains(item)
        Distinct()
        Each("a => alert(a)")
        First("a => a === something")
        GroupBy("a => a.property")
        Last("a => a === something")
        Max("a => a.property")
        Min("a => a.property")
        OrderBy("a => a.property")
        OrderByDecending("a => a.property")
        ReverseEach("a => alert(a)"), the same as Each but in reverse
        Select("a => a.property")
        Skip(10)
        Take(10)
        Union([...])
        Where("a => a === something")

    Extending: 
        --custom linq functions can be added to "linq.fn"
        --use linq.utils.convertLambda to convert a lambda string (or function) to a function
        linq.fn.AnotherSelect = function (selectFunction) {

            selectFunction = linq.utils.convertLambda(selectFunction);
            var output = linq();
            for (var i = 0, ii = this.Result.length; i < ii; i++) {
                output.Result[i] = selectFunction(this.Result[i]);
            }

            return output;
        };        
 */

(function (globalContext) {

    // helpers to speed up looping
    var ii, jj, kk, ll;

    var regex_csv = /^[\w\d_]+(,([\w\d_]+))*$/;
    var regex_bracketed = /^[\s\t\r\n]*\{.*\}[\s\t\r\n]*$/;
    var regex_whitespace = /[\s\t\r\n]/g;

    var defaultAggregateFunction = function (a, b) {
        return linq(a).Union(b).Result;
    };

    globalContext.Linq = function (array) {
        this.Result = linq.utils.transformToArray(array);
    };

    //shortcut
    globalContext.linq = function (array) {
        return new globalContext.Linq(array ? array : []);
    }

    // local convenience var
    var linq = globalContext.linq;
    linq.utils = {
        //Called at the beginning of each linq function. If a function is passed in, returns the function
        //if a lambda expression string is passed in, attempts to convert it to a function.
        convertLambda: function (funcOrString) {
            //Validate input
            if (!funcOrString || funcOrString.constructor === Function) {
                return funcOrString;
            } else if (funcOrString.constructor !== String) {
                throw "Invalid linq function";
            } else if (funcOrString.indexOf("=>") === -1) {
                throw "\"=>\" expression not found";
            }

            //split expression
            var rhs = funcOrString.substring(funcOrString.indexOf("=>") + 2);
            var lhs = funcOrString.substring(0, funcOrString.indexOf("=>")).replace(regex_whitespace, "");

            //remove brackets
            if (lhs.indexOf("(") === 0) {
                lhs = lhs.substring(1);
                if (lhs.indexOf(")") === lhs.length - 1) {
                    lhs = lhs.substring(0, lhs.length - 1);
                } else {
                    throw "Invalid left hand side";
                }
            }

            //validate lhs and rhs
            if (lhs.length < 1 || !regex_csv.test(lhs)) {
                throw "Invalid left hand side";
            } else if (rhs.length < 1) {
                throw "Invalid right hand side";
            }

            if (regex_bracketed.test(rhs)) {
                return new Function(lhs, rhs);
            } else {
                return new Function(lhs, "return " + rhs + ";");
            }
        },
        transformToArray: function (arrayOrLinq) {
            if (arrayOrLinq.constructor === Array) {
                return arrayOrLinq;
            } else if (arrayOrLinq.constructor === globalContext.Linq) {
                return arrayOrLinq.Result;
            } else if (arrayOrLinq.constructor === NodeList) {
                return arrayOrLinq;
            } else {
                throw "Input must be array or Linq object";
            }
        },
        transformToLinq: function (arrayOrLinq) {
            if (arrayOrLinq.constructor === globalContext.Linq) {
                return arrayOrLinq;
            } else if (arrayOrLinq.constructor === Array) {
                return linq(arrayOrLinq);
            } else {
                throw "Input must be array or Linq object";
            }
        }
    };

    // encourage extenstions
    linq.fn = globalContext.Linq.prototype;

    linq.fn.Select = function (selectFunction) {

        selectFunction = linq.utils.convertLambda(selectFunction);
        var output = linq();
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            output.Result[i] = selectFunction(this.Result[i]);
        }

        return output;
    };

    // helper for max and min functions
    var prepareMaxMin = function (linqArray, selectFunction) {

        selectFunction = linq.utils.convertLambda(selectFunction);
        var ignoreNulls = function (i) { return i != null && i.constructor === Number };
        var tmp = (selectFunction ? linqArray.Select(selectFunction) : linqArray).Where(ignoreNulls).Result;

        if (tmp.length === 0) {
            throw "Sequence contains no elements.";
        }

        return tmp;
    }

    linq.fn.Max = function (selectFunction) {

        //trick max into accepting array
        return Math.max.apply(Math, prepareMaxMin(this, selectFunction));
    };

    linq.fn.Min = function (selectFunction) {

        //trick min into accepting array
        return Math.min.apply(Math, prepareMaxMin(this, selectFunction));
    };

    linq.fn.Aggregate = function (aggFunction) {

        if (aggFunction === undefined) {
            aggFunction = defaultAggregateFunction;
        } else {
            aggFunction = linq.utils.convertLambda(aggFunction);
        }

        var output = linq();
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            output.Result = aggFunction(output.Result, this.Result[i]);
        }

        return output;
    };

    linq.fn.Union = function (array) {

        array = linq.utils.transformToArray(array);

        var tester = linq();
        var i;
        for (i = 0, ii = this.Result.length; i < ii; i++) {
            tester.Result[i] = this.Result[i];
        }

        for (var j = 0, jj = array.length; j < jj; j++) {
            if (!tester.Contains(array[j])) {
                tester.Result[i + j] = array[j];
            } else {
                i--;
            }
        }

        return tester;
    };

    linq.fn.Contains = function (element) {

        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            if (this.Result[i] === element) {
                return true;
            }
        }

        return false;
    };

    linq.fn.Any = function (whereFunction) {
        if (!whereFunction) {
            return this.Result.length > 0;
        }

        whereFunction = linq.utils.convertLambda(whereFunction);
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            if (whereFunction(this.Result[i])) {
                return true;
            }
        }

        return false;
    };

    linq.fn.Where = function (whereFunction) {

        whereFunction = linq.utils.convertLambda(whereFunction);
        var skipped = 0;
        var output = linq();
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            if (whereFunction(this.Result[i])) {
                output.Result[i - skipped] = this.Result[i];
            } else {
                skipped++;
            }
        }

        return output;
    };

    linq.fn.First = function (whereFunction) {

        if (!whereFunction) {
            return this.Result[0];
        } else {
            whereFunction = linq.utils.convertLambda(whereFunction);
            for (var i = 0, ii = this.Result.length; i < ii; i++) {
                if (whereFunction(this.Result[i])) {
                    return this.Result[i];
                }
            }
        }

        return null;
    };

    linq.fn.Last = function (whereFunction) {

        if (!whereFunction) {
            return this.Result[this.Result.length - 1];
        } else {
            whereFunction = linq.utils.convertLambda(whereFunction);
            for (var i = this.Result.length - 1; i >= 0; i--) {
                if (whereFunction(this.Result[i])) {
                    return this.Result[i];
                }
            }
        }

        return null;
    };

    linq.fn.GroupBy = function (selectFunction) {

        selectFunction = linq.utils.convertLambda(selectFunction);
        var output = linq();
        var key = null;
        var where = null;
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            key = selectFunction(this.Result[i]);
            where = output.Where(function (a) { return a.key === key; });
            if (where.Any()) {
                where.Result[0].values.push(this.Result[i]);
            } else {
                output.Result.push({ key: key, values: [this.Result[i]] });
            }
        }

        return output;
    };

    linq.fn.OrderBy = function (selectFunction) {

        selectFunction = linq.utils.convertLambda(selectFunction);
        //special constructor ref for Number
        var _number = new Object();

        var output = linq();
        var worker = [];
        var unEdited = [];
        var type = null;
        for (var i = 0, ii = this.Result.length; i < ii; i++) {

            var selected = selectFunction ? selectFunction(this.Result[i]) : this.Result[i];
            worker[i] = { key: i, value: selected };
            unEdited[i] = { key: i, value: this.Result[i] };

            if (selected && type == null) {
                type = typeof selected === "number" ? _number : selected.constructor;
            } else if (selected && type !== (typeof selected === "number" ? _number : selected.constructor)) {
                throw "Invalid comparison type";
            }
        }

        var sortFunc = function (i, j) { if (i.value < j.value) return -1; else if (i.value > j.value) return 1; else return 0; };
        worker.sort(sortFunc);
        for (var i = 0, ii = worker.length; i < ii; i++) {
            output.Result[i] = linq(unEdited).First(function (u) { return u.key === worker[i].key }).value;
        }

        return output;
    };

    linq.fn.OrderByDecending = function (selectFunction) {
        var output = this.OrderBy(linq.utils.convertLambda(selectFunction));
        output.Result.reverse();
        return output;
    };

    linq.fn.Skip = function (skip) {

        var output = linq();
        for (var i = skip, ii = this.Result.length; i < ii; i++) {
            output.Result[i - skip] = this.Result[i];
        }

        return output;
    };

    linq.fn.Take = function (take) {

        var output = linq();
        var last = take < this.Result.length ? take : this.Result.length;

        for (var i = 0; i < last; i++) {
            output.Result[i] = this.Result[i];
        }

        return output;
    };

    linq.fn.Distinct = function () {

        var skipped = 0;
        var output = linq();
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            if (!output.Contains(this.Result[i])) {
                output.Result[i - skipped] = this.Result[i];
            } else {
                skipped++;
            }
        }

        return output;
    };

    linq.fn.Each = function (work) {

        work = linq.utils.convertLambda(work);
        for (var i = 0, ii = this.Result.length; i < ii; i++) {
            work(this.Result[i], i);
        }

        return this;
    };

    linq.fn.ReverseEach = function (work) {

        work = linq.utils.convertLambda(work);
        for (var i = this.Result.length - 1; i >= 0; i--) {
            work(this.Result[i], i);
        }

        return this;
    };
})(window);

