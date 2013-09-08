
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var property = WebExpressions.MemberExpressionBase.PropertyRegex;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.MemberExpressionBase", {
    setup: function () {
        WebExpressions.MemberExpressionBase.PropertyRegex = property;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.MemberExpressionBase.PropertyRegex = property;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    }
});

// Constructor test OK
test("PropertyRegex tests", function () {

    // arrange
    var rx = WebExpressions.MemberExpressionBase.PropertyRegex;

    // act
    // assert
    assert.ok(rx.test("LBLGPG"));
    assert.ok(rx.test("LBL_GPG"));
    assert.ok(rx.test("_LBLGPG"));
    assert.ok(rx.test("LB9LGPG"));
    assert.ok(rx.test("_0LBLGPG"));

    assert.ok(!rx.test("9LBLGPG"));
    assert.ok(!rx.test("LBLG(PG"));
    assert.ok(!rx.test("LBLG=PG"));
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require");

    // arrange
    var meta = { MemberName: "askjhvdkahsvd", Expression: "expression", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.strictEqual(input2.inputName, "MemberName");
        assert.strictEqual(input2.inputConstructor, String);
    };

    // act
    var actual = new WebExpressions.MemberExpressionBase(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(meta.MemberName, actual.MemberName);
});

test("_Compile test: exception", function () {
    var ex = new tUtil.Expect("test");

    var subject = {
        MemberName: "LJBHKLJBLKJB"
    };

    WebExpressions.MemberExpressionBase.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MemberName);
            return false;
        }
    };

    assert.throws(function () {
        WebExpressions.MemberExpressionBase.prototype._Compile.call(subject);
    });

    ex.VerifyOrderedExpectations();
});

// _Compile test
test("_Compile test: ok", function () {
    var ex = new tUtil.Expect("test", "innerCompile", "innerCall", "innerCall");
    var memberName = "LKJHLKJH";

    var context = {};

    var expected = {};

    var subject = {
        MemberName: memberName,
        _CompileMemberContext: function () {
            ex.At("innerCompile");
            return function (ctxt) {
                ex.At("innerCall");
                assert.strictEqual(ctxt, context);

                var output = {};
                output[memberName] = expected;
                return output;
            };
        }
    };

    WebExpressions.MemberExpressionBase.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MemberName);
            return true;
        }
    };

    var compiled = WebExpressions.MemberExpressionBase.prototype._Compile.call(subject);
    assert.strictEqual(expected, compiled(context));
    assert.strictEqual(expected, compiled(context));

    ex.VerifyOrderedExpectations();
});