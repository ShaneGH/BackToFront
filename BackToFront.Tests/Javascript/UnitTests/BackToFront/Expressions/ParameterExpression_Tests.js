
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
    var ex = new tUtil.Expect("require");

    // arrange
    var meta = { Name: "name", NodeType: __BTF.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    __BTF.Sanitizer.Require = function (input1, input2) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Name");
        assert.deepEqual(input2.inputConstructor, String);
    };

    __BTF.Expressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new __BTF.Expressions.ParameterExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual(meta.Name, actual.Name);
});

test("_Compile test", function () {
    var context = {
        something: "HBGLJHKGKJB"
    };

    // arrange
    var _this = {
        Name: "something"
    };

    // act
    var result = __BTF.Expressions.ParameterExpression.prototype._Compile.call(_this);
    var actual = result(context);

    // assert
    assert.strictEqual(context.something, actual);
});