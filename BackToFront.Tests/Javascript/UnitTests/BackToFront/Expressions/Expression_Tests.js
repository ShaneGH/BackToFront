
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />

module("__BTF.Expressions.Expression", {
    setup: function () {
    },
    teardown: function () {
    }
});

test("Constructor test OK", function () {
    // arrange
    var expected = { NodeType: 22, ExpressionType: 55 };

    // act
    var actual = new __BTF.Expressions.Expression(expected);

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
        var actual = new __BTF.Expressions.Expression(expected);
    });
});

test("Constructor test null expression type type", function () {
    // arrange
    var expected = { NodeType: 22 };

    // act
    // assert
    assert.throws(function () {
        var actual = new __BTF.Expressions.Expression(expected);
    });
});

test("Compile test and cache", function () {
    // arrange
    var context = {};
    var subject = new __BTF.Expressions.Expression({ NodeType: 22, ExpressionType: 55 });
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
    var subject = new __BTF.Expressions.Expression({ NodeType: 22, ExpressionType: 55 });

    // act
    // assert
    assert.deepEqual(subject.GetAffectedProperties(), []);
});

test("CreateExpression, valid", function () {
    // arrange
    // act
    var result = __BTF.Expressions.Expression.CreateExpression({ NodeType: 22, ExpressionType: __BTF.Meta.ExpressionWrapperType.Parameter, Name: "KJGKJ" });

    // assert
    assert.notEqual(result, null);
    assert.strictEqual(result.constructor, __BTF.Expressions.ParameterExpression);
});

test("CreateExpression, inalid", function () {
    // arrange
    // act
    // assert
    throws(function () {
        var result = __BTF.Expressions.Expression.CreateExpression({ NodeType: 22, ExpressionType: 9875, Name: "KJGKJ" });
    });
});

test("ExpressionConstructorDictionary", function () {
    // arrange
    // act
    // assert
    for (var i in __BTF.Meta.ExpressionWrapperType) {
        // typescript puts in some crap
        if (__BTF.Meta.ExpressionWrapperType[i].constructor !== Number)
            continue;

        i = __BTF.Meta.ExpressionWrapperType[i];
        assert.notEqual(__BTF.Expressions.Expression.ExpressionConstructorDictionary[i], null, i);
        assert.strictEqual(__BTF.Expressions.Expression.ExpressionConstructorDictionary[i].constructor, Function);
    }
});