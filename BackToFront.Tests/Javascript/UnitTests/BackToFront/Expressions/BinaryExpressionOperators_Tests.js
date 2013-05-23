
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

test("Warmup", function () { expect(0); });

(function (moduleName) {

    var dic = __BTF.Expressions.BinaryExpression.OperatorDictionary;

    module(moduleName, {
        setup: function () {
        },
        teardown: function () {
        }
    });

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.Add](1, 2), 3);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.Add]("1", "2"), "12");
        });
    })("Add");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](true, true), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](true, false), false);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](false, true), false);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.AndAlso](false, false), false);
        });
    })("AndAlso");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.Divide](6, 2), 3);
        });
    })("Divide");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThan](1, 2), false);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThan](2, 2), false);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThan](3, 2), true);
        });
    })("GreaterThan");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThanOrEqual](1, 2), false);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThanOrEqual](2, 2), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.GreaterThanOrEqual](3, 2), true);
        });
    })("GreaterThanOrEqual");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThan](1, 2), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThan](2, 2), false);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThan](3, 2), false);
        });
    })("LessThan");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThanOrEqual](1, 2), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThanOrEqual](2, 2), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.LessThanOrEqual](3, 2), false);
        });
    })("LessThanOrEqual");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.Multiply](6, 2), 12);
        });
    })("Multiply");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](true, true), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](true, false), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](false, true), true);
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.OrElse](false, false), false);
        });
    })("OrElse");

    (function (testName) {
        test(testName, function () {
            assert.deepEqual(dic[__BTF.Meta.ExpressionType.Subtract](6, 2), 4);
        });
    })("Subtract");

})("__BTF.Expressions.BinaryExpression.OperatorDictionary");