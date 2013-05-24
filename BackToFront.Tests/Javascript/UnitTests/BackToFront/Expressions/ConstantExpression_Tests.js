
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

test("Warmup", function () { expect(0); });

(function (moduleName) {

    var createExpression = __BTF.Expressions.Expression.CreateExpression;
    var require = __BTF.Sanitizer.Require;

    module(moduleName, {
        setup: function () {
            __BTF.Expressions.Expression.CreateExpression = createExpression;
            __BTF.Sanitizer.Require = require;
        },
        teardown: function () {
            __BTF.Expressions.Expression.CreateExpression = createExpression;
            __BTF.Sanitizer.Require = require;
        }
    });

    // Constructor test OK
    (function (testName) {
        test(testName, function () {
            assert.ok(true);
        });
    })("Constructor test OK");

})("__BTF.Expressions.ConstantExpression");