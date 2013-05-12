/// <reference path="linq.d.ts" />
/// <reference path="jquery.d.ts" />
/// <reference path="jquery.validation.d.ts" />
/// <reference path="MetaClasses.ts" />
/// <reference path="BackToFront.Core.ts" />
/// <reference path="BackToFront.Validation.ts" />


module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        export class Expression {
            NodeType: Meta.ExpressionType;
            ExpressionType: Meta.ExpressionWrapperType;

            constructor(meta: Meta.ExpressionMeta) {
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

        export class BinaryExpression extends Expression {
            Left: Expression;
            Right: Expression;

            private static OperatorDictionary: { (left, right): any; }[] = [];

            constructor(meta: Meta.BinaryExpressionMeta) {
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

            constructor(meta: Meta.BlockExpressionMeta) {
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

            constructor(meta: Meta.ConditionalExpressionMeta) {
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
            constructor(meta: Meta.ConstantExpressionMeta) {
                super(meta);
            }

            //TODO
            _Compile(): Validation.ExpressionInvokerAction {
                return (namedArguments, context) => null;
            }
        }

        export class DefaultExpression extends Expression {
            constructor(meta: Meta.ExpressionMeta) {
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

            constructor(meta: Meta.MemberExpressionMeta) {
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

            constructor(meta: Meta.MethodCallExpressionMeta) {
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

            constructor(meta: Meta.ParameterExpressionMeta) {
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

            constructor(meta: Meta.UnaryExpressionMeta) {
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
}