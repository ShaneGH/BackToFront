var ex = (function () {
    function ex() { }
    ex.createExpression = function createExpression(meta) {
        return WebExpressions.Expression.CreateExpression(meta);
    };
    return ex;
})();
