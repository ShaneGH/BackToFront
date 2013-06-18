(function () {
    (function () {
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

        var WebExpressions;
        (function (WebExpressions) {
            (function (Utils) {
                var KeyValuePair = (function () {
                    function KeyValuePair(Key, Value) {
                        this.Key = Key;
                        this.Value = Value;
                    }
                    return KeyValuePair;
                })();
                var Dictionary = (function () {
                    function Dictionary() {
                        this._InnerDictionary = [];
                    }
                    Dictionary.prototype.ContainsKey = function (key) {
                        return linq(this._InnerDictionary).Any(function (a) {
                            return a.Key === key;
                        });
                    };
                    Dictionary.prototype.Add = function (key, value) {
                        if (this.ContainsKey(key)) {
                            throw "Dictionary alredy contains the key " + key;
                        }
                        this._InnerDictionary.push(new KeyValuePair(key, value));
                    };
                    Dictionary.prototype.AddOrReplace = function (key, value) {
                        var existing = linq(this._InnerDictionary).First(function (a) {
                            return a.Key === key;
                        });
                        if (existing) {
                            existing.Value = value;
                        } else {
                            this._InnerDictionary.push(new KeyValuePair(key, value));
                        }
                    };
                    Dictionary.prototype.Value = function (key) {
                        var existing = linq(this._InnerDictionary).First(function (a) {
                            return a.Key === key;
                        });
                        return existing ? existing.Value : null;
                    };
                    Dictionary.prototype.Remove = function (key) {
                        for (var i = 0, ii = this._InnerDictionary.length; i < ii; i++) {
                            if (this._InnerDictionary[i].Key === key) {
                                this._InnerDictionary.splice(i, 1);
                                return true;
                            }
                        }
                        return false;
                    };
                    Dictionary.prototype.Merge = function (dictionary) {
                        if (dictionary) {
                            for (var i = 0, ii = dictionary._InnerDictionary.length; i < ii; i++) {
                                var item = dictionary._InnerDictionary[i];
                                this.Add(item.Key, item.Value);
                            }
                        }
                        return this;
                    };
                    return Dictionary;
                })();
                Utils.Dictionary = Dictionary;
            })(WebExpressions.Utils || (WebExpressions.Utils = {}));
            var Utils = WebExpressions.Utils;
        })(WebExpressions || (WebExpressions = {}));


        var WebExpressions;
        (function (WebExpressions) {
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
            })(WebExpressions.Meta || (WebExpressions.Meta = {}));
            var Meta = WebExpressions.Meta;
        })(WebExpressions || (WebExpressions = {}));


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
                Expression.prototype.EvalCompile = function () {
                    if (!this._EvalCompiled) {
                        var result = this.EvalExpression();
                        var logic = new Function(WebExpressions.ConstantExpression.ConstantParameter, result.Expression);
                        this._EvalCompiled = function () {
                            return logic(result.Constants);
                        };
                    }
                    return this._EvalCompiled;
                };
                Expression.prototype._Compile = function () {
                    throw "Invalid operation";
                };
                Expression.prototype.EvalExpression = function () {
                    throw "Invalid operation";
                };
                Expression.prototype.GetAffectedProperties = function () {
                    return [];
                };
                Expression.ExpressionConstructorDictionary = (function () {
                    var dictionary = {
                    };
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
                    dictionary[WebExpressions.Meta.ExpressionWrapperType.MethodCall] = function (meta) {
                        return new WebExpressions.MethodCallExpression(meta);
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
                Expression.CreateExpression = function CreateExpression(meta) {
                    if (meta.NodeType === WebExpressions.Meta.ExpressionType.Assign && meta.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Binary) {
                        return new WebExpressions.AssignmentExpression(meta);
                    }
                    if (Expression.ExpressionConstructorDictionary[meta.ExpressionType]) {
                        return Expression.ExpressionConstructorDictionary[meta.ExpressionType](meta);
                    }
                    throw "Invalid expression type";
                };
                return Expression;
            })();
            WebExpressions.Expression = Expression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var AssignmentExpression = (function (_super) {
                __extends(AssignmentExpression, _super);
                function AssignmentExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Left",
                        inputType: "object"
                    }, {
                        inputName: "Right",
                        inputType: "object"
                    });
                    switch (meta.Left.ExpressionType) {
                        case WebExpressions.Meta.ExpressionWrapperType.Parameter:
                            this.Left = null;
                            this.LeftProperty = (meta.Left).Name;
                            break;
                        case WebExpressions.Meta.ExpressionWrapperType.Member:
                            this.Left = WebExpressions.Expression.CreateExpression((meta.Left).Expression);
                            this.LeftProperty = (meta.Left).MemberName;
                            break;
                        default:
                            throw "The left hand side of an assignment must be a parameter or a member";
                    }
                    this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
                }
                AssignmentExpression.prototype.EvalExpression = function () {
                    var right = this.Right.EvalExpression();
                    var left = this.Left ? this.Left.EvalExpression() : null;
                    if (left) {
                        right.Constants.Merge(left.Constants);
                    }
                    return {
                        Expression: "(" + (left ? left.Expression + "." : "") + this.LeftProperty + " = " + right.Expression + ")",
                        Constants: right.Constants
                    };
                };
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
            })(WebExpressions.Expression);
            WebExpressions.AssignmentExpression = AssignmentExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var Meta = WebExpressions.Meta;
            var BinaryExpression = (function (_super) {
                __extends(BinaryExpression, _super);
                function BinaryExpression(meta) {
                    _super.call(this, meta);
                    if (!BinaryExpression.OperatorDictionary[this.NodeType]) {
                        throw "##" + "Invalid Operator";
                    }
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Left",
                        inputType: "object"
                    }, {
                        inputName: "Right",
                        inputType: "object"
                    });
                    this.Left = WebExpressions.Expression.CreateExpression(meta.Left);
                    this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
                }
                BinaryExpression.OperatorDictionary = (function () {
                    var output = [];
                    output[WebExpressions.Meta.ExpressionType.Add] = function (left, right) {
                        return left + right;
                    };
                    output[WebExpressions.Meta.ExpressionType.AndAlso] = function (left, right) {
                        return left && right;
                    };
                    output[WebExpressions.Meta.ExpressionType.Divide] = function (left, right) {
                        return left / right;
                    };
                    output[WebExpressions.Meta.ExpressionType.Equal] = function (left, right) {
                        return left === right;
                    };
                    output[WebExpressions.Meta.ExpressionType.GreaterThan] = function (left, right) {
                        return left > right;
                    };
                    output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = function (left, right) {
                        return left >= right;
                    };
                    output[WebExpressions.Meta.ExpressionType.LessThan] = function (left, right) {
                        return left < right;
                    };
                    output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = function (left, right) {
                        return left <= right;
                    };
                    output[WebExpressions.Meta.ExpressionType.Multiply] = function (left, right) {
                        return left * right;
                    };
                    output[WebExpressions.Meta.ExpressionType.OrElse] = function (left, right) {
                        return left || right;
                    };
                    output[WebExpressions.Meta.ExpressionType.Subtract] = function (left, right) {
                        return left - right;
                    };
                    return output;
                })();
                BinaryExpression.OperatorStringDictionary = (function () {
                    var output = [];
                    output[WebExpressions.Meta.ExpressionType.Add] = " + ";
                    output[WebExpressions.Meta.ExpressionType.AndAlso] = " && ";
                    output[WebExpressions.Meta.ExpressionType.Divide] = " / ";
                    output[WebExpressions.Meta.ExpressionType.Equal] = " === ";
                    output[WebExpressions.Meta.ExpressionType.GreaterThan] = " > ";
                    output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = " >= ";
                    output[WebExpressions.Meta.ExpressionType.LessThan] = " < ";
                    output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = " left <= ";
                    output[WebExpressions.Meta.ExpressionType.Multiply] = " * ";
                    output[WebExpressions.Meta.ExpressionType.OrElse] = " || ";
                    output[WebExpressions.Meta.ExpressionType.Subtract] = " - ";
                    return output;
                })();
                BinaryExpression.prototype.EvalExpression = function () {
                    if (!BinaryExpression.OperatorStringDictionary[this.NodeType]) {
                        throw "Invalid expression type";
                    }
                    var left = this.Left.EvalExpression();
                    var right = this.Right.EvalExpression();
                    return {
                        Expression: "(" + left.Expression + BinaryExpression.OperatorStringDictionary[this.NodeType] + right.Expression + ")",
                        Constants: left.Constants.Merge(right.Constants)
                    };
                };
                BinaryExpression.prototype._Compile = function () {
                    var _this = this;
                    if (!BinaryExpression.OperatorStringDictionary[this.NodeType]) {
                        throw "Invalid expression type";
                    }
                    var left = this.Left.Compile();
                    var right = this.Right.Compile();
                    return function (ambientContext) {
                        return BinaryExpression.OperatorDictionary[_this.NodeType](left(ambientContext), right(ambientContext));
                    };
                };
                return BinaryExpression;
            })(WebExpressions.Expression);
            WebExpressions.BinaryExpression = BinaryExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var BlockExpression = (function (_super) {
                __extends(BlockExpression, _super);
                function BlockExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Expressions",
                        inputConstructor: Array
                    });
                    this.Expressions = linq(meta.Expressions).Select(function (a) {
                        return WebExpressions.Expression.CreateExpression(a);
                    }).Result;
                }
                BlockExpression.prototype.EvalExpression = function () {
                    var expressions = linq(this.Expressions).Select(function (a) {
                        return a.EvalExpression();
                    }).Result;
                    if (!expressions.length) {
                        return {
                            Constants: new WebExpressions.Utils.Dictionary(),
                            Expression: ""
                        };
                    }
                    for (var i = 1, ii = expressions.length; i < ii; i++) {
                        expressions[0].Constants.Merge(expressions[i].Constants);
                    }
                    return {
                        Expression: linq(expressions).Select(function (a) {
                            return a.Expression;
                        }).Result.join(";\n"),
                        Constants: expressions[0].Constants
                    };
                };
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
            })(WebExpressions.Expression);
            WebExpressions.BlockExpression = BlockExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var ConditionalExpression = (function (_super) {
                __extends(ConditionalExpression, _super);
                function ConditionalExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "IfTrue",
                        inputType: "object"
                    }, {
                        inputName: "IfFalse",
                        inputType: "object"
                    }, {
                        inputName: "Test",
                        inputType: "object"
                    });
                    this.IfTrue = WebExpressions.Expression.CreateExpression(meta.IfTrue);
                    this.IfFalse = WebExpressions.Expression.CreateExpression(meta.IfFalse);
                    this.Test = WebExpressions.Expression.CreateExpression(meta.Test);
                }
                ConditionalExpression.prototype.EvalExpression = function () {
                    if (this.IfTrue.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block || this.IfFalse.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Block) {
                        return this._ToBlockString();
                    }
                    return this._ToInlineString();
                };
                ConditionalExpression.prototype._ToInlineString = function () {
                    var test = this.Test.EvalExpression();
                    var ifTrue = this.IfTrue.EvalExpression();
                    var ifFalse = this.IfFalse.EvalExpression();
                    return {
                        Constants: test.Constants.Merge(ifTrue.Constants).Merge(ifFalse.Constants),
                        Expression: "(" + test.Expression + " ? " + ifTrue.Expression + " : " + ifFalse.Expression + ")"
                    };
                };
                ConditionalExpression.prototype._ToBlockString = function () {
                    var test = this.Test.EvalExpression();
                    var ifTrue = this.IfTrue.EvalExpression();
                    var ifFalse = this.IfFalse.EvalExpression();
                    return {
                        Constants: test.Constants.Merge(ifTrue.Constants).Merge(ifFalse.Constants),
                        Expression: "if(" + test.Expression + ") { " + ifTrue.Expression + " } else { " + ifFalse.Expression + " }"
                    };
                };
                ConditionalExpression.prototype._Compile = function () {
                    var test = this.Test.Compile();
                    var ifTrue = this.IfTrue.Compile();
                    var ifFalse = this.IfFalse.Compile();
                    return function (ambientContext) {
                        return test(ambientContext) ? ifTrue(ambientContext) : ifFalse(ambientContext);
                    };
                };
                return ConditionalExpression;
            })(WebExpressions.Expression);
            WebExpressions.ConditionalExpression = ConditionalExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var ConstantExpression = (function (_super) {
                __extends(ConstantExpression, _super);
                function ConstantExpression(meta) {
                    _super.call(this, meta);
                    this.Value = meta.Value;
                }
                ConstantExpression.ConstantParameter = "__constants";
                ConstantExpression.GenerateConstantId = (function () {
                    var id = 0;
                    return function () {
                        return "constant-" + (++id);
                    };
                })();
                ConstantExpression.prototype.EvalExpression = function () {
                    var accessor = ConstantExpression.GenerateConstantId();
                    var constant = new WebExpressions.Utils.Dictionary();
                    constant.Add(accessor, this.Value);
                    return {
                        Constants: constant,
                        Expression: ConstantExpression.ConstantParameter + "[" + accessor + "]"
                    };
                };
                ConstantExpression.prototype._Compile = function () {
                    var _this = this;
                    return function (ambientContext) {
                        return _this.Value;
                    };
                };
                return ConstantExpression;
            })(WebExpressions.Expression);
            WebExpressions.ConstantExpression = ConstantExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var DefaultExpression = (function (_super) {
                __extends(DefaultExpression, _super);
                function DefaultExpression(meta) {
                    _super.call(this, meta);
                }
                DefaultExpression.prototype.EvalExpression = function () {
                    return {
                        Expression: "",
                        Constants: new WebExpressions.Utils.Dictionary()
                    };
                };
                DefaultExpression.prototype._Compile = function () {
                    return function (ambientContext) {
                        return null;
                    };
                };
                return DefaultExpression;
            })(WebExpressions.Expression);
            WebExpressions.DefaultExpression = DefaultExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var InvocationExpression = (function (_super) {
                __extends(InvocationExpression, _super);
                function InvocationExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Expression",
                        inputType: "object"
                    }, {
                        inputName: "Arguments",
                        inputConstructor: Array
                    });
                    this.Expression = WebExpressions.Expression.CreateExpression(meta.Expression);
                    this.Arguments = linq(meta.Arguments).Select(function (a) {
                        return WebExpressions.Expression.CreateExpression(a);
                    }).Result;
                }
                InvocationExpression.prototype.EvalExpression = function () {
                    var expression = this.Expression.EvalExpression();
                    var args = linq(this.Arguments).Select(function (a) {
                        return a.EvalExpression();
                    });
                    linq(args).Each(function (a) {
                        return expression.Constants.Merge(a.Constants);
                    });
                    return {
                        Constants: expression.Constants,
                        Expression: expression.Expression + "(" + linq(args).Select(function (a) {
                            return a.Expression;
                        }).Result.join(", ") + ")"
                    };
                };
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
            })(WebExpressions.Expression);
            WebExpressions.InvocationExpression = InvocationExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var MemberExpression = (function (_super) {
                __extends(MemberExpression, _super);
                function MemberExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Expression",
                        inputType: "object"
                    }, {
                        inputName: "MemberName",
                        inputConstructor: String
                    });
                    this.Expression = WebExpressions.Expression.CreateExpression(meta.Expression);
                    this.MemberName = meta.MemberName;
                }
                MemberExpression.PropertyRegex = new RegExp("^[a-zA-Z][a-zA-Z0-9]*$");
                MemberExpression.prototype.EvalExpression = function () {
                    if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
                        throw "Invalid property name: " + this.MemberName;
                    }
                    var expression = this.Expression.EvalExpression();
                    return {
                        Expression: expression.Expression + "." + this.MemberName,
                        Constants: expression.Constants
                    };
                };
                MemberExpression.prototype._Compile = function () {
                    if (!MemberExpression.PropertyRegex.test(this.MemberName)) {
                        throw "Invalid property name: " + this.MemberName;
                    }
                    var name = this.MemberName;
                    var expression = this.Expression.Compile();
                    return function (ambientContext) {
                        return expression(ambientContext)[name];
                    };
                };
                return MemberExpression;
            })(WebExpressions.Expression);
            WebExpressions.MemberExpression = MemberExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var MethodCallExpression = (function (_super) {
                __extends(MethodCallExpression, _super);
                function MethodCallExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
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
                    this.Object = WebExpressions.Expression.CreateExpression(meta.Object);
                    this.Arguments = linq(meta.Arguments).Select(function (a) {
                        return WebExpressions.Expression.CreateExpression(a);
                    }).Result;
                    this.MethodName = meta.MethodName;
                    this.MethodFullName = meta.MethodFullName;
                }
                MethodCallExpression.prototype.EvalExpression = function () {
                    if (!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                        throw "Invalid method name: " + this.MethodName;
                    }
                    var args = linq(this.Arguments).Select(function (a) {
                        return a.EvalExpression();
                    }).Result;
                    var object = this.Object.EvalExpression();
                    linq(args).Each(function (a) {
                        return object.Constants.Merge(a.Constants);
                    });
                    var mthd = "__o[\"" + this.MethodName + "\"]";
                    return {
                        Expression: "(function (__o) { return (" + mthd + " ? " + mthd + " : ex.ns.MethodCallExpression.RegisteredMethods[\"" + this.MethodFullName + "\"]).call(__o, " + args.join(", ") + "); })(" + object.Expression + ")",
                        Constants: object.Constants
                    };
                };
                MethodCallExpression.prototype._Compile = function () {
                    var _this = this;
                    if (!WebExpressions.MemberExpression.PropertyRegex.test(this.MethodName)) {
                        throw "Invalid method name: " + this.MethodName;
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
                        return (o[_this.MethodName] ? o[_this.MethodName] : MethodCallExpression.RegisteredMethods[_this.MethodFullName]).apply(o, params);
                    };
                };
                MethodCallExpression.RegisteredMethods = {
                };
                return MethodCallExpression;
            })(WebExpressions.Expression);
            WebExpressions.MethodCallExpression = MethodCallExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var NewExpression = (function (_super) {
                __extends(NewExpression, _super);
                function NewExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Arguments",
                        inputConstructor: Array
                    }, {
                        inputName: "IsAnonymous",
                        inputConstructor: Boolean
                    }, {
                        inputName: "Type",
                        inputConstructor: String
                    });
                    if (meta.IsAnonymous) {
                        WebExpressions.Sanitizer.Require(meta, {
                            inputName: "Members",
                            inputConstructor: Array
                        });
                        if (meta.Members.length !== meta.Arguments.length) {
                            throw "If members are defined, each must have a corresponding argument";
                        }
                    }
                    this.Arguments = linq(meta.Arguments).Select(function (a) {
                        return WebExpressions.Expression.CreateExpression(a);
                    }).Result;
                    this.Members = meta.Members;
                    this.Type = meta.Type;
                    this.IsAnonymous = meta.IsAnonymous;
                }
                NewExpression.prototype.EvalExpression = function () {
                    var args = linq(this.Arguments).Select(function (a) {
                        return a.EvalExpression();
                    }).Result;
                    var constants = new WebExpressions.Utils.Dictionary();
                    linq(args).Each(function (a) {
                        return constants.Merge(a.Constants);
                    });
                    var argsString = linq(args).Select(function (a) {
                        return a.Expression;
                    }).Result.join(", ");
                    if (this.IsAnonymous) {
                        return {
                            Constants: constants,
                            Expression: "{ " + argsString + " }"
                        };
                    } else if (NewExpression.RegisteredTypes[this.Type]) {
                        return {
                            Constants: constants,
                            Expression: "new ex.ns.NewExpression.RegisteredTypes[\"" + this.Type + "\"](" + argsString + ")"
                        };
                    } else {
                        return {
                            Constants: constants,
                            Expression: "{}"
                        };
                    }
                };
                NewExpression.prototype._Compile = function () {
                    var _this = this;
                    var args = linq(this.Arguments).Select(function (a) {
                        return a.Compile();
                    }).Result;
                    return function (ambientContext) {
                        var params = linq(args).Select(function (a) {
                            return a(ambientContext);
                        }).Result;
                        if (_this.IsAnonymous) {
                            return _this.ConstructAnonymous(params);
                        } else if (NewExpression.RegisteredTypes[_this.Type]) {
                            return NewExpression.Construct(NewExpression.RegisteredTypes[_this.Type], params);
                        } else {
                            return {
                            };
                        }
                    };
                };
                NewExpression.prototype.ConstructAnonymous = function (params) {
                    var obj = {
                    };
                    for (var i = 0, ii = this.Members.length; i < ii; i++) {
                        obj[this.Members[i]] = params[i];
                    }
                    return obj;
                };
                NewExpression.Construct = function Construct(constr, args) {
                    var obj = Object.create(constr.prototype);
                    var result = constr.apply(obj, args);
                    return typeof result === 'object' ? result : obj;
                };
                NewExpression.RegisteredTypes = {
                };
                return NewExpression;
            })(WebExpressions.Expression);
            WebExpressions.NewExpression = NewExpression;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var ParameterExpression = (function (_super) {
                __extends(ParameterExpression, _super);
                function ParameterExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Name",
                        inputConstructor: String
                    });
                    this.Name = meta.Name;
                }
                ParameterExpression.prototype.EvalExpression = function () {
                    return {
                        Expression: this.Name,
                        Constants: new WebExpressions.Utils.Dictionary()
                    };
                };
                ParameterExpression.prototype._Compile = function () {
                    var _this = this;
                    return function (ambientContext) {
                        return ambientContext[_this.Name];
                    };
                };
                return ParameterExpression;
            })(WebExpressions.Expression);
            WebExpressions.ParameterExpression = ParameterExpression;
        })(WebExpressions || (WebExpressions = {}));


        var WebExpressions;
        (function (WebExpressions) {
            WebExpressions.Initialize = function (data) {
            };
            var Sanitizer = (function () {
                function Sanitizer() { }
                Sanitizer.Require = function Require(item) {
                    var properties = [];
                    for (var _i = 0; _i < (arguments.length - 1) ; _i++) {
                        properties[_i] = arguments[_i + 1];
                    }
                    if (item == null) {
                        throw "Item must have a value";
                    }
                    for (var i = 0, ii = properties.length; i < ii; i++) {
                        var prop = properties[i];
                        if (prop.allowNull && item[prop.inputName] == null) {
                            return;
                        }
                        if (!prop.allowNull && item[prop.inputName] == null) {
                            throw "Property " + prop.inputName + " cannot be null";
                        }
                        if (prop.inputType && typeof item[prop.inputName] !== prop.inputType) {
                            throw "Property " + prop.inputName + " must be of type " + prop.inputType;
                        }
                        if (prop.inputConstructor && item[prop.inputName].constructor !== prop.inputConstructor) {
                            throw {
                                message: "Property " + prop.inputName + " must be of a give type",
                                type: prop.inputConstructor
                            };
                        }
                    }
                };
                return Sanitizer;
            })();
            WebExpressions.Sanitizer = Sanitizer;
        })(WebExpressions || (WebExpressions = {}));


        var __extends = this.__extends || function (d, b) {
            function __() { this.constructor = d; }
            __.prototype = b.prototype;
            d.prototype = new __();
        };
        var WebExpressions;
        (function (WebExpressions) {
            var UnaryExpression = (function (_super) {
                __extends(UnaryExpression, _super);
                function UnaryExpression(meta) {
                    _super.call(this, meta);
                    WebExpressions.Sanitizer.Require(meta, {
                        inputName: "Operand",
                        inputType: "object"
                    });
                    this.Operand = WebExpressions.Expression.CreateExpression(meta.Operand);
                }
                UnaryExpression.OperatorDictionary = (function () {
                    var output = [];
                    output[WebExpressions.Meta.ExpressionType.Convert] = function (operand) {
                        return operand;
                    };
                    output[WebExpressions.Meta.ExpressionType.Not] = function (operand) {
                        return !operand;
                    };
                    return output;
                })();
                UnaryExpression.OperatorStringDictionary = (function () {
                    var output = [];
                    output[WebExpressions.Meta.ExpressionType.Convert] = function (operand) {
                        return operand;
                    };
                    output[WebExpressions.Meta.ExpressionType.Not] = function (operand) {
                        return "!" + operand;
                    };
                    return output;
                })();
                UnaryExpression.prototype.EvalExpression = function () {
                    var operand = this.Operand.EvalExpression();
                    return {
                        Expression: "(" + UnaryExpression.OperatorStringDictionary[this.NodeType](operand.Expression) + ")",
                        Constants: operand.Constants
                    };
                };
                UnaryExpression.prototype._Compile = function () {
                    var _this = this;
                    var operand = this.Operand.Compile();
                    return function (ambientContext) {
                        return UnaryExpression.OperatorDictionary[_this.NodeType](operand(ambientContext));
                    };
                };
                return UnaryExpression;
            })(WebExpressions.Expression);
            WebExpressions.UnaryExpression = UnaryExpression;
        })(WebExpressions || (WebExpressions = {}));


        var ex = (function () {
            function ex() { }
            ex.createExpression = WebExpressions.Expression.CreateExpression;
            ex.registeredConstructors = WebExpressions.NewExpression.RegisteredTypes;
            ex.registeredMethods = WebExpressions.MethodCallExpression.RegisteredMethods;
            return ex;
        })();


        window.ex = ex;
        window.WebExpressions = WebExpressions;
    })();



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

    var BackToFront;
    (function (BackToFront) {
    })(BackToFront || (BackToFront = {}));


    var BackToFront;
    (function (BackToFront) {
        (function (Validation) {
            var Validator = (function () {
                function Validator(rules, Entity) {
                    this.Entity = Entity;
                    this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
                }
                Validator.CreateRule = function CreateRule(rule) {
                    var r = ex.createExpression(rule.Expression).Compile();
                    return {
                        RequiredForValidation: linq(rule.RequiredForValidation).Select(function (a) {
                            return Validator.MemberChainItemString(a, true);
                        }).Result,
                        ValidationSubjects: linq(rule.ValidationSubjects).Select(function (a) {
                            return Validator.MemberChainItemString(a, true);
                        }).Result,
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
                Validator.MemberChainItemString = function MemberChainItemString(memberChainItem, skipFirst) {
                    if (skipFirst) {
                        memberChainItem = memberChainItem.NextItem;
                    }
                    var output = [];
                    while (memberChainItem) {
                        output.push(memberChainItem.MemberName);
                        memberChainItem = memberChainItem.NextItem;
                    }
                    return output.join(".");
                };
                Validator.prototype.Validate = function (propertyName, breakOnFirstError) {
                    if (typeof breakOnFirstError === "undefined") { breakOnFirstError = false; }
                    var entity = this.GetEntity();
                    try {
                        return linq(this.Rules).Where(function (rule) {
                            return rule.ValidationSubjects.indexOf(propertyName) !== -1;
                        }).Select(function (rule) {
                            return rule.Validate(entity, breakOnFirstError);
                        }).Aggregate().Result;
                    } catch (e) {
                        debugger;

                    }
                };
                Validator.prototype.GetEntity = function () {
                    throw "Invalid operation, this method is abstract";
                };
                return Validator;
            })();
            Validation.Validator = Validator;
        })(BackToFront.Validation || (BackToFront.Validation = {}));
        var Validation = BackToFront.Validation;
    })(BackToFront || (BackToFront = {}));


    var BackToFront;
    (function (BackToFront) {
        var Initialize = (function () {
            function Initialize() { }
            Initialize._Initialized = false;
            Initialize.Bootstrap = function Bootstrap() {
                if (Initialize._Initialized) {
                    return;
                }
                Initialize._Initialized = true;
                Initialize._InitializeConstructors();
                Initialize._InitializeMethods();
            };
            Initialize._InitializeConstructors = function _InitializeConstructors() {
            };
            Initialize._InitializeMethods = function _InitializeMethods() {
                ex.registeredMethods["System.Collections.Generic.ICollection`1[[BackToFront.IViolation, BackToFront, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].Add"] = function (item) {
                    if (!this || !this.push || this.push.constructor !== Function) {
                        throw "This method must be called on an array" + "##";
                    }
                    this.push(item);
                };
                ex.registeredConstructors["BackToFront.Utilities.SimpleViolation"] = function (userMessage) {
                    this.UserMessage = userMessage;
                };
            };
            return Initialize;
        })();
        BackToFront.Initialize = Initialize;
    })(BackToFront || (BackToFront = {}));
    BackToFront.Initialize.Bootstrap();


    var BackToFront;
    (function (BackToFront) {
        var Sanitizer = (function () {
            function Sanitizer() { }
            Sanitizer.Require = function Require(item) {
                var properties = [];
                for (var _i = 0; _i < (arguments.length - 1) ; _i++) {
                    properties[_i] = arguments[_i + 1];
                }
                if (item == null) {
                    throw "Item must have a value";
                }
                for (var i = 0, ii = properties.length; i < ii; i++) {
                    var prop = properties[i];
                    if (prop.allowNull && item[prop.inputName] == null) {
                        return;
                    }
                    if (!prop.allowNull && item[prop.inputName] == null) {
                        throw "Property " + prop.inputName + " cannot be null";
                    }
                    if (prop.inputType && typeof item[prop.inputName] !== prop.inputType) {
                        throw "Property " + prop.inputName + " must be of type " + prop.inputType;
                    }
                    if (prop.inputConstructor && item[prop.inputName].constructor !== prop.inputConstructor) {
                        throw {
                            message: "Property " + prop.inputName + " must be of a give type",
                            type: prop.inputConstructor
                        };
                    }
                }
            };
            return Sanitizer;
        })();
        BackToFront.Sanitizer = Sanitizer;
    })(BackToFront || (BackToFront = {}));


    var __extends = this.__extends || function (d, b) {
        function __() { this.constructor = d; }
        __.prototype = b.prototype;
        d.prototype = new __();
    };
    var BackToFront;
    (function (BackToFront) {
        (function (Validation) {
            var JqueryBTFContext = (function () {
                function JqueryBTFContext(Errors) {
                    if (typeof Errors === "undefined") { Errors = []; }
                    this.Errors = Errors;
                }
                return JqueryBTFContext;
            })();
            Validation.JqueryBTFContext = JqueryBTFContext;
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
                        return linq(r.RequiredForValidation || []).Union(r.ValidationSubjects || []).Result;
                    }).Aggregate().Result;
                    for (var j = 0, jj = allNames.length; j < jj; j++) {
                        var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);
                        if (item.attr("type") === "checkbox") {
                            entity[allNames[j]] = item.is(":checked");
                        } else {
                            entity[allNames[j]] = item.val();
                            if (item.attr("data-val-number")) {
                                entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ? parseFloat(entity[allNames[j]]) : parseInt(entity[allNames[j]]);
                            }
                        }
                    }
                    return entity;
                };
                JQueryValidator.Registered = [];
                JQueryValidator.ValidatorName = "backtofront";
                JQueryValidator.RegisterRule = function RegisterRule(rule) {
                    BackToFront.Sanitizer.Require(rule, {
                        inputName: "Rules",
                        inputConstructor: Array
                    }, {
                        inputName: "Entity",
                        inputConstructor: String
                    });
                    JQueryValidator.Registered.push(new JQueryValidator(rule.Rules, rule.Entity));
                };
                JQueryValidator.Setup = function Setup() {
                    if (!jQuery || !jQuery.validator) {
                        throw "This item requires jQuery and jQuery validation";
                    }
                    if (jQuery.validator.methods[JQueryValidator.ValidatorName]) {
                        return;
                    }
                    jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, function (aaaa, bbbb) {
                        if (this.__BTFContext && this.__BTFContext.Errors && this.__BTFContext.Errors.length) {
                            return linq(this.__BTFContext.Errors).Select(function (a) {
                                return a.UserMessage;
                            }).Result.join("\n");
                            return jQuery.validator.format("These have been injected: {0}, {1}", "\"me\"", "\"and me\"");
                        } else {
                            return undefined;
                        }
                    });
                    if (jQuery.validator.unobtrusive && jQuery.validator.unobtrusive.adapters) {
                        jQuery.validator.unobtrusive.adapters.add("backtofront", [], function (options) {
                            options.rules["backtofront"] = options.params;
                        });
                    }
                };
                JQueryValidator.Validate = function Validate(value, element) {
                    var params = [];
                    for (var _i = 0; _i < (arguments.length - 2) ; _i++) {
                        params[_i] = arguments[_i + 2];
                    }
                    var results = linq(JQueryValidator.Registered).Select(function (a) {
                        return a.Validate($(element).attr("name"), false);
                    }).Aggregate();
                    this.__BTFContext = new JqueryBTFContext(results.Result);
                    return results.Result.length === 0;
                };
                return JQueryValidator;
            })(BackToFront.Validation.Validator);
            Validation.JQueryValidator = JQueryValidator;
        })(BackToFront.Validation || (BackToFront.Validation = {}));
        var Validation = BackToFront.Validation;
    })(BackToFront || (BackToFront = {}));


    window["__BTF"] = BackToFront;
})();
