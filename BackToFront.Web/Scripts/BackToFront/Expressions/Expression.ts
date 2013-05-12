/// <reference path="../../ref/linq.d.ts" />
/// <reference path="../../ref/jquery.d.ts" />
/// <reference path="../../ref/jquery.validation.d.ts" />
/// <reference path="../MetaClasses.ts" />
/// <reference path="../Sanitizer.ts" />

/// <reference path="BinaryExpression.ts" />
/// <reference path="BlockExpression.ts" />
/// <reference path="ConditionalExpression.ts" />
/// <reference path="ConstantExpression.ts" />
/// <reference path="DefaultExpression.ts" />
/// <reference path="MemberExpression.ts" />
/// <reference path="MethodCallExpression.ts" />
/// <reference path="ParameterExpression.ts" />
/// <reference path="UnaryExpression.ts" />


/// <reference path="../Validation/ValidationContext.ts" />
/// <reference path="../Validation/ExpressionInvoker.ts" />


module __BTF {

    export module Expressions {
        export class Expression {
            NodeType: __BTF.Meta.ExpressionType;
            ExpressionType: __BTF.Meta.ExpressionWrapperType;

            constructor(meta: __BTF.Meta.ExpressionMeta) {
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

            private _Compiled: __BTF.Validation.ExpressionInvokerAction;
            Compile(): __BTF.Validation.ExpressionInvokerAction {
                if (!this._Compiled) {
                    var compiled = this._Compile();
                    this._Compiled = (item, context) => {
                        if (!context.Break())
                            compiled(item, context);
                    };
                }

                return this._Compiled;
            }

            // abstract
            _Compile(): __BTF.Validation.ExpressionInvokerAction {
                throw "Invalid operation";
            }

            GetAffectedProperties(): string[]{ return []; }

            static ExpressionConstructorDictionary = (function () {
                var dictionary = {};
                dictionary[__BTF.Meta.ExpressionWrapperType.Binary] = meta => new __BTF.Expressions.BinaryExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Block] = meta => new __BTF.Expressions.BlockExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Conditional] = meta => new __BTF.Expressions.ConditionalExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Constant] = meta => new __BTF.Expressions.ConstantExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Default] = meta => new __BTF.Expressions.DefaultExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Member] = meta => new __BTF.Expressions.MemberExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.MethodCall] = meta => new __BTF.Expressions.MethodCallExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Parameter] = meta => new __BTF.Expressions.ParameterExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Unary] = meta => new __BTF.Expressions.UnaryExpression(meta);
                dictionary[__BTF.Meta.ExpressionWrapperType.Invocation] = meta => new __BTF.Expressions.InvocationExpression(meta);

                return dictionary;                
            })();

            static CreateExpression(meta: __BTF.Meta.ExpressionMeta): Expression {
                if (Expression.ExpressionConstructorDictionary[meta.ExpressionType])
                    return Expression.ExpressionConstructorDictionary[meta.ExpressionType](meta);

                throw "Invalid expression type";
            }
        }
    }
}