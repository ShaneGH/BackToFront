
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../Base/testUtils.js" />

var assExp = WebExpressions.AssignmentExpression;

module("WebExpressions.Expression", {
    setup: function () {
        WebExpressions.AssignmentExpression = assExp;
    },
    teardown: function () {
        WebExpressions.AssignmentExpression = assExp;
    }
});

test("Constructor test OK", function () {
    // arrange
    var expected = { NodeType: 22, ExpressionType: 55 };

    // act
    var actual = new WebExpressions.Expression(expected);

    // assert
    assert.strictEqual(expected.NodeType, actual.NodeType);
    assert.strictEqual(expected.ExpressionType, actual.ExpressionType);
});

test("Constructor test Bad node type", function () {
    // arrange
    var expected = { NodeType: "22", ExpressionType: 55 };

    // act
    // assert
    assert.throws(function () {
        var actual = new WebExpressions.Expression(expected);
    });
});

test("Constructor test null expression type type", function () {
    // arrange
    var expected = { NodeType: 22 };

    // act
    // assert
    assert.throws(function () {
        var actual = new WebExpressions.Expression(expected);
    });
});

test("Compile test and cache", function () {
    // arrange
    var context = {};
    var subject = new WebExpressions.Expression({ NodeType: 22, ExpressionType: 55 });
    var returnVal = function (c) {
        assert.strictEqual(c, context);
    };

    subject._Compile = function () {
        return returnVal;
    };

    // act
    var actual1 = subject.Compile();
    var actual2 = subject.Compile();

    // assert
    assert.strictEqual(actual1, actual2);
    actual1(context);

    expect(2);
});

test("GetAffectedProperties", function () {
    // arrange
    var subject = new WebExpressions.Expression({ NodeType: 22, ExpressionType: 55 });

    // act
    // assert
    assert.deepEqual(subject.GetAffectedProperties(), []);
});

test("CreateExpression, assignment special case", function () {
    var ex = new tUtil.Expect("new");

    // arrange
    var meta = {
        NodeType: WebExpressions.Meta.ExpressionType.Assign,
        ExpressionType: WebExpressions.Meta.ExpressionWrapperType.Binary
    };

    WebExpressions.AssignmentExpression = function (input) {
        assert.strictEqual(input, meta);
        ex.At("new");
    }

    // act
    var result = WebExpressions.Expression.CreateExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.strictEqual(result.constructor, WebExpressions.AssignmentExpression);
});


test("CreateExpression, valid", function () {
    // arrange
    // act
    var result = WebExpressions.Expression.CreateExpression({ NodeType: 22, ExpressionType: WebExpressions.Meta.ExpressionWrapperType.Parameter, Name: "KJGKJ" });

    // assert
    assert.notEqual(result, null);
    assert.strictEqual(result.constructor, WebExpressions.ParameterExpression);
});

test("CreateExpression, inalid", function () {
    // arrange
    // act
    // assert
    throws(function () {
        var result = WebExpressions.Expression.CreateExpression({ NodeType: 22, ExpressionType: 9875, Name: "KJGKJ" });
    });
});

test("ExpressionConstructorDictionary", function () {
    // arrange
    // act
    // assert
    for (var i in WebExpressions.Meta.ExpressionWrapperType) {
        // typescript puts in some crap
        if (WebExpressions.Meta.ExpressionWrapperType[i].constructor !== Number)
            continue;

        i = WebExpressions.Meta.ExpressionWrapperType[i];
        assert.notEqual(WebExpressions.Expression.ExpressionConstructorDictionary[i], null, i);
        assert.strictEqual(WebExpressions.Expression.ExpressionConstructorDictionary[i].constructor, Function);
    }
});

test("EvalCompile test, no cache", function () {
    // arrange
    var val = { v: "LIJBLKJBLKJG*GBV" };
    var subject = {
        EvalExpression: function () {
            return {
                Expression: "return " + WebExpressions.ConstantExpression.ConstantParameter + ".v",
                Constants: val
            };
        }
    };

    // act
    var result = WebExpressions.Expression.prototype.EvalCompile.call(subject);
    var endValue = result();

    // assert
    assert.strictEqual(subject._EvalCompiled, result);
    assert.strictEqual(endValue, val.v);
});

test("EvalCompile test, with cache", function () {
    // arrange
    var ec = {};
    var subject = {
        _EvalCompiled: ec
    };

    // act
    var result = WebExpressions.Expression.prototype.EvalCompile.call(subject);

    // assert
    assert.strictEqual(result, subject._EvalCompiled);
    assert.strictEqual(ec, subject._EvalCompiled);
});