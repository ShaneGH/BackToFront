
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

module("__BTF.Expressions.ConstantExpression", {
    setup: function () {
    },
    teardown: function () {
    }
});

// Constructor test OK
test(testName, function () {
    // arrange
    var meta = { Value: {}, ExpressionType: 2, NodeType: __BTF.Meta.ExpressionType.Add };
    // act
    // assert
    var subject = new __BTF.Expressions.ConstantExpression(meta);
    assert.strictEqual(subject.Value, meta.Value);
    assert.strictEqual(subject.Compile()(), meta.Value);
});