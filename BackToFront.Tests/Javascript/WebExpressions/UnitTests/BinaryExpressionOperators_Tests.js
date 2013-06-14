
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var dic = WebExpressions.BinaryExpression.OperatorDictionary;

module("WebExpressions.BinaryExpression.OperatorDictionary", {
    setup: function () {
    },
    teardown: function () {
    }
});

(function (testName) {
    test("Add", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Add](1, 2), 3);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Add]("1", "2"), "12");
    });

    test("AndAlso", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.AndAlso](true, true), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.AndAlso](true, false), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.AndAlso](false, true), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.AndAlso](false, false), false);
    });

    test("Divide", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Divide](6, 2), 3);
    });

    test("Equal", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Equal](2, 2), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Equal]("2", "2"), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Equal](2, "2"), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Equal](2, 3), false);
    });

    test("GreaterThan", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.GreaterThan](1, 2), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.GreaterThan](2, 2), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.GreaterThan](3, 2), true);
    });

    test("GreaterThanOrEqual", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual](1, 2), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual](2, 2), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual](3, 2), true);
    });

    test("LessThan", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.LessThan](1, 2), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.LessThan](2, 2), false);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.LessThan](3, 2), false);
    });

    test("LessThanOrEqual", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.LessThanOrEqual](1, 2), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.LessThanOrEqual](2, 2), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.LessThanOrEqual](3, 2), false);
    });

    test("Multiply", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Multiply](6, 2), 12);
    });

    test("OrElse", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.OrElse](true, true), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.OrElse](true, false), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.OrElse](false, true), true);
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.OrElse](false, false), false);
    });

    test("Subtract", function () {
        assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Subtract](6, 2), 4);
    });

})("WebExpressions.BinaryExpression.OperatorDictionary");
