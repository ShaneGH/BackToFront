
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;
var operators = WebExpressions.BinaryExpression.OperatorDictionary;

module("WebExpressions.BinaryExpression", {
    setup: function () {
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
        WebExpressions.BinaryExpression.OperatorDictionary = operators;
    },
    teardown: function () {
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
        WebExpressions.BinaryExpression.OperatorDictionary = operators;
    }
});

test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "left", "right");

    // arrange
    var meta = { Left: {}, Right: {}, NodeType: WebExpressions.Meta.ExpressionType.Add };

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

    var time = 0;
    WebExpressions.Expression.CreateExpression = function (input) {
        assert.deepEqual(true, time < 2);
        time++;

        if (time === 1) {
            ex.ExpectationReached.push("left");
            strictEqual(input, meta.Left);
            return "left";
        }
        else {
            ex.ExpectationReached.push("right");
            strictEqual(input, meta.Right);
            return "right";
        }
    };

    // act
    var actual = new WebExpressions.BinaryExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("left", actual.Left);
    assert.deepEqual("right", actual.Right);
});

test("Constructor test Invalid node type", function () {
    // arrange
    var meta = { Left: {}, Right: {}, NodeType: -7654 };

    // act
    // assert
    throws(function () { new WebExpressions.BinaryExpression(meta); });
});

test("_Compile test", function () {
    var ex = new tUtil.Expect("left", "right", "exp");

    var context = {};

    // arrange
    var _this = {
        Left: {
            Compile: function () {
                return function (ctxt) {
                    ex.ExpectationReached.push("left");
                    assert.strictEqual(ctxt, context);
                    return "left";
                }
            }
        },
        Right: {
            Compile: function () {
                return function (ctxt) {
                    ex.ExpectationReached.push("right");
                    assert.strictEqual(ctxt, context);
                    return "right";
                }
            }
        },
        NodeType: 0
    };

    WebExpressions.BinaryExpression.OperatorDictionary = [function (arg1, arg2) {
        ex.ExpectationReached.push("exp");
        assert.strictEqual(arg1, "left");
        assert.strictEqual(arg2, "right");
        return "exp";
    }];

    // act
    var result = WebExpressions.BinaryExpression.prototype._Compile.call(_this);

    // assert
    assert.deepEqual(result(context), "exp");
    ex.VerifyOrderedExpectations();
});

test("OperatorDictionary", function () {
    // arrange
    var ignored = {};
    ignored.AddChecked = true;
    ignored.And = true;
    ignored.ArrayLength = true;
    ignored.ArrayIndex = true;
    ignored.Call = true;
    ignored.Coalesce = true;
    ignored.Conditional = true;
    ignored.Constant = true;
    ignored.Convert = true;
    ignored.ConvertChecked = true;
    ignored.Equal = true;
    ignored.ExclusiveOr = true;
    ignored.Invoke = true;
    ignored.Lambda = true;
    ignored.LeftShift = true;
    ignored.ListInit = true;
    ignored.MemberAccess = true;
    ignored.MemberInit = true;
    ignored.Modulo = true;
    ignored.MultiplyChecked = true;
    ignored.Negate = true;
    ignored.UnaryPlus = true;
    ignored.NegateChecked = true;
    ignored.New = true;
    ignored.NewArrayInit = true;
    ignored.NewArrayBounds = true;
    ignored.Not = true;
    ignored.NotEqual = true;
    ignored.Or = true;
    ignored.Parameter = true;
    ignored.Power = true;
    ignored.Quote = true;
    ignored.RightShift = true;
    ignored.SubtractChecked = true;
    ignored.TypeAs = true;
    ignored.TypeIs = true;
    ignored.Assign = true;
    ignored.Block = true;
    ignored.DebugInfo = true;
    ignored.Decrement = true;
    ignored.Dynamic = true;
    ignored.Default = true;
    ignored.Extension = true;
    ignored.Goto = true;
    ignored.Increment = true;
    ignored.Index = true;
    ignored.Label = true;
    ignored.RuntimeVariables = true;
    ignored.Loop = true;
    ignored.Switch = true;
    ignored.Throw = true;
    ignored.Try = true;
    ignored.Unbox = true;
    ignored.AddAssign = true;
    ignored.AndAssign = true;
    ignored.DivideAssign = true;
    ignored.ExclusiveOrAssign = true;
    ignored.LeftShiftAssign = true;
    ignored.ModuloAssign = true;
    ignored.MultiplyAssign = true;
    ignored.OrAssign = true;
    ignored.PowerAssign = true;
    ignored.RightShiftAssign = true;
    ignored.SubtractAssign = true;
    ignored.AddAssignChecked = true;
    ignored.MultiplyAssignChecked = true;
    ignored.SubtractAssignChecked = true;
    ignored.PreIncrementAssign = true;
    ignored.PreDecrementAssign = true;
    ignored.PostIncrementAssign = true;
    ignored.PostDecrementAssign = true;
    ignored.TypeEqual = true;
    ignored.OnesComplement = true;
    ignored.IsTrue = true;
    ignored.IsFalse = true;

    for (var i in WebExpressions.Meta.ExpressionType) {
        if (WebExpressions.Meta.ExpressionType[i].constructor === Number && !ignored[i]) {
            ok(WebExpressions.BinaryExpression.OperatorDictionary[WebExpressions.Meta.ExpressionType[i]]);
        }
    }
});