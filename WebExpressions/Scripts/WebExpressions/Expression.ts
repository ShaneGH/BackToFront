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


module WebExpressions {

    export interface ExpressionInvokerAction {
        (ambientContext): any;
    }

    export interface CreateEvalExpression {
        Expression: string;
        Constants: WebExpressions.Utils.Dictionary;
    }

    export class Expression {
        NodeType: WebExpressions.Meta.ExpressionType;
        ExpressionType: WebExpressions.Meta.ExpressionWrapperType;

        constructor(meta: WebExpressions.Meta.ExpressionMeta) {
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

        private _Compiled: ExpressionInvokerAction;
        Compile(): ExpressionInvokerAction {
            if (!this._Compiled) {
                this._Compiled = this._Compile();
            }

            return this._Compiled;
        }

        //private _EvalCompiled: Function;
        //EvalCompile(): Function {
        //    if (!this._EvalCompiled) {
        //        var result = this.EvalExpression();
        //        var logic = new Function(WebExpressions.ConstantExpression.ConstantParameter, result.Expression);
        //        this._EvalCompiled = function () { return logic(result.Constants); };
        //    }

        //    return this._EvalCompiled;
        //}

        // abstract
        _Compile(): ExpressionInvokerAction {
            throw "Invalid operation";
        }

        // abstract
        //EvalExpression(): CreateEvalExpression {
        //    throw "Invalid operation";
        //}

        GetAffectedProperties(): string[] { return []; }

        static ExpressionConstructorDictionary = (function () {
            var dictionary = {};
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Binary] = meta => new WebExpressions.BinaryExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Block] = meta => new WebExpressions.BlockExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Conditional] = meta => new WebExpressions.ConditionalExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Constant] = meta => new WebExpressions.ConstantExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Default] = meta => new WebExpressions.DefaultExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.MemberX] = meta => new WebExpressions.MemberExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.StaticMember] = meta => new WebExpressions.StaticMemberExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.MethodCall] = meta => new WebExpressions.MethodCallExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Parameter] = meta => new WebExpressions.ParameterExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Unary] = meta => new WebExpressions.UnaryExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.Invocation] = meta => new WebExpressions.InvocationExpression(meta);
            dictionary[WebExpressions.Meta.ExpressionWrapperType.New] = meta => new WebExpressions.NewExpression(meta);

            return dictionary;
        })();

        static CreateExpression(meta: WebExpressions.Meta.ExpressionMeta): Expression {
            // special case for assignment
            if (meta.NodeType === WebExpressions.Meta.ExpressionType.Assign && meta.ExpressionType === WebExpressions.Meta.ExpressionWrapperType.Binary) {
                return new WebExpressions.AssignmentExpression(<Meta.BinaryExpressionMeta>meta);
            }

            if (Expression.ExpressionConstructorDictionary[meta.ExpressionType])
                return Expression.ExpressionConstructorDictionary[meta.ExpressionType](meta);

            throw "Invalid expression type";
        }
    }
}