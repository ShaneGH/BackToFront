
// Chutzpah
/// <reference path="../Scripts/build/BackToFront.debug.js" />
/// <reference path="Base/testUtils.js" />
/// <reference path="Base/qunit.mock.js" />
/// <reference path="Base/jquery-2.0.2.js" />

var require = WebExpressions.Sanitizer.Require;
var createExpression = WebExpressions.Expression.CreateExpression;
var operators = WebExpressions.UnaryExpression.OperatorDictionary;
var operatorStrings = WebExpressions.UnaryExpression.OperatorStringDictionary;

// Constructor test OK
test("EvalExpression test", function () {
    var expect = new tUtil.Expect("callFromDictionary");

    // arrange
    var expression = {};
    var operand = { Constants: {}, Expression: {}};
    var subject = {
        NodeType: "nt",
        Operand: {
            EvalExpression: function () { return operand; }
        }
    };
    WebExpressions.UnaryExpression.OperatorStringDictionary = {
        nt: function (input) {
            expect.At("callFromDictionary");
            assert.strictEqual(input, operand.Expression);
            return expression;
        }
    };

    // act
    var result = WebExpressions.UnaryExpression.prototype.EvalExpression.call(subject);

    // assert
    expect.VerifyOrderedExpectations();
    assert.strictEqual(result.Expression, expression);
    assert.strictEqual(result.Constants, operand.Constants);
});