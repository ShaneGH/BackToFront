
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var WebExpressions = ex.ns;

var property = WebExpressions.MemberExpression.PropertyRegex;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.InvocationExpression", {
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
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "expr", "arg1", "arg2");

    // arrange
    var meta = { Expression: "expr", Arguments: ["arg1", "arg2"], NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Expression");
        assert.deepEqual(input2.inputType, "object");

        assert.deepEqual(input3.inputName, "Arguments");
        assert.deepEqual(input3.inputConstructor, Array);
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.InvocationExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("exprXX", actual.Expression);
    assert.deepEqual(2, actual.Arguments.length);
    assert.deepEqual("arg1XX", actual.Arguments[0]);
    assert.deepEqual("arg2XX", actual.Arguments[1]);
});

test("_Compile test", function () {
    var context = {};

    var arg1 = {};
    var arg2 = {};
    var expected = {};

    // arrange
    var _this = {
        Expression: {
            Compile: function () {
                return function (ctxt) {
                    assert.strictEqual(ctxt, context);
                    return function (argument1, argument2) {
                        assert.strictEqual(argument1, arg1, "YYY");
                        assert.strictEqual(argument2, arg2, "ZZZ");

                        return expected;
                    };
                };
            }
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
    var result = WebExpressions.InvocationExpression.prototype._Compile.call(_this);
    var actual = result(context);

    // assert
    assert.strictEqual(expected, actual);
});