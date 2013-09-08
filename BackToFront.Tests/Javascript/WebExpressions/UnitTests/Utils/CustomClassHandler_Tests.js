
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />


module("WebExpressions.Utils.CustomClassHandler", {
    setup: function () {
    },
    teardown: function () {
    }
});

// Constructor test OK
test("PropertyRegex tests", function () {

    // arrange
    var rx = WebExpressions.Utils.CustomClassHandler.PropertyRegex;

    // act
    // assert
    assert.ok(rx.test("LBLGPG"));
    assert.ok(rx.test("LBL_GPG"));
    assert.ok(rx.test("_LBLGPG"));
    assert.ok(rx.test("LB9LGPG"));
    assert.ok(rx.test("_0LBLGPG"));

    assert.ok(!rx.test("9LBLGPG"));
    assert.ok(!rx.test("LBLG(PG"));
    assert.ok(!rx.test("LBLG=PG"));
});

// Constructor test OK
test("GetClass, ok", function () {

    // arrange
    window.aa = {
        bb: {
            cc: {}
        }
    };
    var input = ["aa", "bb", "cc"];

    // act
    var actual = WebExpressions.Utils.CustomClassHandler.GetClass(input);

    // assert
    strictEqual(actual, window.aa.bb.cc);
});

// Constructor test OK
test("GetClass, invalid namespace part", function () {

    // arrange
    window.aa = undefined;
    var input = ["aa", "bb", "cc"];

    // act
    // assert
    throws(function () {
        WebExpressions.Utils.CustomClassHandler.GetClass(input);
    });
});

test("SplitNamespace test, ok", function () {

    // arrange
    // act
    var actual = WebExpressions.Utils.CustomClassHandler.SplitNamespace("aaa.bbb.ccc");

    // assert
    deepEqual(["aaa", "bbb", "ccc"], actual);
});