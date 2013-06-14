
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.AssignmentExpression", {
    setup: function () {
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    }
});

test("Constructor test OK, parameter", function () {
    var ex = new tUtil.Expect("require", "right");

    // arrange
    var meta = { Left: { ExpressionType: WebExpressions.Meta.ExpressionWrapperType.Parameter, Name: "HVLKJHVKJHV" }, Right: "right", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Left");
        assert.deepEqual(input2.inputType, "object");

        assert.deepEqual(input3.inputName, "Right");
        assert.deepEqual(input3.inputType, "object");
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.At(input);
        return "XX" + input;
    };

    // act
    var actual = new WebExpressions.AssignmentExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.strictEqual(null, actual.Left);
    assert.strictEqual(meta.Left.Name, actual.LeftProperty);
    assert.strictEqual("XXright", actual.Right);
});

test("Constructor test OK, member", function () {
    var ex = new tUtil.Expect("require", "left", "right");

    // arrange
    var meta = { Left: { ExpressionType: WebExpressions.Meta.ExpressionWrapperType.Member, Expression: "left", MemberName: "HVLKJHVKJHV" }, Right: "right", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Left");
        assert.deepEqual(input2.inputType, "object");

        assert.deepEqual(input3.inputName, "Right");
        assert.deepEqual(input3.inputType, "object");
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.At(input);
        return "XX" + input;
    };

    // act
    var actual = new WebExpressions.AssignmentExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.strictEqual("XXleft", actual.Left);
    assert.strictEqual(meta.Left.MemberName, actual.LeftProperty);
    assert.strictEqual("XXright", actual.Right);
});

test("Constructor test invalid meta type", function () {
    var ex = new tUtil.Expect("require");

    // arrange
    var meta = { Left: { ExpressionType: 213123123 } };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Left");
        assert.deepEqual(input2.inputType, "object");

        assert.deepEqual(input3.inputName, "Right");
        assert.deepEqual(input3.inputType, "object");
    };

    // act
    // assert
    assert.throws(function () {
        var actual = new WebExpressions.AssignmentExpression(meta);
    });

    ex.VerifyOrderedExpectations();
});

test("_Compile test, has Left", function () {
    var ex = new tUtil.Expect("compileLeft", "compileRight", "calledLeft", "callRight");

    // arrange
    var context = {};
    var subject = {
        Left: {
            Compile: function () {
                ex.At("compileLeft");
                return function (ctxt) {
                    assert.strictEqual(context, ctxt);
                    ex.At("calledLeft");
                    return subject;
                }
            }
        },
        LeftProperty: ":KJLNB:LKNB:OIG",
        Right: {
            Compile: function () {
                ex.At("compileRight");
                return function (ctxt) {
                    assert.strictEqual(context, ctxt);
                    ex.At("callRight");
                    return subject.rightValue;
                }
            }
        },
        rightValue: {}
    };

    // act
    var compiled = WebExpressions.AssignmentExpression.prototype._Compile.call(subject);
    var actual = compiled(context);

    // assert
    ex.VerifyOrderedExpectations();
    assert.strictEqual(subject[subject.LeftProperty], subject.rightValue);
});

test("_Compile test, no Left", function () {
    var ex = new tUtil.Expect("compileRight", "callRight");

    // arrange
    var context = {};
    var subject = {
        LeftProperty: ":KJLNB:LKNB:OIG",
        Right: {
            Compile: function () {
                ex.At("compileRight");
                return function (ctxt) {
                    assert.strictEqual(context, ctxt);
                    ex.At("callRight");
                    return subject.rightValue;
                }
            }
        },
        rightValue: {}
    };

    // act
    var compiled = WebExpressions.AssignmentExpression.prototype._Compile.call(subject);
    var actual = compiled(context);

    // assert
    ex.VerifyOrderedExpectations();
    assert.strictEqual(context[subject.LeftProperty], subject.rightValue);
});