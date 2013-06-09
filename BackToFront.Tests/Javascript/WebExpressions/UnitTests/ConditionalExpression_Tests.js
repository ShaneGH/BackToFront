
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var WebExpressions = ex.ns;

var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.ConditionalExpression", {
    setup: function () {
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "true", "false", "test");

    // arrange
    var meta = { IfTrue: "true", IfFalse: "false", Test: "test", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3, input4) {
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

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.ConditionalExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("trueXX", actual.IfTrue);
    assert.deepEqual("falseXX", actual.IfFalse);
    assert.deepEqual("testXX", actual.Test);
});

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
    var result = WebExpressions.ConditionalExpression.prototype._Compile.call(_this);

    // assert
    assert.deepEqual(result(context), test);
    ex.VerifyOrderedExpectations();
}

test("_Compile test: true", function () {
    compileTest(true);
});

test("_Compile test: false", function () {
    compileTest(false);
});

