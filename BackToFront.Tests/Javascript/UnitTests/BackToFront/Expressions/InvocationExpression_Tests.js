
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var property = __BTF.Expressions.MemberExpression.PropertyRegex;
var createExpression = __BTF.Expressions.Expression.CreateExpression;
var require = __BTF.Sanitizer.Require;

module("__BTF.Expressions.InvocationExpression", {
    setup: function () {
        __BTF.Expressions.MemberExpression.PropertyRegex = property;
        __BTF.Expressions.Expression.CreateExpression = createExpression;
        __BTF.Sanitizer.Require = require;
    },
    teardown: function () {
        __BTF.Expressions.MemberExpression.PropertyRegex = property;
        __BTF.Expressions.Expression.CreateExpression = createExpression;
        __BTF.Sanitizer.Require = require;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "expr", "arg1", "arg2");

    // arrange
    var meta = { Expression: "expr", Arguments: ["arg1", "arg2"], NodeType: __BTF.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    __BTF.Sanitizer.Require = function (input1, input2, input3) {
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

    __BTF.Expressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new __BTF.Expressions.InvocationExpression(meta);

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
    var result = __BTF.Expressions.InvocationExpression.prototype._Compile.call(_this);
    var actual = result(context);

    // assert
    assert.strictEqual(expected, actual);
});