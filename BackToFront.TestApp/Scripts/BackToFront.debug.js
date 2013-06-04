
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

var __BTF;
(function (__BTF) {
    (function (Meta) {
        (function (ExpressionWrapperType) {
            ExpressionWrapperType._map = [];
            ExpressionWrapperType.Binary = 1;
            ExpressionWrapperType.Constant = 2;
            ExpressionWrapperType.Member = 3;
            ExpressionWrapperType.MethodCall = 4;
            ExpressionWrapperType.Parameter = 5;
            ExpressionWrapperType.Unary = 6;
            ExpressionWrapperType.Default = 7;
            ExpressionWrapperType.Block = 8;
            ExpressionWrapperType.Conditional = 9;
            ExpressionWrapperType.Invocation = 10;
            ExpressionWrapperType.New = 11;
        })(Meta.ExpressionWrapperType || (Meta.ExpressionWrapperType = {}));
        var ExpressionWrapperType = Meta.ExpressionWrapperType;
        (function (ExpressionType) {
            ExpressionType._map = [];
            ExpressionType.Add = 0;
            ExpressionType.AddChecked = 1;
            ExpressionType.And = 2;
            ExpressionType.AndAlso = 3;
            ExpressionType.ArrayLength = 4;
            ExpressionType.ArrayIndex = 5;
            ExpressionType.Call = 6;
            ExpressionType.Coalesce = 7;
            ExpressionType.Conditional = 8;
            ExpressionType.Constant = 9;
            ExpressionType.Convert = 10;
            ExpressionType.ConvertChecked = 11;
            ExpressionType.Divide = 12;
            ExpressionType.Equal = 13;
            ExpressionType.ExclusiveOr = 14;
            ExpressionType.GreaterThan = 15;
            ExpressionType.GreaterThanOrEqual = 16;
            ExpressionType.Invoke = 17;
            ExpressionType.Lambda = 18;
            ExpressionType.LeftShift = 19;
            ExpressionType.LessThan = 20;
            ExpressionType.LessThanOrEqual = 21;
            ExpressionType.ListInit = 22;
            ExpressionType.MemberAccess = 23;
            ExpressionType.MemberInit = 24;
            ExpressionType.Modulo = 25;
            ExpressionType.Multiply = 26;
            ExpressionType.MultiplyChecked = 27;
            ExpressionType.Negate = 28;
            ExpressionType.UnaryPlus = 29;
            ExpressionType.NegateChecked = 30;
            ExpressionType.New = 31;
            ExpressionType.NewArrayInit = 32;
            ExpressionType.NewArrayBounds = 33;
            ExpressionType.Not = 34;
            ExpressionType.NotEqual = 35;
            ExpressionType.Or = 36;
            ExpressionType.OrElse = 37;
            ExpressionType.Parameter = 38;
            ExpressionType.Power = 39;
            ExpressionType.Quote = 40;
            ExpressionType.RightShift = 41;
            ExpressionType.Subtract = 42;
            ExpressionType.SubtractChecked = 43;
            ExpressionType.TypeAs = 44;
            ExpressionType.TypeIs = 45;
            ExpressionType.Assign = 46;
            ExpressionType.Block = 47;
            ExpressionType.DebugInfo = 48;
            ExpressionType.Decrement = 49;
            ExpressionType.Dynamic = 50;
            ExpressionType.Default = 51;
            ExpressionType.Extension = 52;
            ExpressionType.Goto = 53;
            ExpressionType.Increment = 54;
            ExpressionType.Index = 55;
            ExpressionType.Label = 56;
            ExpressionType.RuntimeVariables = 57;
            ExpressionType.Loop = 58;
            ExpressionType.Switch = 59;
            ExpressionType.Throw = 60;
            ExpressionType.Try = 61;
            ExpressionType.Unbox = 62;
            ExpressionType.AddAssign = 63;
            ExpressionType.AndAssign = 64;
            ExpressionType.DivideAssign = 65;
            ExpressionType.ExclusiveOrAssign = 66;
            ExpressionType.LeftShiftAssign = 67;
            ExpressionType.ModuloAssign = 68;
            ExpressionType.MultiplyAssign = 69;
            ExpressionType.OrAssign = 70;
            ExpressionType.PowerAssign = 71;
            ExpressionType.RightShiftAssign = 72;
            ExpressionType.SubtractAssign = 73;
            ExpressionType.AddAssignChecked = 74;
            ExpressionType.MultiplyAssignChecked = 75;
            ExpressionType.SubtractAssignChecked = 76;
            ExpressionType.PreIncrementAssign = 77;
            ExpressionType.PreDecrementAssign = 78;
            ExpressionType.PostIncrementAssign = 79;
            ExpressionType.PostDecrementAssign = 80;
            ExpressionType.TypeEqual = 81;
            ExpressionType.OnesComplement = 82;
            ExpressionType.IsTrue = 83;
            ExpressionType.IsFalse = 84;
        })(Meta.ExpressionType || (Meta.ExpressionType = {}));
        var ExpressionType = Meta.ExpressionType;
    })(__BTF.Meta || (__BTF.Meta = {}));
    var Meta = __BTF.Meta;
})(__BTF || (__BTF = {}));


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
                    this._Compiled = this._Compile();
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
                dictionary[__BTF.Meta.ExpressionWrapperType.New] = function (meta) {
                    return new __BTF.Expressions.NewExpression(meta);
                };
                return dictionary;
            })();
            Expression.CreateExpression = function CreateExpression(meta) {
                if(meta.NodeType === __BTF.Meta.ExpressionType.Assign && meta.ExpressionType === __BTF.Meta.ExpressionWrapperType.Binary) {
                    return new __BTF.Expressions.AssignmentExpression(meta);
                }
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


var __BTF;
(function (__BTF) {
    (function (Validation) {
        var Validator = (function () {
            function Validator(rules, Entity) {
                this.Entity = Entity;
                this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
            }
            Validator.CreateRule = function CreateRule(rule) {
                var r = __BTF.Expressions.Expression.CreateExpression(rule.Expression).Compile();
                return {
                    RequiredForValidationNames: rule.RequiredForValidationNames,
                    ValidationSubjectNames: rule.ValidationSubjectNames,
                    Validate: function (entity, breakOnFirstError) {
                        if (typeof breakOnFirstError === "undefined") { breakOnFirstError = false; }
                        var context = {
                        };
                        context[rule.EntityParameter] = entity;
                        context[rule.ContextParameter] = {
                            Violations: [],
                            BreakOnFirstError: breakOnFirstError,
                            Mocks: [],
                            Dependencies: {
                            }
                        };
                        r(context);
                        return context[rule.ContextParameter].Violations;
                    }
                };
            };
            Validator.prototype.Validate = function (propertyName, breakOnFirstError) {
                if (typeof breakOnFirstError === "undefined") { breakOnFirstError = false; }
                var entity = this.GetEntity();
                return linq(this.Rules).Where(function (rule) {
                    return rule.ValidationSubjectNames.indexOf(propertyName) !== -1;
                }).Select(function (rule) {
                    return rule.Validate(entity, breakOnFirstError);
                }).Aggregate().Result;
            };
            Validator.prototype.GetEntity = function () {
                throw "Invalid operation, this method is abstract";
            };
            return Validator;
        })();
        Validation.Validator = Validator;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));


var __BTF;
(function (__BTF) {
    __BTF.Initialize = function (data) {
    };
    var Sanitizer = (function () {
        function Sanitizer() { }
        Sanitizer.Require = function Require(item) {
            var properties = [];
            for (var _i = 0; _i < (arguments.length - 1); _i++) {
                properties[_i] = arguments[_i + 1];
            }
            if(item == null) {
                throw "Item must have a value";
            }
            for(var i = 0, ii = properties.length; i < ii; i++) {
                var prop = properties[i];
                if(prop.allowNull && item[prop.inputName] == null) {
                    return;
                }
                if(!prop.allowNull && item[prop.inputName] == null) {
                    throw "Property " + prop.inputName + " cannot be null";
                }
                if(prop.inputType && typeof item[prop.inputName] !== prop.inputType) {
                    throw "Property " + prop.inputName + " must be of type " + prop.inputType;
                }
                if(prop.inputConstructor && item[prop.inputName].constructor !== prop.inputConstructor) {
                    throw {
                        message: "Property " + prop.inputName + " must be of a give type",
                        type: prop.inputConstructor
                    };
                }
            }
        };
        return Sanitizer;
    })();
    __BTF.Sanitizer = Sanitizer;    
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
        var AssignmentExpression = (function (_super) {
            __extends(AssignmentExpression, _super);
            function AssignmentExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });
                switch(meta.Left.ExpressionType) {
                    case __BTF.Meta.ExpressionWrapperType.Parameter:
                        this.Left = null;
                        this.LeftProperty = (meta.Left).Name;
                        break;
                    case __BTF.Meta.ExpressionWrapperType.Member:
                        this.Left = __BTF.Expressions.Expression.CreateExpression((meta.Left).Expression);
                        this.LeftProperty = (meta.Left).MemberName;
                        break;
                    default:
                        throw "The left hand side of an assignment must be a parameter or a member";
                }
                this.Right = __BTF.Expressions.Expression.CreateExpression(meta.Right);
            }
            AssignmentExpression.prototype._Compile = function () {
                var _this = this;
                var left = this.Left ? this.Left.Compile() : function (context) {
                    return context;
                };
                var right = this.Right.Compile();
                return function (ambientContext) {
                    return left(ambientContext)[_this.LeftProperty] = right(ambientContext);
                };
            };
            return AssignmentExpression;
        })(Expressions.Expression);
        Expressions.AssignmentExpression = AssignmentExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
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
        var BinaryExpression = (function (_super) {
            __extends(BinaryExpression, _super);
            function BinaryExpression(meta) {
                        _super.call(this, meta);
                if(!BinaryExpression.OperatorDictionary[this.NodeType]) {
                    throw "##" + "Invalid Operator";
                }
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });
                this.Left = Expressions.Expression.CreateExpression(meta.Left);
                this.Right = Expressions.Expression.CreateExpression(meta.Right);
            }
            BinaryExpression.OperatorDictionary = (function () {
                var output = [];
                output[__BTF.Meta.ExpressionType.Add] = function (left, right) {
                    return left + right;
                };
                output[__BTF.Meta.ExpressionType.AndAlso] = function (left, right) {
                    return left && right;
                };
                output[__BTF.Meta.ExpressionType.Divide] = function (left, right) {
                    return left / right;
                };
                output[__BTF.Meta.ExpressionType.Equal] = function (left, right) {
                    return left === right;
                };
                output[__BTF.Meta.ExpressionType.GreaterThan] = function (left, right) {
                    return left > right;
                };
                output[__BTF.Meta.ExpressionType.GreaterThanOrEqual] = function (left, right) {
                    return left >= right;
                };
                output[__BTF.Meta.ExpressionType.LessThan] = function (left, right) {
                    return left < right;
                };
                output[__BTF.Meta.ExpressionType.LessThanOrEqual] = function (left, right) {
                    return left <= right;
                };
                output[__BTF.Meta.ExpressionType.Multiply] = function (left, right) {
                    return left * right;
                };
                output[__BTF.Meta.ExpressionType.OrElse] = function (left, right) {
                    return left || right;
                };
                output[__BTF.Meta.ExpressionType.Subtract] = function (left, right) {
                    return left - right;
                };
                return output;
            })();
            BinaryExpression.prototype._Compile = function () {
                var _this = this;
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return function (ambientContext) {
                    return BinaryExpression.OperatorDictionary[_this.NodeType](left(ambientContext), right(ambientContext));
                };
            };
            return BinaryExpression;
        })(Expressions.Expression);
        Expressions.BinaryExpression = BinaryExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var BlockExpression = (function (_super) {
            __extends(BlockExpression, _super);
            function BlockExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expressions",
                    inputConstructor: Array
                });
                this.Expressions = linq(meta.Expressions).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
            }
            BlockExpression.prototype._Compile = function () {
                var children = linq(this.Expressions).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    return linq(children).Each(function (a) {
                        return a(ambientContext);
                    });
                };
            };
            return BlockExpression;
        })(Expressions.Expression);
        Expressions.BlockExpression = BlockExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var ConditionalExpression = (function (_super) {
            __extends(ConditionalExpression, _super);
            function ConditionalExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "IfTrue",
                    inputType: "object"
                }, {
                    inputName: "IfFalse",
                    inputType: "object"
                }, {
                    inputName: "Test",
                    inputType: "object"
                });
                this.IfTrue = Expressions.Expression.CreateExpression(meta.IfTrue);
                this.IfFalse = Expressions.Expression.CreateExpression(meta.IfFalse);
                this.Test = Expressions.Expression.CreateExpression(meta.Test);
            }
            ConditionalExpression.prototype._Compile = function () {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();
                return function (ambientContext) {
                    return test(ambientContext) ? ifTrue(ambientContext) : ifFalse(ambientContext);
                };
            };
            return ConditionalExpression;
        })(Expressions.Expression);
        Expressions.ConditionalExpression = ConditionalExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var ConstantExpression = (function (_super) {
            __extends(ConstantExpression, _super);
            function ConstantExpression(meta) {
                        _super.call(this, meta);
                this.Value = meta.Value;
            }
            ConstantExpression.prototype._Compile = function () {
                var _this = this;
                return function (ambientContext) {
                    return _this.Value;
                };
            };
            return ConstantExpression;
        })(Expressions.Expression);
        Expressions.ConstantExpression = ConstantExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var MethodCallExpression = (function (_super) {
            __extends(MethodCallExpression, _super);
            function MethodCallExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Object",
                    inputType: "object"
                }, {
                    inputName: "Arguments",
                    inputConstructor: Array
                }, {
                    inputName: "MethodName",
                    inputConstructor: String
                }, {
                    inputName: "MethodFullName",
                    inputConstructor: String
                });
                this.Object = Expressions.Expression.CreateExpression(meta.Object);
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
                this.MethodName = meta.MethodName;
                this.MethodFullName = meta.MethodFullName;
            }
            MethodCallExpression.prototype._Compile = function () {
                var name = this.MethodName;
                if(!__BTF.Expressions.MemberExpression.PropertyRegex.test(name)) {
                    throw "Invalid method name: " + name;
                }
                var object = this.Object.Compile();
                var args = linq(this.Arguments).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    var o = object(ambientContext);
                    var params = linq(args).Select(function (a) {
                        return a(ambientContext);
                    }).Result;
                    return o[name].apply(o, params);
                };
            };
            return MethodCallExpression;
        })(Expressions.Expression);
        Expressions.MethodCallExpression = MethodCallExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var DefaultExpression = (function (_super) {
            __extends(DefaultExpression, _super);
            function DefaultExpression(meta) {
                        _super.call(this, meta);
            }
            DefaultExpression.prototype._Compile = function () {
                return function (ambientContext) {
                    return null;
                };
            };
            return DefaultExpression;
        })(Expressions.Expression);
        Expressions.DefaultExpression = DefaultExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var InvocationExpression = (function (_super) {
            __extends(InvocationExpression, _super);
            function InvocationExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object"
                }, {
                    inputName: "Arguments",
                    inputConstructor: Array
                });
                this.Expression = Expressions.Expression.CreateExpression(meta.Expression);
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
            }
            InvocationExpression.prototype._Compile = function () {
                var expresion = this.Expression.Compile();
                var args = linq(this.Arguments).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    var e = expresion(ambientContext);
                    var params = linq(args).Select(function (a) {
                        return a(ambientContext);
                    }).Result;
                    return e.apply(ambientContext, params);
                };
            };
            return InvocationExpression;
        })(Expressions.Expression);
        Expressions.InvocationExpression = InvocationExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var MemberExpression = (function (_super) {
            __extends(MemberExpression, _super);
            function MemberExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object"
                }, {
                    inputName: "MemberName",
                    inputConstructor: String
                });
                this.Expression = Expressions.Expression.CreateExpression(meta.Expression);
                this.MemberName = meta.MemberName;
            }
            MemberExpression.PropertyRegex = new RegExp("^[a-zA-Z][a-zA-Z0-9]*$");
            MemberExpression.prototype._Compile = function () {
                if(!MemberExpression.PropertyRegex.test(this.MemberName)) {
                    throw "Invalid property name: " + this.MemberName;
                }
                var name = this.MemberName;
                var expression = this.Expression.Compile();
                return function (ambientContext) {
                    return expression(ambientContext)[name];
                };
            };
            return MemberExpression;
        })(Expressions.Expression);
        Expressions.MemberExpression = MemberExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var MethodCallExpression = (function (_super) {
            __extends(MethodCallExpression, _super);
            function MethodCallExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Object",
                    inputType: "object"
                }, {
                    inputName: "Arguments",
                    inputConstructor: Array
                }, {
                    inputName: "MethodName",
                    inputConstructor: String
                }, {
                    inputName: "MethodFullName",
                    inputConstructor: String
                });
                this.Object = Expressions.Expression.CreateExpression(meta.Object);
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
                this.MethodName = meta.MethodName;
                this.MethodFullName = meta.MethodFullName;
            }
            MethodCallExpression.prototype._Compile = function () {
                var name = this.MethodName;
                if(!__BTF.Expressions.MemberExpression.PropertyRegex.test(name)) {
                    throw "Invalid method name: " + name;
                }
                var object = this.Object.Compile();
                var args = linq(this.Arguments).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    var o = object(ambientContext);
                    var params = linq(args).Select(function (a) {
                        return a(ambientContext);
                    }).Result;
                    return o[name].apply(o, params);
                };
            };
            return MethodCallExpression;
        })(Expressions.Expression);
        Expressions.MethodCallExpression = MethodCallExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var NewExpression = (function (_super) {
            __extends(NewExpression, _super);
            function NewExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Arguments",
                    inputConstructor: Array
                }, {
                    inputName: "IsAnonymous",
                    inputConstructor: Boolean
                }, {
                    inputName: "Type",
                    inputConstructor: String
                });
                if(meta.IsAnonymous) {
                    __BTF.Sanitizer.Require(meta, {
                        inputName: "Members",
                        inputConstructor: Array
                    });
                    if(meta.Members.length !== meta.Arguments.length) {
                        throw "If members are defined, each must have a corresponding argument";
                    }
                }
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return __BTF.Expressions.Expression.CreateExpression(a);
                }).Result;
                this.Members = meta.Members;
                this.Type = meta.Type;
                this.IsAnonymous = meta.IsAnonymous;
            }
            NewExpression.prototype._Compile = function () {
                var _this = this;
                var args = linq(this.Arguments).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    var params = linq(args).Select(function (a) {
                        return a(ambientContext);
                    }).Result;
                    if(_this.IsAnonymous) {
                        return _this.ConstructAnonymous(params);
                    } else if(NewExpression.RegisteredTypes[_this.Type]) {
                        return _this.Construct(NewExpression.RegisteredTypes[_this.Type], params);
                    } else {
                        return {
                        };
                    }
                };
            };
            NewExpression.prototype.ConstructAnonymous = function (params) {
                var obj = {
                };
                for(var i = 0, ii = this.Members.length; i < ii; i++) {
                    obj[this.Members[i]] = params[i];
                }
                return obj;
            };
            NewExpression.prototype.Construct = function (constr, args) {
                var obj = Object.create(constr.prototype);
                var result = constr.apply(obj, args);
                return typeof result === 'object' ? result : obj;
            };
            NewExpression.RegisteredTypes = {
            };
            return NewExpression;
        })(Expressions.Expression);
        Expressions.NewExpression = NewExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var ParameterExpression = (function (_super) {
            __extends(ParameterExpression, _super);
            function ParameterExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Name",
                    inputConstructor: String
                });
                this.Name = meta.Name;
            }
            ParameterExpression.prototype._Compile = function () {
                var _this = this;
                return function (ambientContext) {
                    return ambientContext[_this.Name];
                };
            };
            return ParameterExpression;
        })(Expressions.Expression);
        Expressions.ParameterExpression = ParameterExpression;        
    })(__BTF.Expressions || (__BTF.Expressions = {}));
    var Expressions = __BTF.Expressions;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Expressions) {
        var UnaryExpression = (function (_super) {
            __extends(UnaryExpression, _super);
            function UnaryExpression(meta) {
                        _super.call(this, meta);
                __BTF.Sanitizer.Require(meta, {
                    inputName: "Operand",
                    inputType: "object"
                });
                this.Operand = Expressions.Expression.CreateExpression(meta.Operand);
            }
            UnaryExpression.OperatorDictionary = (function () {
                var output = [];
                output[__BTF.Meta.ExpressionType.Convert] = function (operand) {
                    return operand;
                };
                output[__BTF.Meta.ExpressionType.Not] = function (operand) {
                    return !operand;
                };
                return output;
            })();
            UnaryExpression.prototype._Compile = function () {
                var _this = this;
                var operand = this.Operand.Compile();
                return function (ambientContext) {
                    return UnaryExpression.OperatorDictionary[_this.NodeType](operand(ambientContext));
                };
            };
            return UnaryExpression;
        })(Expressions.Expression);
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
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));


var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Validation) {
        var JQueryValidator = (function (_super) {
            __extends(JQueryValidator, _super);
            function JQueryValidator(rules, entity, Context) {
                        _super.call(this, rules, entity);
                this.Context = Context;
                JQueryValidator.Setup();
            }
            JQueryValidator.prototype.GetEntity = function () {
                var entity = {
                };
                var allNames = linq(this.Rules).Select(function (r) {
                    return linq(r.RequiredForValidationNames || []).Union(r.ValidationSubjectNames || []).Result;
                }).Aggregate().Result;
                for(var j = 0, jj = allNames.length; j < jj; j++) {
                    var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);
                    if(item.attr("type") === "checkbox") {
                        entity[allNames[j]] = item.is(":checked");
                    } else {
                        entity[allNames[j]] = item.val();
                        if(item.attr("data-val-number")) {
                            entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ? parseFloat(entity[allNames[j]]) : parseInt(entity[allNames[j]]);
                        }
                    }
                }
                return entity;
            };
            JQueryValidator.Registered = [];
            JQueryValidator.ValidatorName = "backtofront";
            JQueryValidator.RegisterRule = function RegisterRule(rule) {
                __BTF.Sanitizer.Require(rule, {
                    inputName: "Rules",
                    inputConstructor: Array
                }, {
                    inputName: "Entity",
                    inputConstructor: String
                });
                JQueryValidator.Registered.push(new JQueryValidator(rule.Rules, rule.Entity));
            };
            JQueryValidator.Setup = function Setup() {
                if(!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }
                if(jQuery.validator.methods[JQueryValidator.ValidatorName]) {
                    return;
                }
                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, "XXX");
            };
            JQueryValidator.Validate = function Validate(value, element) {
                var params = [];
                for (var _i = 0; _i < (arguments.length - 2); _i++) {
                    params[_i] = arguments[_i + 2];
                }
                debugger;

                var results = linq(JQueryValidator.Registered).Select(function (a) {
                    return a.Validate($(element).attr("name"), false);
                }).Aggregate();
                return results.Result.length === 0;
            };
            return JQueryValidator;
        })(__BTF.Validation.Validator);
        Validation.JQueryValidator = JQueryValidator;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));


