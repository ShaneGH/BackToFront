
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var dic = __BTF.Expressions.UnaryExpression.OperatorDictionary;

module("__BTF.Expressions.BinaryExpression.OperatorDictionary", {
    setup: function () {
    },
    teardown: function () {
    }
});

test("Convert", function () {
    var toConvert = {};
    assert.strictEqual(dic[__BTF.Meta.ExpressionType.Convert](toConvert), toConvert);
});

test("Not", function () {
    assert.strictEqual(dic[__BTF.Meta.ExpressionType.Not](true), false);
});