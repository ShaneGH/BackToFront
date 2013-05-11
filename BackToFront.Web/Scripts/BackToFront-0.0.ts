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

                var failure = linq(properties).First(a => item[a] == null);
                if (failure == null) {
                    throw failure + " cannot be null";
                }
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
                // child classes must implement this
                throw "Invalid operation";
            }

            GetAffectedProperties(): string[] { return []; }

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

            private static OperatorDictionary: { (left, right): any; }[] = [];

            constructor(meta: M.BinaryExpressionMeta) {
                super(meta);

                this.Required(meta, "Left", "Right");
                if (!BinaryExpression.OperatorDictionary[this.NodeType])
                    throw "Invalid Operator";

                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            }

            _Compile(): Validation.ExpressionInvokerAction {
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return (namedArguments, context) => BinaryExpression.OperatorDictionary[this.NodeType](left(namedArguments, context), right(namedArguments, context))
            }
        }

        export class BlockExpression extends Expression {
            Expressions: Expression[];

            constructor(meta: M.BlockExpressionMeta) {
                super(meta);

                this.Required(meta, "Expressions");
                this.Expressions = linq(meta.Expressions).Select(a => Expression.CreateExpression(a)).Result;
            }

            _Compile(): Validation.ExpressionInvokerAction {
                var children = linq(this.Expressions).Select(a => a.Compile()).Result;
                return (namedArguments, context) => linq(children).Each(a => a(namedArguments, context));
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

            _Compile(): Validation.ExpressionInvokerAction {
                var test = this.Test.Compile();
                var ifTrue = this.IfTrue.Compile();
                var ifFalse = this.IfFalse.Compile();

                return (namedArguments, context) => test(namedArguments, context) ? ifTrue(namedArguments, context) : ifFalse(namedArguments, context);
            }
        }

        export class ConstantExpression extends Expression {
            constructor(meta: M.ConstantExpressionMeta) {
                super(meta);
            }

            //TODO
            _Compile(): Validation.ExpressionInvokerAction {
                return (namedArguments, context) => null;
            }
        }

        export class DefaultExpression extends Expression {
            constructor(meta: M.ExpressionMeta) {
                super(meta);
            }

            //TODO
            _Compile(): Validation.ExpressionInvokerAction {
                return (namedArguments, context) => null;
            }
        }

        export interface MemberExpressionRegex {
            Property: RegExp;
            IndexedProperty: RegExp;
        }

        export class MemberExpression extends Expression {
            static RegexValues: MemberExpressionRegex = (() => {
                var index = "\\[[0-9]+\\]";
                var indexedProperty = "[_a-zA-Z][_a-zA-Z0-9]*(" + index + ")?";

                return {
                    IndexedProperty: new RegExp(index + "$"),
                    Property: new RegExp("^" + indexedProperty + "$")
                };
            })();

            Expression: Expression;
            MemberName: string;
            
            constructor(meta: M.MemberExpressionMeta) {
                super(meta);

                this.Required(meta, "Expression", "MemberName", "Test");
                this.Expression = Expression.CreateExpression(meta.Expression);
                this.MemberName = meta.MemberName;
            }

            // TODO: taking pessimistic view of properties with regards to arrays
            // TODO: not throwing null exceptions
            // TODO: not validating at compile time, but at run time
            _Compile(): Validation.ExpressionInvokerAction {
                var expression = this.Expression.Compile();
                
                return (namedArguments, context) => {
                    if (!MemberExpression.RegexValues.Property.test(this.MemberName)) {
                        throw "Invalid property name: " + this.MemberName;
                    }

                    var base = expression(namedArguments, context);
                    if (MemberExpression.RegexValues.IndexedProperty.test(this.MemberName)) {
                        var property = this.MemberName.substr(0, this.MemberName.indexOf("[") - 1);
                        var index: any = MemberExpression.RegexValues.Property.exec(this.MemberName)[0];
                        index = parseInt(index.substring(1, index.length - 1));

                        base = base[property];
                        if (base == null) return null;
                        return base[index];
                    }

                    return base[this.MemberName];
                };
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

            // TODO
            _Compile(): Validation.ExpressionInvokerAction {
                throw "Not implemented";
            }

        }

        export class ParameterExpression extends Expression {
            Name: string;

            constructor(meta: M.ParameterExpressionMeta) {
                super(meta);

                this.Required(meta, "Name");
                this.Name = meta.Name;
            }

            _Compile(): Validation.ExpressionInvokerAction {
                return (namedArguments, context) => namedArguments[this.Name];
            }
        }

        export class UnaryExpression extends Expression {

            private static OperatorDictionary: { (operand): any; }[] = [];

            Operand: Expression;

            constructor(meta: M.UnaryExpressionMeta) {
                super(meta);

                this.Required(meta, "Operand");
                this.Operand = Expression.CreateExpression(meta.Operand);
            }

            _Compile(): Validation.ExpressionInvokerAction {
                var operand = this.Operand.Compile();
                return (namedArguments, context) => UnaryExpression.OperatorDictionary[this.NodeType](operand(namedArguments, context))
            }
        }
    }

    export module Validation {

        export interface ExpressionInvokerAction {
            (namedArguments: any, context: ValidationContext): any;
        }

        export class ExpressionInvoker {
            constructor(public Logic: ExpressionInvokerAction, public AffectedProperties: string[]) { }
        }

        export class ValidationContext {
            // TODO
            Break(): bool { return false; }
        }
    }
}