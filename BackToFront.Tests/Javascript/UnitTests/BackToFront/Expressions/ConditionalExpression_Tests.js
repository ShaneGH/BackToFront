
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
            var ex = new tUtil.Expect("require", "true", "false", "test");

            // arrange
            var meta = { IfTrue: "true", IfFalse: "false", Test: "test", NodeType: __BTF.Meta.ExpressionType.Add };

            // first Sanitizer is in parent class
            var skip = true;
            __BTF.Sanitizer.Require = function (input1, input2, input3, input4) {
                if (skip) {
                    skip = false;
                    return;
                }

                ex.ExpectationReached.push("require");

                assert.strictEqual(input1, meta);

                assert.deepEqual(input2.inputName, "IfTrue");
                assert.deepEqual(input2.inputType, "object");

                assert.deepEqual(input3.inputName, "IfFalse");
                assert.deepEqual(input3.inputType, "object");

                assert.deepEqual(input4.inputName, "Test");
                assert.deepEqual(input4.inputType, "object");
            };

            __BTF.Expressions.Expression.CreateExpression = function (input) {
                ex.ExpectationReached.push(input);
                return input + "XX";
            };

            // act
            var actual = new __BTF.Expressions.ConditionalExpression(meta);

            // assert
            ex.VerifyOrderedExpectations();
            assert.deepEqual("trueXX", actual.IfTrue);
            assert.deepEqual("falseXX", actual.IfFalse);
            assert.deepEqual("testXX", actual.Test);
        });
    })("Constructor test OK");

    // _Compile test
    var compileTest = function (test) {
        var ex = new tUtil.Expect("test", "true", "false");

        var context = {};

        // arrange
        var _this = {
            IfTrue: {
                Compile: function () {
                    ex.ExpectationReached.push("true");
                    return function (ctxt) {
                        assert.strictEqual(ctxt, context);
                        return true;
                    }
                }
            },
            IfFalse: {
                Compile: function () {
                    ex.ExpectationReached.push("false");
                    return function (ctxt) {
                        assert.strictEqual(ctxt, context);
                        return false;
                    }
                }
            },
            Test: {
                Compile: function () {
                    ex.ExpectationReached.push("test");
                    return function (ctxt) {
                        assert.strictEqual(ctxt, context);
                        return test;
                    }
                }
            }
        };

        // act
        var result = __BTF.Expressions.ConditionalExpression.prototype._Compile.call(_this);

        // assert
        assert.deepEqual(result(context), test);
        ex.VerifyOrderedExpectations();
    }

    (function (testName) {
        test(testName, function () {
            compileTest(true);
        });
    })("_Compile test: true");

    (function (testName) {
        test(testName, function () {
            compileTest(false);
        });
    })("_Compile test: false");
})("__BTF.Expressions.ConditionalExpression");