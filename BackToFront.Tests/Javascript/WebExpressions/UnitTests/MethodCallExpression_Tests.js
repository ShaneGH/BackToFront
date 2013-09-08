
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.MethodCallExpression", {
    setup: function () {
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.Sanitizer.Require = require;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "obj");

    // arrange
    var meta = { Object: "obj", Arguments: ["arg1", "arg2"], MethodName: "mn", MethodFullName: "mfn", NodeType: WebExpressions.Meta.ExpressionType.Add };

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

        assert.strictEqual(input2.inputName, "Object");
        assert.strictEqual(input2.inputType, "object");
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        if (input === meta.Object)
            ex.ExpectationReached.push(input);

        return input + "XX";
    };

    // act
    var actual = new WebExpressions.MethodCallExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("objXX", actual.Object);
    assert.deepEqual(2, actual.Arguments.length);
});

test("_CompileMethodCallContext test", function () {

    // arrange
    var expected = {};
    var subject = {
        Object: {
            Compile: function () { return expected; }
        }
    };

    // act
    var actual = WebExpressions.MethodCallExpression.prototype._CompileMethodCallContext.call(subject);

    // assert
    strictEqual(actual, expected);
});