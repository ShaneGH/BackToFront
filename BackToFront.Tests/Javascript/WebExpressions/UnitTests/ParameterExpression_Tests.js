
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var property = WebExpressions.Utils.CustomClassHandler.PropertyRegex;
var createExpression = WebExpressions.Expression.CreateExpression;
var require = WebExpressions.Sanitizer.Require;

module("WebExpressions.ParameterExpression", {
    setup: function () {
        WebExpressions.Utils.CustomClassHandler.PropertyRegex = property;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    },
    teardown: function () {
        WebExpressions.Utils.CustomClassHandler.PropertyRegex = property;
        WebExpressions.Expression.CreateExpression = createExpression;
        WebExpressions.Sanitizer.Require = require;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require");

    // arrange
    var meta = { Name: "name", NodeType: WebExpressions.Meta.ExpressionType.Add };

    // first Sanitizer is in parent class
    var skip = true;
    WebExpressions.Sanitizer.Require = function (input1, input2) {
        if (skip) {
            skip = false;
            return;
        }

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, meta);

        assert.deepEqual(input2.inputName, "Name");
        assert.deepEqual(input2.inputConstructor, String);
    };

    WebExpressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new WebExpressions.ParameterExpression(meta);

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
    var result = WebExpressions.ParameterExpression.prototype._Compile.call(_this);
    var actual = result(context);

    // assert
    assert.strictEqual(context.something, actual);
});

// ##########################################
// # Replace if EvalExpression is put back in
// ##########################################
//test("EvalExpression test", function () {

//    // arrange
//    var subject = {
//        Name: "LKJLKJHLKH"
//    };

//    // act
//    var result = WebExpressions.ParameterExpression.prototype.EvalExpression.call(subject);

//    // assert
//    assert.strictEqual(result.Expression, subject.Name);
//    assert.strictEqual(result.Constants.constructor, WebExpressions.Utils.Dictionary);
//    assert.strictEqual(0, result.Constants._InnerDictionary.length);
//});