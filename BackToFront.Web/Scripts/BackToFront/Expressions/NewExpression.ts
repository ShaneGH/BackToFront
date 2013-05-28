
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    export module Expressions {

        export class NewExpression extends Expression {
            Arguments: Expression[];
            Members: string[];
            Type: string;

            constructor(meta: Meta.NewExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Arguments",
                    inputConstructor: Array
                }, {
                    inputName: "Type",
                    inputConstructor: String
                });

                if (meta.Members) {
                    __BTF.Sanitizer.Require(meta, {
                        inputName: "Members",
                        inputConstructor: Array
                    });

                    if (meta.Members.length && meta.Members.length !== meta.Arguments.length)
                        throw "If members are defined, each must have a corresponding argument";
                }

                this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
                this.Members = meta.Members;
                this.Type = meta.Type;
            }

            // TODO: register types
            // TODO: optomistic construction, what to do with arguments
            _Compile(): Validation.ExpressionInvokerAction {
                var construct = NewExpression.RegisteredTypes[this.Type] ?
                    NewExpression.RegisteredTypes[this.Type] :
                    Object;

                var args = linq(this.Arguments).Select(a => a.Compile()).Result;
                return (ambientContext) => {
                    var params = linq(args).Select(a => a(ambientContext)).Result;
                    //TODO better way of determining if it is anonymous constructor
                    if (this.Members && this.Members.length)
                        return this.ConstructAnonymous(params);
                    else
                        return this.Construct(construct, params);
                };
            }

            ConstructAnonymous(params: any[]) {
                var obj = {};
                for (var i = 0, ii = this.Members.length; i < ii; i++)
                    obj[this.Members[i]] = params[i];

                return obj;
            }

            Construct(constr: Function, args: any[]) {
                var obj = Object.create(constr.prototype);
                var result = constr.apply(obj, args);

                //TODO: this
                return typeof result === 'object' ? result : obj;
            }

            static RegisteredTypes: Object = {};
        }
    }
}