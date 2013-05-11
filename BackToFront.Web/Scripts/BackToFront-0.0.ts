/// <reference path="linq.d.ts" />
/// <reference path="jquery.d.ts" />
/// <reference path="jquery.validation.d.ts" />
/// <reference path="AutoGen/MetaClasses.d.ts" />

if (window["__BTF"] != null) throw "BackToFront is defined already";

import M = __BTF.Meta;

module __BTF {
    export var Initialize = function (data) { };

    export class TestClass {
        Test() {
            return true;
        }
    }

    export module Expressions {
        export class Expression {
            NodeType: M.ExpressionType;
            ExpressionType: M.ExpressionWrapperType;

            constructor(meta: M.ExpressionMeta) {
                this.Required(meta, "NodeType", "ExpressionType");

                this.NodeType = meta.NodeType;
                this.ExpressionType = meta.ExpressionType;
            }

            Required(item: any, ...properties: string[]) {
                if (item == null) {
                    throw "Item must have a value";
                }

                for (var i = 0, ii = properties.length; i < ii; i++) {
                    if (item[properties[i]] == null) {
                        throw properties[i] + " cannot be null";
                    }
                }
            }

            Compile() : ExpressionInvoker {
                // child classes must implement this
                throw "Invalid operation";
            }

            static CreateExpression(meta: M.ExpressionMeta): Expression {
                switch (meta.ExpressionType) {
                    case M.ExpressionWrapperType.Binary:
                        return new BinaryExpression(<M.BinaryExpressionMeta>meta);
                    case M.ExpressionWrapperType.Block:
                        return new BlockExpression(<M.BlockExpressionMeta>meta);
                    case M.ExpressionWrapperType.Conditional:
                        return new ConditionalExpression(<M.ConditionalExpressionMeta>meta);
                    case M.ExpressionWrapperType.Constant:
                        return new ConstantExpression(<M.ConstantExpressionMeta>meta);
                    case M.ExpressionWrapperType.Default:
                        return new DefaultExpression(<M.ExpressionMeta>meta);
                    case M.ExpressionWrapperType.Member:
                        return new MemberExpression(<M.MemberExpressionMeta>meta);
                    case M.ExpressionWrapperType.MethodCall:
                        return new MethodCallExpression(<M.MethodCallExpressionMeta>meta);
                    case M.ExpressionWrapperType.Parameter:
                        return new ParameterExpression(<M.ParameterExpressionMeta>meta);
                    case M.ExpressionWrapperType.Unary:
                        return new UnaryExpression(<M.UnaryExpressionMeta>meta);
                }

                throw "Invalid expression type";
            }
        }

        export class BinaryExpression extends Expression {
            Left: Expression;
            Right: Expression;

            constructor(meta: M.BinaryExpressionMeta) {
                super(meta);

                this.Required(meta, "Left", "Right");

                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            }
        }

        export class BlockExpression extends Expression {
            Expressions: Expression[];

            constructor(meta: M.BlockExpressionMeta) {
                super(meta);

                this.Required(meta, "Expressions");
                this.Expressions = linq(meta.Expressions).Select(a => Expression.CreateExpression(a)).Result;
            }
        }

        export class ConditionalExpression extends Expression {
            IfTrue: Expression;
            IfFalse: Expression;
            Test: Expression;

            constructor(meta: M.ConditionalExpressionMeta) {
                super(meta);

                this.Required(meta, "IfTrue", "IfFalse", "Test");
                this.IfTrue = Expression.CreateExpression(meta.IfTrue);
                this.IfFalse = Expression.CreateExpression(meta.IfFalse);
                this.Test = Expression.CreateExpression(meta.Test);
            }
        }

        export class ConstantExpression extends Expression {
            constructor(meta: M.ConstantExpressionMeta) {
                super(meta);
            }
        }

        export class DefaultExpression extends Expression {
            constructor(meta: M.ExpressionMeta) {
                super(meta);
            }
        }

        export class MemberExpression extends Expression {
            Expression: Expression;
            MemberName: string;

            constructor(meta: M.MemberExpressionMeta) {
                super(meta);

                this.Required(meta, "Expression", "MemberName", "Test");
                this.Expression = Expression.CreateExpression(meta.Expression);
                this.MemberName = meta.MemberName;
            }
        }

        export class MethodCallExpression extends Expression {
            Object: Expression;
            Arguments: Expression[];
            MethodName: string;
            MethodFullName: string;

            constructor(meta: M.MethodCallExpressionMeta) {
                super(meta);

                this.Required(meta, "Object", "Arguments", "MethodName", "MethodFullName");
                this.Object = Expression.CreateExpression(meta.Object);
                this.Arguments = linq(meta.Arguments).Select(a => Expression.CreateExpression(a)).Result;
                this.MethodName = meta.MethodName;
                this.MethodFullName = meta.MethodFullName;
            }
        }

        export class ParameterExpression extends Expression {
            Name: string;

            constructor(meta: M.ParameterExpressionMeta) {
                super(meta);

                this.Required(meta, "Name");
                this.Name = meta.Name;
            }
        }

        export class UnaryExpression extends Expression {
            Operand: Expression;

            constructor(meta: M.UnaryExpressionMeta) {
                super(meta);

                this.Required(meta, "Expression", "MemberName", "Test");
                this.Operand = Expression.CreateExpression(meta.Operand);
            }
        }

        export class ExpressionInvoker {
        }
    }
}