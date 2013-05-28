
///// <reference path="../Sanitizer.ts" />
///// <reference path="Expression.ts" />

//module __BTF {
//    export module Expressions {

//        export class NewExpression extends Expression {
//            Arguments: Expression[];
//            Members: string[];
//            Type: string;

//            constructor(meta: Meta.NewExpressionMeta) {
//                super(meta);

//                __BTF.Sanitizer.Require(meta, {
//                    inputName: "Arguments",
//                    inputConstructor: Array
//                }, {
//                    inputName: "Members",
//                    inputConstructor: Array
//                }, {
//                    inputName: "Type",
//                    inputConstructor: String
//                });

//                this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
//                this.Members = meta.Members;
//                this.Type = meta.Type;
//            }

//            // TODO: register types
//            // TODO: optomistic construction, what to do with arguments
//            _Compile(): Validation.ExpressionInvokerAction {
//                var construct = NewExpression.RegisteredTypes[this.Type] ?
//                    NewExpression.RegisteredTypes[this.Type] :
//                    Object;

//                var args = linq(this.Arguments).Select(a => a.Compile()).Result;
//                return (ambientContext) => {
//                    var params = linq(args).Select(a => a(ambientContext)).Result;

//                    return NewExpression.Construct(construct, params);
//                };
//            }

//            static Construct(constr: Function, args: any[]) {
//                var obj = Object.create(constr.prototype);
//                var result = constr.apply(obj, args);

//                //TODO: this
//                return typeof result === 'object' ? result : obj;
//            }

//            static RegisteredTypes: Object = {};
//        }
//    }
//}