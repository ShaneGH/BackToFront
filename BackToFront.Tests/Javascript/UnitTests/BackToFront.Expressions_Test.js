
// Chutzpah
/// <reference path="../../Scripts/build/BackToFront.debug.js" />

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

// __BTF.Expressions.Expression
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

    // CreateExpression - TODO
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            expect(0);
        });
    })("CreateExpression");

})("__BTF.Expressions.Expression");