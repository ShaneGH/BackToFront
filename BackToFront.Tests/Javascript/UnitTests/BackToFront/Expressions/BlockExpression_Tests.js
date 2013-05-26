
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
            var ex = new tUtil.Expect("require", "ex1", "ex2");

            // arrange
            var meta = { Expressions: ["ex1", "ex2"], NodeType: __BTF.Meta.ExpressionType.Add };

            // first Sanitizer is in parent class
            var skip = true;
            __BTF.Sanitizer.Require = function (input1, input2) {
                if (skip) {
                    skip = false;
                    return;
                }

                ex.ExpectationReached.push("require");

                assert.strictEqual(input1, meta);

                assert.deepEqual(input2.inputName, "Expressions");
                assert.deepEqual(input2.inputConstructor, Array);
            };

            __BTF.Expressions.Expression.CreateExpression = function (input) {
                ex.ExpectationReached.push(input);
                return input + "XX";
            };

            // act
            var actual = new __BTF.Expressions.BlockExpression(meta);

            // assert
            ex.VerifyOrderedExpectations();
            assert.deepEqual(2, actual.Expressions.length);
            assert.deepEqual("ex1XX", actual.Expressions[0]);
            assert.deepEqual("ex2XX", actual.Expressions[1]);
        });
    })("Constructor test OK");

    // _Compile test
    (function (testName) {
        test(testName, function () {
            var ex = new tUtil.Expect("ex1", "ex2");
            
            var context = {};

            // arrange
            var _this = {
                Expressions: [
                {
                    Compile: function () {
                        return function (ctxt) {
                            ex.ExpectationReached.push("ex1");
                            assert.strictEqual(ctxt, context);
                            return "ex1";
                        }
                    }
                }, {
                    Compile: function () {
                        return function (ctxt) {
                            ex.ExpectationReached.push("ex2");
                            assert.strictEqual(ctxt, context);
                            return "ex2";
                        }
                    }
                }]
            };

            // act
            var result = __BTF.Expressions.BlockExpression.prototype._Compile.call(_this);

            // assert
            result(context);
            ex.VerifyOrderedExpectations();
        });
    })("_Compile test");
})("__BTF.Expressions.BlockExpression");