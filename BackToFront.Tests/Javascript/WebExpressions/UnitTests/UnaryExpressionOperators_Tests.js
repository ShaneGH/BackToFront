
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var dic = WebExpressions.UnaryExpression.OperatorDictionary;

module("WebExpressions.BinaryExpression.OperatorDictionary", {
    setup: function () {
    },
    teardown: function () {
    }
});

test("Convert", function () {
    var toConvert = {};
    assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Convert](toConvert), toConvert);
});

test("Not", function () {
    assert.strictEqual(dic[WebExpressions.Meta.ExpressionType.Not](true), false);
});