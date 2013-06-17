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
            if(meta.IsAnonymous) {
                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Members",
                    inputConstructor: Array
                });
                if(meta.Members.length !== meta.Arguments.length) {
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
            if(this.IsAnonymous) {
                return {
                    Constants: constants,
                    Expression: "{ " + argsString + " }"
                };
            } else if(NewExpression.RegisteredTypes[this.Type]) {
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
                if(_this.IsAnonymous) {
                    return _this.ConstructAnonymous(params);
                } else if(NewExpression.RegisteredTypes[_this.Type]) {
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
            for(var i = 0, ii = this.Members.length; i < ii; i++) {
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