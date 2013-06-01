
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var dic = __BTF.Expressions.BinaryExpression.OperatorDictionary;

module("__BTF.Expressions.BinaryExpression.OperatorDictionary", {
    setup: function () {
    },
    teardown: function () {
    }
});

(function (testName) {
    test("Add", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.Add](1, 2), 3);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.Add]("1", "2"), "12");
    });

    test("AndAlso", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](true, true), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](true, false), false);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](false, true), false);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](false, false), false);
    });

    test("Divide", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.Divide](6, 2), 3);
    });

    test("GreaterThan", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThan](1, 2), false);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThan](2, 2), false);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThan](3, 2), true);
    });

    test("GreaterThanOrEqual", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThanOrEqual](1, 2), false);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThanOrEqual](2, 2), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThanOrEqual](3, 2), true);
    });

    test("LessThan", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThan](1, 2), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThan](2, 2), false);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThan](3, 2), false);
    });

    test("LessThanOrEqual", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThanOrEqual](1, 2), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThanOrEqual](2, 2), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThanOrEqual](3, 2), false);
    });

    test("Multiply", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.Multiply](6, 2), 12);
    });

    test("OrElse", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](true, true), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](true, false), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](false, true), true);
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](false, false), false);
    });

    test("Subtract", function () {
        assert.deepEqual(dic[__BTF.Meta.ExpressionType.Subtract](6, 2), 4);
    });

})("__BTF.Expressions.BinaryExpression.OperatorDictionary");