
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.MemberExpression", {
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
    var ex = new tUtil.Expect("require", "expression");

    // arrange
    var meta = { MemberName: "askjhvdkahsvd", Expression: "expression", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip1 = true;
    var skip2 = true;
    WebExpressions.Sanitizer.Require = function (input1, input2) {
        if (skip1) {
            skip1 = false;
            return;
        }
        if (skip2) {
            skip2 = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.strictEqual(input2.inputName, "Expression");
        assert.strictEqual(input2.inputType, "object");
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.MemberExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("expressionXX", actual.Expression);
});

// Constructor test OK
test("_CompileMemberContext", function () {

    // arrange
    var expected = {};
    var subject = {
        Expression: {
            Compile: function () {
                return expected;
            }
        }
    };

    // act
    var actual = WebExpressions.MemberExpression.prototype._CompileMemberContext.call(subject);

    // assert
    strictEqual(actual, expected);
});