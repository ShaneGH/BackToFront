var ex = (function () {
    function ex() { }
    ex.createExpression = WebExpressions.Expression.CreateExpression;
    ex.registeredConstructors = WebExpressions.NewExpression.RegisteredTypes;
    ex.registeredMethods = WebExpressions.MethodCallExpression.RegisteredMethods;
    return ex;
})();
