
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var property = WebExpressions.MemberExpressionBase.PropertyRegex;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.MethodCallExpressionBase", {
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
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "arg1", "arg2");

    // arrange
    var meta = { Arguments: ["arg1", "arg2"], MethodName: "mn", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.strictEqual(input2.inputName, "Arguments");
        assert.strictEqual(input2.inputConstructor, Array);

        assert.strictEqual(input3.inputName, "MethodName");
        assert.strictEqual(input3.inputConstructor, String);
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.MethodCallExpressionBase(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(2, actual.Arguments.length);
    assert.deepEqual("arg1XX", actual.Arguments[0]);
    assert.deepEqual("arg2XX", actual.Arguments[1]);
    assert.deepEqual("mn", actual.MethodName);
});

test("_Compile test: exception", function () {

    var ex = new tUtil.Expect("test");

    var subject = {
        MethodName: "LJBHKLJBLKJB"
    };

    WebExpressions.MemberExpressionBase.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MethodName);
            return false;
        }
    };

    assert.throws(function () {
        WebExpressions.MethodCallExpressionBase.prototype._Compile.call(subject);
    });

    ex.VerifyOrderedExpectations();
});

test("_Compile test", function () {
    var context = {};
    var methodName = "IGBGBBJKB";

    var arg1 = {};
    var arg2 = {};
    var expected = {};

    // arrange
    var _this = {
        MethodName: methodName,
        _CompileMethodCallContext: function () {
            return function (ctxt) {
                assert.strictEqual(ctxt, context);
                var output = {};
                output[methodName] = function (argument1, argument2) {
                    assert.strictEqual(argument1, arg1, "YYY");
                    assert.strictEqual(argument2, arg2, "ZZZ");

                    return expected;
                };

                return output;
            };
        },
        Arguments: [
        {
            Compile: function () {
                return function (ctxt) {
                    assert.strictEqual(ctxt, context);
                    return arg1;
                }
            }
        }, {
            Compile: function () {
                return function (ctxt) {
                    assert.strictEqual(ctxt, context);
                    return arg2;
                }
            }
        }]
    };

    // act
    var result = WebExpressions.MethodCallExpression.prototype._Compile.call(_this);
    var actual = result(context);

    // assert
    assert.strictEqual(expected, actual);
});