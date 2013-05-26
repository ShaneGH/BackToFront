
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

test("Warmup", function () { expect(0); });

(function (moduleName) {

    module(moduleName, {
        setup: function () {
        },
        teardown: function () {
        }
    });

    // Constructor test OK
    (function (testName) {
        test(testName, function () {
            // arrange
            var meta = { Value: {}, ExpressionType: 2, NodeType: __BTF.Meta.ExpressionType.Add };
            // act
            // assert
            var subject = new __BTF.Expressions.ConstantExpression(meta);
            assert.strictEqual(subject.Value, meta.Value);
            assert.strictEqual(subject.Compile()(), meta.Value);
        });
    })("Constructor and compile test");

})("__BTF.Expressions.ConstantExpression");