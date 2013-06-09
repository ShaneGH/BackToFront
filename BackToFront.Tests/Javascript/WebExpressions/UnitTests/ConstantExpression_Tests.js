
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var WebExpressions = ex.ns;

module("WebExpressions.ConstantExpression", {
    setup: function () {
    },
    teardown: function () {
    }
});

// Constructor test OK
test("Value test", function () {
    // arrange
    var meta = { Value: {}, ExpressionType: 2, NodeType: WebExpressions.Meta.ExpressionType.Add };
    // act
    // assert
    var subject = new WebExpressions.ConstantExpression(meta);
    assert.strictEqual(subject.Value, meta.Value);
    assert.strictEqual(subject.Compile()(), meta.Value);
});