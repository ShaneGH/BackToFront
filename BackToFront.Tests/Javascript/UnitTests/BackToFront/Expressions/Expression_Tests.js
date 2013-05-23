
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />

//// My Module
//(function (moduleName) {
//    module(moduleName, {
//        setup: function () {
//        },
//        teardown: function () {
//        }
//    });

//    // My Test
//    (function (testName) {
//        test(testName, function () {
//            // arrange

//            // act

//            // assert
//        });
//    })("My Test");
//})("My Module");

test("Warmup", function () { expect(0); });

(function (moduleName) {
    module(moduleName, {
        setup: function () {
        },
        teardown: function () {
        }
    });

    // Constructor test OK
    (function (testName) {
        test(testName, function () {
            // arrange
            var expected = { NodeType: 22, ExpressionType: 55 };

            // act
            var actual = new __BTF.Expressions.Expression(expected);

            // assert
            assert.strictEqual(expected.NodeType, actual.NodeType);
            assert.strictEqual(expected.ExpressionType, actual.ExpressionType);
        });
    })("Constructor test OK");

    // Constructor test Bad node type
    (function (testName) {
        test(testName, function () {
            // arrange
            var expected = { NodeType: "22", ExpressionType: 55 };

            // act
            // assert
            assert.throws(function () {
                var actual = new __BTF.Expressions.Expression(expected);
            });
        });
    })("Constructor test Bad node type");

    // Constructor test Bad node type
    (function (testName) {
        test(testName, function () {
            // arrange
            var expected = { NodeType: 22 };

            // act
            // assert
            assert.throws(function () {
                var actual = new __BTF.Expressions.Expression(expected);
            });
        });
    })("Constructor test null expression type type");

    // Compile test and cache
    var compileTest = function (_break) {
        return function () {
            // arrange
            var item = {};
            var subject = new __BTF.Expressions.Expression({ NodeType: 22, ExpressionType: 55 });
            subject._Compile = function () {
                return function (i, c) {
                    assert.strictEqual(item, i);
                };
            };

            // act
            var actual1 = subject.Compile();
            var actual2 = subject.Compile();

            // assert
            assert.strictEqual(actual1, actual2);
            actual1(item, {
                Break: function () {
                    return _break;
                }
            });

            expect(1 + (_break ? 0 : 1));
        };
    };

    // Compile test and cache, Break == true
    (function (testName) {
        test(testName, compileTest(true));
    })("Compile test and cache, Break == true");

    // Compile test and cache, Break == false
    (function (testName) {
        test(testName, compileTest(true));
    })("Compile test and cache, Break == false");

    // GetAffectedProperties
    (function (testName) {
        test(testName, function () {
            // arrange
            var subject = new __BTF.Expressions.Expression({ NodeType: 22, ExpressionType: 55 });

            // act
            // assert
            assert.deepEqual(subject.GetAffectedProperties(), []);
        });
    })("GetAffectedProperties");

    // CreateExpression - Valid
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            var result = __BTF.Expressions.Expression.CreateExpression({ NodeType: 22, ExpressionType: __BTF.Meta.ExpressionWrapperType.Parameter, Name: "KJGKJ" });

            // assert
            assert.notEqual(result, null);
            assert.strictEqual(result.constructor, __BTF.Expressions.ParameterExpression);
        });
    })("CreateExpression, valid");

    // CreateExpression - Invalid
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            throws(function () {
                var result = __BTF.Expressions.Expression.CreateExpression({ NodeType: 22, ExpressionType: 9875, Name: "KJGKJ" });
            });
        });
    })("CreateExpression, inalid");

    // ExpressionConstructorDictionary test
    (function (testName) {
        test(testName, function () {
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
    })("ExpressionConstructorDictionary");
})("__BTF.Expressions.Expression");