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
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        export class Expression {
            NodeType: Meta.ExpressionType;
            ExpressionType: Meta.ExpressionWrapperType;

            constructor(meta: Meta.ExpressionMeta) {
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

            private _Compiled: Validation.ExpressionInvokerAction;
            Compile(): Validation.ExpressionInvokerAction {
                if (!this._Compiled) {
                    var compiled = this._Compile();
                    this._Compiled = (item, context) => {
                        if (!context.Break())
                            compiled(item, context);
                    };
                }

                return this._Compiled;
            }

            _Compile(): Validation.ExpressionInvokerAction {
                // child classes must override this
                throw "Invalid operation";
            }

            GetAffectedProperties(): string[] { return []; }

            static CreateExpression(meta: Meta.ExpressionMeta): Expression {
                switch (meta.ExpressionType) {
                    case Meta.ExpressionWrapperType.Binary:
                        return new BinaryExpression(<Meta.BinaryExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Block:
                        return new BlockExpression(<Meta.BlockExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Conditional:
                        return new ConditionalExpression(<Meta.ConditionalExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Constant:
                        return new ConstantExpression(<Meta.ConstantExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Default:
                        return new DefaultExpression(<Meta.ExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Member:
                        return new MemberExpression(<Meta.MemberExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.MethodCall:
                        return new MethodCallExpression(<Meta.MethodCallExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Parameter:
                        return new ParameterExpression(<Meta.ParameterExpressionMeta>meta);
                    case Meta.ExpressionWrapperType.Unary:
                        return new UnaryExpression(<Meta.UnaryExpressionMeta>meta);
                }

                throw "Invalid expression type";
            }
        }
    }
}