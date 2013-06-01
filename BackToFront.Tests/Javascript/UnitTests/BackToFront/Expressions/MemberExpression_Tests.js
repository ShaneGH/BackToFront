
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var property = __BTF.Expressions.MemberExpression.PropertyRegex;
var createExpression = __BTF.Expressions.Expression.CreateExpression;
var require = __BTF.Sanitizer.Require;

module("__BTF.Expressions.MemberExpression", {
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
    var ex = new tUtil.Expect("require", "expression");

    // arrange
    var meta = { MemberName: "askjhvdkahsvd", Expression: "expression", NodeType: __BTF.Meta.ExpressionType.Add };

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

        assert.deepEqual(input3.inputName, "MemberName");
        assert.deepEqual(input3.inputConstructor, String);
    };

    __BTF.Expressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new __BTF.Expressions.MemberExpression(meta);

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

    __BTF.Expressions.MemberExpression.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MemberName);
            return false;
        }
    };

    assert.throws(function () {
        __BTF.Expressions.MemberExpression.prototype._Compile.call(subject);
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

    __BTF.Expressions.MemberExpression.PropertyRegex = {
        test: function (member) {
            ex.At("test");
            assert.strictEqual(member, subject.MemberName);
            return true;
        }
    };

    var compiled = __BTF.Expressions.MemberExpression.prototype._Compile.call(subject);
    assert.strictEqual(expected, compiled(context));
    assert.strictEqual(expected, compiled(context));

    ex.VerifyOrderedExpectations();
});