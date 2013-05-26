
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var createExpression = __BTF.Expressions.Expression.CreateExpression;
var require = __BTF.Sanitizer.Require;
var operators = __BTF.Expressions.UnaryExpression.OperatorDictionary;

module("__BTF.Expressions.UnaryExpression", {
    setup: function () {
        __BTF.Expressions.Expression.CreateExpression = createExpression;
        __BTF.Sanitizer.Require = require;
        __BTF.Expressions.UnaryExpression.OperatorDictionary = operators;
    },
    teardown: function () {
        __BTF.Expressions.Expression.CreateExpression = createExpression;
        __BTF.Sanitizer.Require = require;
        __BTF.Expressions.UnaryExpression.OperatorDictionary = operators;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require", "operand");

    // arrange
    var meta = { Operand: {}, NodeType: __BTF.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    __BTF.Sanitizer.Require = function (input1, input2, input3) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Operand");
        assert.deepEqual(input2.inputType, "object");
    };

    __BTF.Expressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push("operand");
        strictEqual(input, meta.Operand);
        return "operand";
    };

    // act
    var actual = new __BTF.Expressions.UnaryExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.deepEqual("operand", actual.Operand);
});

// Constructor test Invalid node type
test("Constructor test Invalid node type", function () {
    // arrange
    var meta = { Operand: {}, NodeType: -7654 };

    // act
    // assert
    throws(function () { new __BTF.Expressions.UnaryExpression(meta); });
});

// _Compile test
test("_Compile test", function () {
    var ex = new tUtil.Expect("operand", "exp");

    var context = {};

    // arrange
    var _this = {
        Operand: {
            Compile: function () {
                return function (ctxt) {
                    ex.ExpectationReached.push("operand");
                    assert.strictEqual(ctxt, context);
                    return "operand";
                }
            }
        },
        NodeType: 0
    };

    __BTF.Expressions.UnaryExpression.OperatorDictionary = [function (arg1, arg2) {
        ex.ExpectationReached.push("exp");
        assert.strictEqual(arg1, "operand");
        return "exp";
    }];

    // act
    var result = __BTF.Expressions.UnaryExpression.prototype._Compile.call(_this);

    // assert
    assert.deepEqual(result(context), "exp");
    ex.VerifyOrderedExpectations();
});

// 
test("OperatorDictionary test", function () {
    // arrange
    var ignored = {};
    ignored.Add = true;
    ignored.AddChecked = true;
    ignored.And = true;
    ignored.AndAlso = true;
    ignored.ArrayLength = true;
    ignored.ArrayIndex = true;
    ignored.Call = true;
    ignored.Coalesce = true;
    ignored.Conditional = true;
    ignored.Constant = true;
    ignored.ConvertChecked = true;
    ignored.Divide = true;
    ignored.Equal = true;
    ignored.ExclusiveOr = true;
    ignored.GreaterThan = true;
    ignored.GreaterThanOrEqual = true;
    ignored.Invoke = true;
    ignored.Lambda = true;
    ignored.LeftShift = true;
    ignored.LessThan = true;
    ignored.LessThanOrEqual = true;
    ignored.ListInit = true;
    ignored.MemberAccess = true;
    ignored.MemberInit = true;
    ignored.Modulo = true;
    ignored.Multiply = true;
    ignored.MultiplyChecked = true;
    ignored.Negate = true;
    ignored.UnaryPlus = true;
    ignored.NegateChecked = true;
    ignored.New = true;
    ignored.NewArrayInit = true;
    ignored.NewArrayBounds = true;
    ignored.NotEqual = true;
    ignored.Or = true;
    ignored.OrElse = true;
    ignored.Parameter = true;
    ignored.Power = true;
    ignored.Quote = true;
    ignored.RightShift = true;
    ignored.Subtract = true;
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

    for (var i in __BTF.Meta.ExpressionType) {
        if (__BTF.Meta.ExpressionType[i].constructor === Number && !ignored[i]) {
            ok(__BTF.Expressions.UnaryExpression.OperatorDictionary[__BTF.Meta.ExpressionType[i]]);
        }
    }
});