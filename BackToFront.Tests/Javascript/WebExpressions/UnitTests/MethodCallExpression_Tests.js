
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var property = WebExpressions.MemberExpression.PropertyRegex;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.MethodCallExpression", {
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
    var ex = new tUtil.Expect("require", "obj", "arg1", "arg2");

    // arrange
    var meta = { Object: "obj", Arguments: ["arg1", "arg2"], MethodName: "mn", MethodFullName: "mfn", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3, input4, input5) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Object");
        assert.deepEqual(input2.inputType, "object");

        assert.deepEqual(input3.inputName, "Arguments");
        assert.deepEqual(input3.inputConstructor, Array);

        assert.deepEqual(input4.inputName, "MethodName");
        assert.deepEqual(input4.inputConstructor, String);

        assert.deepEqual(input5.inputName, "MethodFullName");
        assert.deepEqual(input5.inputConstructor, String);
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.MethodCallExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("objXX", actual.Object);
    assert.deepEqual(2, actual.Arguments.length);
    assert.deepEqual("arg1XX", actual.Arguments[0]);
    assert.deepEqual("arg2XX", actual.Arguments[1]);
    assert.deepEqual("mn", actual.MethodName);
    assert.deepEqual("mfn", actual.MethodFullName);
});

test("_Compile test: exception", function () {
    debugger;
    var ex = new tUtil.Expect("test");

    var subject = {
        MethodName: "LJBHKLJBLKJB"
    };

    WebExpressions.MemberExpression.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MethodName);
            return false;
        }
    };

    assert.throws(function () {
        WebExpressions.MethodCallExpression.prototype._Compile.call(subject);
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
        Object: {
            Compile: function () {
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
    var result = WebExpressions.MethodCallExpression.prototype._Compile.call(_this);
    var actual = result(context);

    // assert
    assert.strictEqual(expected, actual);
});