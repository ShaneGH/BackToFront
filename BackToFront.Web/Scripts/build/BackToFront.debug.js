(function () { (function () { 
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

// Module
var Shapes;
(function (Shapes) {
    // Class
    var Point = (function () {
        // Constructor
        function Point(x, y) {
            this.x = x;
            this.y = y;
        }
        // Instance member
        Point.prototype.getDist = function () {
            return Math.sqrt(this.x * this.x + this.y * this.y);
        };

        Point.origin = new Point(0, 0);
        return Point;
    })();
    Shapes.Point = Point;
})(Shapes || (Shapes = {}));

// Local variables
var p = new Shapes.Point(3, 4);
var dist = p.getDist();


var WebExpressions;
(function (WebExpressions) {
    /// <reference path="../../ref/linq.d.ts" />
    (function (Utils) {
        //Temporary location, typescript build is being a bit problematic
        var CustomClassHandler = (function () {
            function CustomClassHandler() {
            }
            CustomClassHandler.GetClass = function (fullyQualifiedClassName) {
                var className = CustomClassHandler.SplitNamespace(fullyQualifiedClassName);

                var item = window;
                for (var i = 0, ii = className.length; i < ii; i++) {
                    item = item[className[i]];
                    if (item == undefined)
                        throw "Cannot evaluate member " + className;
                }

                return item;
            };

            CustomClassHandler.SplitNamespace = function (input) {
                var output = input.split(".");
                linq(output).Each(function (a) {
                    if (!CustomClassHandler.PropertyRegex.test(a))
                        throw "Invalid namespace part " + a;
                });

                return output;
            };

            CustomClassHandler.AddStaticItem = function (fullyQualifiedName, item) {
                var ns = CustomClassHandler.SplitNamespace(fullyQualifiedName);
                if (ns.length < 2)
                    throw "Invalid namespace";

                var item = window;
                var i = 0;
                for (var ii = ns.length - 2; i < ii; i++) {
                    if (!item[ns[i]])
                        item = (item[ns[i]] = {});
else
                        item = item[ns[i]];
                }

                if (!item[ns[i]])
                    item = (item[ns[i]] = function () {
                    });
            };
            CustomClassHandler.PropertyRegex = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");
            return CustomClassHandler;
        })();
        Utils.CustomClassHandler = CustomClassHandler;

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
                if (this.ContainsKey(key))
                    throw "Dictionary alredy contains the key " + key;

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
            ExpressionWrapperType[ExpressionWrapperType["Binary"] = 1] = "Binary";
            ExpressionWrapperType[ExpressionWrapperType["Constant"] = 2] = "Constant";
            ExpressionWrapperType[ExpressionWrapperType["Member"] = 3] = "Member";
            ExpressionWrapperType[ExpressionWrapperType["StaticMember"] = 4] = "StaticMember";
            ExpressionWrapperType[ExpressionWrapperType["MethodCall"] = 5] = "MethodCall";
            ExpressionWrapperType[ExpressionWrapperType["StaticMethodCall"] = 6] = "StaticMethodCall";
            ExpressionWrapperType[ExpressionWrapperType["Parameter"] = 7] = "Parameter";
            ExpressionWrapperType[ExpressionWrapperType["Unary"] = 8] = "Unary";
            ExpressionWrapperType[ExpressionWrapperType["Default"] = 9] = "Default";
            ExpressionWrapperType[ExpressionWrapperType["Block"] = 10] = "Block";
            ExpressionWrapperType[ExpressionWrapperType["Conditional"] = 11] = "Conditional";
            ExpressionWrapperType[ExpressionWrapperType["Invocation"] = 12] = "Invocation";
            ExpressionWrapperType[ExpressionWrapperType["New"] = 13] = "New";
        })(Meta.ExpressionWrapperType || (Meta.ExpressionWrapperType = {}));
        var ExpressionWrapperType = Meta.ExpressionWrapperType;

        (function (ExpressionType) {
            ExpressionType[ExpressionType["Add"] = 0] = "Add";
            ExpressionType[ExpressionType["AddChecked"] = 1] = "AddChecked";
            ExpressionType[ExpressionType["And"] = 2] = "And";
            ExpressionType[ExpressionType["AndAlso"] = 3] = "AndAlso";
            ExpressionType[ExpressionType["ArrayLength"] = 4] = "ArrayLength";
            ExpressionType[ExpressionType["ArrayIndex"] = 5] = "ArrayIndex";
            ExpressionType[ExpressionType["Call"] = 6] = "Call";
            ExpressionType[ExpressionType["Coalesce"] = 7] = "Coalesce";
            ExpressionType[ExpressionType["Conditional"] = 8] = "Conditional";
            ExpressionType[ExpressionType["Constant"] = 9] = "Constant";
            ExpressionType[ExpressionType["Convert"] = 10] = "Convert";
            ExpressionType[ExpressionType["ConvertChecked"] = 11] = "ConvertChecked";
            ExpressionType[ExpressionType["Divide"] = 12] = "Divide";
            ExpressionType[ExpressionType["Equal"] = 13] = "Equal";
            ExpressionType[ExpressionType["ExclusiveOr"] = 14] = "ExclusiveOr";
            ExpressionType[ExpressionType["GreaterThan"] = 15] = "GreaterThan";
            ExpressionType[ExpressionType["GreaterThanOrEqual"] = 16] = "GreaterThanOrEqual";
            ExpressionType[ExpressionType["Invoke"] = 17] = "Invoke";
            ExpressionType[ExpressionType["Lambda"] = 18] = "Lambda";
            ExpressionType[ExpressionType["LeftShift"] = 19] = "LeftShift";
            ExpressionType[ExpressionType["LessThan"] = 20] = "LessThan";
            ExpressionType[ExpressionType["LessThanOrEqual"] = 21] = "LessThanOrEqual";
            ExpressionType[ExpressionType["ListInit"] = 22] = "ListInit";
            ExpressionType[ExpressionType["MemberAccess"] = 23] = "MemberAccess";
            ExpressionType[ExpressionType["MemberInit"] = 24] = "MemberInit";
            ExpressionType[ExpressionType["Modulo"] = 25] = "Modulo";
            ExpressionType[ExpressionType["Multiply"] = 26] = "Multiply";
            ExpressionType[ExpressionType["MultiplyChecked"] = 27] = "MultiplyChecked";
            ExpressionType[ExpressionType["Negate"] = 28] = "Negate";
            ExpressionType[ExpressionType["UnaryPlus"] = 29] = "UnaryPlus";
            ExpressionType[ExpressionType["NegateChecked"] = 30] = "NegateChecked";
            ExpressionType[ExpressionType["New"] = 31] = "New";
            ExpressionType[ExpressionType["NewArrayInit"] = 32] = "NewArrayInit";
            ExpressionType[ExpressionType["NewArrayBounds"] = 33] = "NewArrayBounds";
            ExpressionType[ExpressionType["Not"] = 34] = "Not";
            ExpressionType[ExpressionType["NotEqual"] = 35] = "NotEqual";
            ExpressionType[ExpressionType["Or"] = 36] = "Or";
            ExpressionType[ExpressionType["OrElse"] = 37] = "OrElse";
            ExpressionType[ExpressionType["Parameter"] = 38] = "Parameter";
            ExpressionType[ExpressionType["Power"] = 39] = "Power";
            ExpressionType[ExpressionType["Quote"] = 40] = "Quote";
            ExpressionType[ExpressionType["RightShift"] = 41] = "RightShift";
            ExpressionType[ExpressionType["Subtract"] = 42] = "Subtract";
            ExpressionType[ExpressionType["SubtractChecked"] = 43] = "SubtractChecked";
            ExpressionType[ExpressionType["TypeAs"] = 44] = "TypeAs";
            ExpressionType[ExpressionType["TypeIs"] = 45] = "TypeIs";
            ExpressionType[ExpressionType["Assign"] = 46] = "Assign";
            ExpressionType[ExpressionType["Block"] = 47] = "Block";
            ExpressionType[ExpressionType["DebugInfo"] = 48] = "DebugInfo";
            ExpressionType[ExpressionType["Decrement"] = 49] = "Decrement";
            ExpressionType[ExpressionType["Dynamic"] = 50] = "Dynamic";
            ExpressionType[ExpressionType["Default"] = 51] = "Default";
            ExpressionType[ExpressionType["Extension"] = 52] = "Extension";
            ExpressionType[ExpressionType["Goto"] = 53] = "Goto";
            ExpressionType[ExpressionType["Increment"] = 54] = "Increment";
            ExpressionType[ExpressionType["Index"] = 55] = "Index";
            ExpressionType[ExpressionType["Label"] = 56] = "Label";
            ExpressionType[ExpressionType["RuntimeVariables"] = 57] = "RuntimeVariables";
            ExpressionType[ExpressionType["Loop"] = 58] = "Loop";
            ExpressionType[ExpressionType["Switch"] = 59] = "Switch";
            ExpressionType[ExpressionType["Throw"] = 60] = "Throw";
            ExpressionType[ExpressionType["Try"] = 61] = "Try";
            ExpressionType[ExpressionType["Unbox"] = 62] = "Unbox";
            ExpressionType[ExpressionType["AddAssign"] = 63] = "AddAssign";
            ExpressionType[ExpressionType["AndAssign"] = 64] = "AndAssign";
            ExpressionType[ExpressionType["DivideAssign"] = 65] = "DivideAssign";
            ExpressionType[ExpressionType["ExclusiveOrAssign"] = 66] = "ExclusiveOrAssign";
            ExpressionType[ExpressionType["LeftShiftAssign"] = 67] = "LeftShiftAssign";
            ExpressionType[ExpressionType["ModuloAssign"] = 68] = "ModuloAssign";
            ExpressionType[ExpressionType["MultiplyAssign"] = 69] = "MultiplyAssign";
            ExpressionType[ExpressionType["OrAssign"] = 70] = "OrAssign";
            ExpressionType[ExpressionType["PowerAssign"] = 71] = "PowerAssign";
            ExpressionType[ExpressionType["RightShiftAssign"] = 72] = "RightShiftAssign";
            ExpressionType[ExpressionType["SubtractAssign"] = 73] = "SubtractAssign";
            ExpressionType[ExpressionType["AddAssignChecked"] = 74] = "AddAssignChecked";
            ExpressionType[ExpressionType["MultiplyAssignChecked"] = 75] = "MultiplyAssignChecked";
            ExpressionType[ExpressionType["SubtractAssignChecked"] = 76] = "SubtractAssignChecked";
            ExpressionType[ExpressionType["PreIncrementAssign"] = 77] = "PreIncrementAssign";
            ExpressionType[ExpressionType["PreDecrementAssign"] = 78] = "PreDecrementAssign";
            ExpressionType[ExpressionType["PostIncrementAssign"] = 79] = "PostIncrementAssign";
            ExpressionType[ExpressionType["PostDecrementAssign"] = 80] = "PostDecrementAssign";
            ExpressionType[ExpressionType["TypeEqual"] = 81] = "TypeEqual";
            ExpressionType[ExpressionType["OnesComplement"] = 82] = "OnesComplement";
            ExpressionType[ExpressionType["IsTrue"] = 83] = "IsTrue";
            ExpressionType[ExpressionType["IsFalse"] = 84] = "IsFalse";
        })(Meta.ExpressionType || (Meta.ExpressionType = {}));
        var ExpressionType = Meta.ExpressionType;
    })(WebExpressions.Meta || (WebExpressions.Meta = {}));
    var Meta = WebExpressions.Meta;
})(WebExpressions || (WebExpressions = {}));


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


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    ///<summary>Special case for binary assignments</summary>
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
                case WebExpressions.Meta.ExpressionWrapperType.StaticMember:
                case WebExpressions.Meta.ExpressionWrapperType.Member:
                    this.Left = WebExpressions.Expression.CreateExpression((meta.Left).Expression);
                    this.LeftProperty = (meta.Left).MemberName;
                    break;
                default:
                    throw "The left hand side of an assignment must be a parameter or a member";
            }

            this.Right = WebExpressions.Expression.CreateExpression(meta.Right);
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
    })(WebExpressions.Expression);
    WebExpressions.AssignmentExpression = AssignmentExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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

            if (!BinaryExpression.OperatorDictionary[this.NodeType])
                throw "##" + "Invalid Operator";

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
        BinaryExpression.OperatorDictionary = (function () {
            var output = [];

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Add] = function (left, right) {
                return left + right;
            };
            output[WebExpressions.Meta.ExpressionType.AndAlso] = function (left, right) {
                return left && right;
            };
            output[WebExpressions.Meta.ExpressionType.Divide] = function (left, right) {
                return left / right;
            };

            //TODO: is this the right equals?
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
            output[WebExpressions.Meta.ExpressionType.NotEqual] = function (left, right) {
                return left !== right;
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

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Add] = " + ";
            output[WebExpressions.Meta.ExpressionType.AndAlso] = " && ";
            output[WebExpressions.Meta.ExpressionType.Divide] = " / ";

            //TODO: is this the right equals?
            output[WebExpressions.Meta.ExpressionType.Equal] = " === ";
            output[WebExpressions.Meta.ExpressionType.GreaterThan] = " > ";
            output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = " >= ";
            output[WebExpressions.Meta.ExpressionType.LessThan] = " < ";
            output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = " <= ";
            output[WebExpressions.Meta.ExpressionType.Multiply] = " * ";
            output[WebExpressions.Meta.ExpressionType.NotEqual] = " !== ";
            output[WebExpressions.Meta.ExpressionType.OrElse] = " || ";
            output[WebExpressions.Meta.ExpressionType.Subtract] = " - ";

            return output;
        })();
        return BinaryExpression;
    })(WebExpressions.Expression);
    WebExpressions.BinaryExpression = BinaryExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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

            //TODO: can IfFalse be null?
            this.IfFalse = WebExpressions.Expression.CreateExpression(meta.IfFalse);
            this.Test = WebExpressions.Expression.CreateExpression(meta.Test);
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
    })(WebExpressions.Expression);
    WebExpressions.ConditionalExpression = ConditionalExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
/// <reference path="Utils/Dictionary.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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
        ConstantExpression.prototype._Compile = function () {
            var _this = this;
            return function (ambientContext) {
                return _this.Value;
            };
        };
        ConstantExpression.ConstantParameter = "__constants";

        ConstantExpression.GenerateConstantId = (function () {
            var id = 0;
            return function () {
                return "constant-" + (++id);
            };
        })();
        return ConstantExpression;
    })(WebExpressions.Expression);
    WebExpressions.ConstantExpression = ConstantExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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
        //TODO
        DefaultExpression.prototype._Compile = function () {
            return function (ambientContext) {
                return null;
            };
        };
        return DefaultExpression;
    })(WebExpressions.Expression);
    WebExpressions.DefaultExpression = DefaultExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var WebExpressions;
(function (WebExpressions) {
    var MemberExpressionBase = (function (_super) {
        __extends(MemberExpressionBase, _super);
        function MemberExpressionBase(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "MemberName",
                inputConstructor: String
            });

            this.MemberName = meta.MemberName;
        }
        MemberExpressionBase.prototype._Compile = function () {
            if (!WebExpressions.Utils.CustomClassHandler.PropertyRegex.test(this.MemberName)) {
                throw "Invalid property name: " + this.MemberName;
            }

            var name = this.MemberName;
            var expression = this._CompileMemberContext();
            return function (ambientContext) {
                return expression(ambientContext)[name];
            };
        };

        MemberExpressionBase.prototype._CompileMemberContext = function () {
            throw "This method is abstract";
        };
        return MemberExpressionBase;
    })(WebExpressions.Expression);
    WebExpressions.MemberExpressionBase = MemberExpressionBase;

    var MemberExpression = (function (_super) {
        __extends(MemberExpression, _super);
        function MemberExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Expression",
                inputType: "object"
            });

            this.Expression = WebExpressions.Expression.CreateExpression(meta.Expression);
        }
        MemberExpression.prototype._CompileMemberContext = function () {
            return this.Expression.Compile();
        };
        return MemberExpression;
    })(MemberExpressionBase);
    WebExpressions.MemberExpression = MemberExpression;

    var StaticMemberExpression = (function (_super) {
        __extends(StaticMemberExpression, _super);
        function StaticMemberExpression(meta) {
            _super.call(this, meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Class",
                inputType: "string"
            });

            this.Class = meta.Class;
        }
        StaticMemberExpression.prototype._CompileMemberContext = function () {
            var item = WebExpressions.Utils.CustomClassHandler.GetClass(this.Class);
            return function (ambientContext) {
                return item;
            };
        };
        return StaticMemberExpression;
    })(MemberExpressionBase);
    WebExpressions.StaticMemberExpression = StaticMemberExpression;
})(WebExpressions || (WebExpressions = {}));


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

            this.Class = meta.Class;
        }
        StaticMethodCallExpression.prototype._CompileMethodCallContext = function () {
            var item = WebExpressions.Utils.CustomClassHandler.GetClass(this.Class);
            return function (context) {
                return item;
            };
        };
        return StaticMethodCallExpression;
    })(MethodCallExpressionBase);
    WebExpressions.StaticMethodCallExpression = StaticMethodCallExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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

                if (meta.Members.length !== meta.Arguments.length)
                    throw "If members are defined, each must have a corresponding argument";
            }

            this.Arguments = linq(meta.Arguments).Select(function (a) {
                return WebExpressions.Expression.CreateExpression(a);
            }).Result;
            this.Members = meta.Members;
            this.Type = meta.Type;
            this.IsAnonymous = meta.IsAnonymous;
        }
        // TODO: register types
        NewExpression.prototype._Compile = function () {
            var _this = this;
            var args = linq(this.Arguments).Select(function (a) {
                return a.Compile();
            }).Result;
            return function (ambientContext) {
                var params = linq(args).Select(function (a) {
                    return a(ambientContext);
                }).Result;
                if (_this.IsAnonymous)
                    return _this.ConstructAnonymous(params);
else if (NewExpression.RegisteredTypes[_this.Type])
                    return NewExpression.Construct(NewExpression.RegisteredTypes[_this.Type], params);
else
                    return {};
            };
        };

        NewExpression.prototype.ConstructAnonymous = function (params) {
            var obj = {};
            for (var i = 0, ii = this.Members.length; i < ii; i++)
                obj[this.Members[i]] = params[i];

            return obj;
        };

        NewExpression.Construct = function (constr, args) {
            var obj = Object.create(constr.prototype);
            var result = constr.apply(obj, args);

            //TODO: this
            return typeof result === 'object' ? result : obj;
        };

        NewExpression.RegisteredTypes = {};
        return NewExpression;
    })(WebExpressions.Expression);
    WebExpressions.NewExpression = NewExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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
        function Sanitizer() {
        }
        Sanitizer.Require = ///<summary>Class which deals with interactions with non ts code</summary>
        function (item) {
            var properties = [];
            for (var _i = 0; _i < (arguments.length - 1); _i++) {
                properties[_i] = arguments[_i + 1];
            }
            if (item == null) {
                throw "Item must have a value";
            }

            for (var i = 0, ii = properties.length; i < ii; i++) {
                var prop = properties[i];

                if (prop.allowNull && item[prop.inputName] == null)
                    return;
                if (!prop.allowNull && item[prop.inputName] == null)
                    throw "Property " + prop.inputName + " cannot be null";
                if (prop.inputType && typeof item[prop.inputName] !== prop.inputType)
                    throw "Property " + prop.inputName + " must be of type " + prop.inputType;
                if (prop.inputConstructor && item[prop.inputName].constructor !== prop.inputConstructor)
                    throw { message: "Property " + prop.inputName + " must be of a give type", type: prop.inputConstructor };
            }
        };
        return Sanitizer;
    })();
    WebExpressions.Sanitizer = Sanitizer;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="Expression.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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
        UnaryExpression.prototype._Compile = function () {
            var _this = this;
            var operand = this.Operand.Compile();
            return function (ambientContext) {
                return UnaryExpression.OperatorDictionary[_this.NodeType](operand(ambientContext));
            };
        };
        UnaryExpression.OperatorDictionary = (function () {
            var output = [];

            // TODO: more (all) operators
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

            // TODO: more (all) operators
            output[WebExpressions.Meta.ExpressionType.Convert] = function (operand) {
                return operand;
            };
            output[WebExpressions.Meta.ExpressionType.Not] = function (operand) {
                return "!" + operand;
            };

            return output;
        })();
        return UnaryExpression;
    })(WebExpressions.Expression);
    WebExpressions.UnaryExpression = UnaryExpression;
})(WebExpressions || (WebExpressions = {}));


/// <reference path="../WebExpressions/MetaClasses.ts" />
/// <reference path="../WebExpressions/Expression.ts" />
//TODO: rename
var ex = (function () {
    function ex() {
    }
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

/// <reference path="../../../WebExpressions/Scripts/ref/Exports.ts" />


/// <reference path="../Sanitizer.ts" />
/// <reference path="../../../../WebExpressions/Scripts/ref/Exports.ts" />
var BackToFront;
(function (BackToFront) {
    (function (Validation) {
        var Validator = (function () {
            function Validator(rules, Entity) {
                this.Entity = Entity;
                this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
            }
            Validator.CreateRule = function (rule) {
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
                        var context = {};
                        context[rule.EntityParameter] = entity;
                        context[rule.ContextParameter] = {
                            // TODO: hardcoded names (BreakOnFirstError, Violations, Mocks, Dependencies)
                            // TODO: Mocks and Dependencies
                            Violations: [],
                            BreakOnFirstError: breakOnFirstError,
                            Mocks: [],
                            Dependencies: {}
                        };

                        r(context);
                        return context[rule.ContextParameter].Violations;
                    }
                };
            };

            Validator.MemberChainItemString = function (memberChainItem, skipFirst) {
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

                return linq(this.Rules).Where(function (rule) {
                    return rule.ValidationSubjects.indexOf(propertyName) !== -1;
                }).Select(function (rule) {
                    return rule.Validate(entity, breakOnFirstError);
                }).Aggregate().Where(function (violation) {
                    return Validator.FilterViolation(violation, propertyName);
                }).Result;
            };

            // abstract
            Validator.prototype.GetEntity = function () {
                throw "Invalid operation, this method is abstract";
            };

            Validator.FilterViolation = function (violation, propertyName) {
                return linq(violation.Violated).Any(function (member) {
                    return Validator.MemberChainItemString(member, true) === propertyName;
                });
            };
            return Validator;
        })();
        Validation.Validator = Validator;
    })(BackToFront.Validation || (BackToFront.Validation = {}));
    var Validation = BackToFront.Validation;
})(BackToFront || (BackToFront = {}));


/// <reference path="../../../WebExpressions/Scripts/ref/Exports.ts" />
var BackToFront;
(function (BackToFront) {
    //TODO: test
    var Initialize = (function () {
        function Initialize() {
        }
        Initialize.Bootstrap = function () {
            if (Initialize._Initialized)
                return;

            Initialize._Initialized = true;
            Initialize._InitializeConstructors();
            Initialize._InitializeMethods();
        };

        Initialize._InitializeConstructors = function () {
            ex.registeredConstructors["BackToFront.Utilities.SimpleViolation"] = function (userMessage) {
                this.UserMessage = userMessage;
            };
        };

        Initialize._InitializeMethods = function () {
            //TODO: this
            ex.registeredMethods["System.Collections.Generic.ICollection`1[[BackToFront.IViolation, BackToFront, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].Add"] = function (item) {
                if (!this || !this.push || this.push.constructor !== Function)
                    throw "This method must be called on an array" + "##";

                this.push(item);
            };

            // TODO: move to WebExpressions library
            ex.registeredMethods["System.String.IsNullOrEmpty"] = function (input) {
                return !input;
            };
            ex.registeredMethods["System.String.IsNullOrWhiteSpace"] = function (input) {
                return !input || !input.trim();
            };
        };
        Initialize._Initialized = false;
        return Initialize;
    })();
    BackToFront.Initialize = Initialize;
})(BackToFront || (BackToFront = {}));

BackToFront.Initialize.Bootstrap();


/// <reference path="../ref/jquery.d.ts" />
/// <reference path="../ref/jquery.validation.d.ts" />
/// <reference path="MetaClasses.ts" />
var BackToFront;
(function (BackToFront) {
    var Sanitizer = (function () {
        function Sanitizer() {
        }
        Sanitizer.Require = ///<summary>Class which deals with interactions with non ts code</summary>
        function (item) {
            var properties = [];
            for (var _i = 0; _i < (arguments.length - 1); _i++) {
                properties[_i] = arguments[_i + 1];
            }
            if (item == null) {
                throw "Item must have a value";
            }

            for (var i = 0, ii = properties.length; i < ii; i++) {
                var prop = properties[i];

                if (prop.allowNull && item[prop.inputName] == null)
                    return;
                if (!prop.allowNull && item[prop.inputName] == null)
                    throw "Property " + prop.inputName + " cannot be null";
                if (prop.inputType && typeof item[prop.inputName] !== prop.inputType)
                    throw "Property " + prop.inputName + " must be of type " + prop.inputType;
                if (prop.inputConstructor && item[prop.inputName].constructor !== prop.inputConstructor)
                    throw { message: "Property " + prop.inputName + " must be of a give type", type: prop.inputConstructor };
            }
        };
        return Sanitizer;
    })();
    BackToFront.Sanitizer = Sanitizer;
})(BackToFront || (BackToFront = {}));


/// <reference path="Validator.ts" />
/// <reference path="../Sanitizer.ts" />
/// <reference path="../../../../WebExpressions/Scripts/ref/Exports.ts" />
/// <reference path="../../ref/jquery.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
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
                var entity = {};

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
                            if (!entity[allNames[j]]) {
                                entity[allNames[j]] = null;
                            } else {
                                entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ? parseFloat(entity[allNames[j]]) : parseInt(entity[allNames[j]]);
                            }
                        }
                    }
                }

                return entity;
            };

            JQueryValidator.RegisterRule = function (rule) {
                BackToFront.Sanitizer.Require(rule, {
                    inputName: "Rules",
                    inputConstructor: Array
                }, {
                    inputName: "Entity",
                    inputConstructor: String
                });

                JQueryValidator.Registered.push(new JQueryValidator(rule.Rules, rule.Entity));
            };

            JQueryValidator.Setup = function () {
                if (!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }

                if (jQuery.validator.methods[JQueryValidator.ValidatorName])
                    return;

                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, JQueryValidator.HandlerErrors);

                if (jQuery.validator.unobtrusive && jQuery.validator.unobtrusive.adapters) {
                    jQuery.validator.unobtrusive.adapters.add(JQueryValidator.ValidatorName, [], JQueryValidator.AddOptions);
                }
            };

            JQueryValidator.HandlerErrors = // assume will be run onther the context of a jQuery validation
            function () {
                if (this["__BTFContext"] && this["__BTFContext"].Errors && this["__BTFContext"].Errors.length) {
                    return JQueryValidator.JoinErrors(linq(this["__BTFContext"].Errors).Select(function (a) {
                        return a.UserMessage;
                    }).Result);
                    // if string.format is needed for custom errors you can use this:
                    // jQuery.validator.format("These have been injected: {0}, {1}", "\"me\"", "\"and me\"");
                } else {
                    return undefined;
                }
            };

            JQueryValidator.AddOptions = function (options) {
                options.rules[JQueryValidator.ValidatorName] = options.params;
            };

            JQueryValidator.Validate = // assume will be run onther the context of a jQuery validation
            function (value, element) {
                var params = [];
                for (var _i = 0; _i < (arguments.length - 2); _i++) {
                    params[_i] = arguments[_i + 2];
                }
                var results = linq(JQueryValidator.Registered).Select(function (a) {
                    return a.Validate($(element).attr("name"), false);
                }).Aggregate();

                this["__BTFContext"] = new JqueryBTFContext(results.Result);
                return results.Result.length === 0;
            };

            JQueryValidator.JoinErrors = // default error message aggregation
            function (errors) {
                return errors.join("<br />");
            };
            JQueryValidator.Registered = [];
            JQueryValidator.ValidatorName = "backtofront";
            return JQueryValidator;
        })(BackToFront.Validation.Validator);
        Validation.JQueryValidator = JQueryValidator;
    })(BackToFront.Validation || (BackToFront.Validation = {}));
    var Validation = BackToFront.Validation;
})(BackToFront || (BackToFront = {}));


window["__BTF"] = BackToFront;
})();
