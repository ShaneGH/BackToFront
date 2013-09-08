
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var splitNamespace = WebExpressions.Utils.CustomClassHandler.SplitNamespace;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;
var getClass = WebExpressions.Utils.CustomClassHandler.GetClass;

module("WebExpressions.StaticMemberExpression", {
    setup: function () {
        WebExpressions.Utils.CustomClassHandler.SplitNamespace = splitNamespace;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
        WebExpressions.Utils.CustomClassHandler.GetClass = getClass;
    },
    teardown: function () {
        WebExpressions.Utils.CustomClassHandler.SplitNamespace = splitNamespace;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
        WebExpressions.Utils.CustomClassHandler.GetClass = getClass;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require");

    // arrange
    var meta = { MemberName: "askjhvdkahsvd", Class: "class.class1.class2", NodeType: WebExpressions.Meta.ExpressionType.Add };

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

    var split = {};
    WebExpressions.Utils.CustomClassHandler.SplitNamespace = function (input) {
        strictEqual(input, meta.Class);
        return split;
    };

    // act
    var actual = new WebExpressions.StaticMemberExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(split, actual.Class);
});

// Constructor test OK
test("_CompileMemberContext, ok", function () {

    // arrange
    var expected = {};
    var subject = { Class: "HELLO" };
    WebExpressions.Utils.CustomClassHandler.GetClass = function (input) {
        strictEqual(input, subject.Class);
        return expected;
    };

    // act
    var actual = WebExpressions.StaticMemberExpression.prototype._CompileMemberContext.call(subject)();

    // assert
    strictEqual(expected, actual);
});