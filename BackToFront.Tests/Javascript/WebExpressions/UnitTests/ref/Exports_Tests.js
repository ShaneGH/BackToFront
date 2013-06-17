
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

module("ex", {
    setup: function () {
    },
    teardown: function () {
    }
});

test("Constants test", function () {

    // arrange
    // act
    // assert
    assert.strictEqual(ex.createExpression, WebExpressions.Expression.CreateExpression);
    assert.strictEqual(ex.registeredConstructors, WebExpressions.NewExpression.RegisteredTypes);
    assert.strictEqual(ex.registeredMethods, WebExpressions.MethodCallExpression.RegisteredMethods);
});