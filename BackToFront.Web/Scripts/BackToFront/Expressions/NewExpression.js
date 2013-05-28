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
                    inputName: "Type",
                    inputConstructor: String
                });
                if(meta.Members) {
                    __BTF.Sanitizer.Require(meta, {
                        inputName: "Members",
                        inputConstructor: Array
                    });
                    if(meta.Members.length && meta.Members.length !== meta.Arguments.length) {
                        throw "If members are defined, each must have a corresponding argument";
                    }
                }
                this.Arguments = linq(meta.Arguments).Select(function (a) {
                    return Expressions.Expression.CreateExpression(a);
                }).Result;
                this.Members = meta.Members;
                this.Type = meta.Type;
            }
            NewExpression.prototype._Compile = function () {
                var _this = this;
                var construct = NewExpression.RegisteredTypes[this.Type] ? NewExpression.RegisteredTypes[this.Type] : Object;
                var args = linq(this.Arguments).Select(function (a) {
                    return a.Compile();
                }).Result;
                return function (ambientContext) {
                    var params = linq(args).Select(function (a) {
                        return a(ambientContext);
                    }).Result;
                    if(_this.Members && _this.Members.length) {
                        return _this.ConstructAnonymous(params);
                    } else {
                        return _this.Construct(construct, params);
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
