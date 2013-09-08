
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var require = WebExpressions.Sanitizer.Require;
var splitNamespace = WebExpressions.Utils.CustomClassHandler.SplitNamespace;
var getClass = WebExpressions.Utils.CustomClassHandler.GetClass;

module("WebExpressions.StaticMethodCallExpression", {
    setup: function () {
        WebExpressions.Sanitizer.Require = require;
        WebExpressions.Utils.CustomClassHandler.SplitNamespace = splitNamespace;
        WebExpressions.Utils.CustomClassHandler.GetClass = getClass;
    },
    teardown: function () {
        WebExpressions.Sanitizer.Require = require;
        WebExpressions.StaticMemberExpression.SplitNamespace = splitNamespace;
        WebExpressions.Utils.CustomClassHandler.GetClass = getClass;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "class");

    // arrange
    var meta = { Arguments: [], MethodName: "mn", Class: "mfn", NodeType: WebExpressions.Meta.ExpressionType.Add };
    var expected = {};

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

        assert.strictEqual(input2.inputName, "Class");
        assert.strictEqual(input2.inputType, "string");
    };

    WebExpressions.Utils.CustomClassHandler.SplitNamespace = function (input) {
        ex.ExpectationReached.push("class");
        strictEqual(input, meta.Class);
        return expected;
    };

    // act
    var actual = new WebExpressions.StaticMethodCallExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(expected, actual.Class);
});

test("_CompileMethodCallContext test", function () {

    // arrange
    var expected = {};
    var subject = {
        Class: {}
    };
    WebExpressions.Utils.CustomClassHandler.GetClass = function (input) {
        strictEqual(input, subject.Class);
        return expected;
    };

    // act
    var actual = WebExpressions.StaticMethodCallExpression.prototype._CompileMethodCallContext.call(subject)();

    // assert
    strictEqual(actual, expected);
});