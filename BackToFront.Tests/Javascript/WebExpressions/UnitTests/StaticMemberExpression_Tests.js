
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var splitNamespace = WebExpressions.StaticMemberExpression.SplitNamespace;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.StaticMemberExpression", {
    setup: function () {
        WebExpressions.StaticMemberExpression.SplitNamespace = splitNamespace;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.StaticMemberExpression.SplitNamespace = splitNamespace;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
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
    WebExpressions.StaticMemberExpression.SplitNamespace = function (input) {
        strictEqual(input, meta.Class);
        return split;
    };

    // act
    var actual = new WebExpressions.StaticMemberExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(split, actual.Class);
});

test("SplitNamespace test, ok", function () {
    var ex = new tUtil.Expect("require");

    // arrange
    // act
    var actual = new WebExpressions.StaticMemberExpression.SplitNamespace("aaa.bbb.ccc");

    // assert
    deepEqual(["aaa", "bbb", "ccc"], actual);
});

// Constructor test OK
test("_CompileMemberContext, ok", function () {

    // arrange
    window.aa = {
        bb: {
            cc: {}
        }
    };
    var subject = {
        Class: ["aa", "bb", "cc"]
    };

    // act
    var actual = WebExpressions.StaticMemberExpression.prototype._CompileMemberContext.call(subject);

    // assert
    strictEqual(actual(), window.aa.bb.cc);
});