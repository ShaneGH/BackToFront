
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var WebExpressions = ex.ns;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.BlockExpression", {
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
    var ex = new tUtil.Expect("require", "ex1", "ex2");

    // arrange
    var meta = { Expressions: ["ex1", "ex2"], NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Expressions");
        assert.deepEqual(input2.inputConstructor, Array);
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.BlockExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(2, actual.Expressions.length);
    assert.deepEqual("ex1XX", actual.Expressions[0]);
    assert.deepEqual("ex2XX", actual.Expressions[1]);
});

// _Compile test
test("_Compile test", function () {
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
    var result = WebExpressions.BlockExpression.prototype._Compile.call(_this);

    // assert
    result(context);
    ex.VerifyOrderedExpectations();
});
