
/// <reference path="../WebExpressions/MetaClasses.ts" />
/// <reference path="../WebExpressions/Expression.ts" />

//TODO: rename
class ex {
    static createExpression(meta: WebExpressions.Meta.ExpressionMeta): WebExpressions.Expression {
        return WebExpressions.Expression.CreateExpression(meta);
    }

    static ns = WebExpressions;
}