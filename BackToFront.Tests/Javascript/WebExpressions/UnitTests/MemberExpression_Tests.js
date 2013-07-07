
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var property = WebExpressions.MemberExpression.PropertyRegex;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.MemberExpression", {
    setup: function () {
        WebExpressions.MemberExpression.PropertyRegex = property;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.MemberExpression.PropertyRegex = property;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    }
});

// Constructor test OK
test("PropertyRegex tests", function () {

    // arrange
    var rx = WebExpressions.MemberExpression.PropertyRegex;

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
    var ex = new tUtil.Expect("require", "expression");

    // arrange
    var meta = { MemberName: "askjhvdkahsvd", Expression: "expression", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.strictEqual(input2.inputName, "Expression");
        assert.strictEqual(input2.inputType, "object");
        assert.strictEqual(input2.allowNull, true);

        assert.strictEqual(input3.inputName, "MemberName");
        assert.strictEqual(input3.inputConstructor, String);
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
    assert.deepEqual(meta.MemberName, actual.MemberName);
});

test("_Compile test: exception", function () {
    var ex = new tUtil.Expect("test");

    var subject = {
        MemberName: "LJBHKLJBLKJB"
    };

    WebExpressions.MemberExpression.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MemberName);
            return false;
        }
    };

    assert.throws(function () {
        WebExpressions.MemberExpression.prototype._Compile.call(subject);
    });

    ex.VerifyOrderedExpectations();
});

// _Compile test
test("_Compile test: ok", function () {
    var ex = new tUtil.Expect("test", "innerCompile", "innerCall", "innerCall");
    debugger;
    var memberName = "LKJHLKJH";

    var context = {};

    var expected = {};

    var subject = {
        MemberName: memberName,
        Expression: {
            Compile: function () {
                ex.At("innerCompile");
                return function (ctxt) {
                    ex.At("innerCall");
                    assert.strictEqual(ctxt, context);

                    var output = {};
                    output[memberName] = expected;
                    return output;
                };
            }
        }
    };

    WebExpressions.MemberExpression.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MemberName);
            return true;
        }
    };

    var compiled = WebExpressions.MemberExpression.prototype._Compile.call(subject);
    assert.strictEqual(expected, compiled(context));
    assert.strictEqual(expected, compiled(context));

    ex.VerifyOrderedExpectations();
});