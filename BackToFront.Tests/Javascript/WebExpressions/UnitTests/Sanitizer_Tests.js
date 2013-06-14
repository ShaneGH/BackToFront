
// Chutzpah
/// <reference path="../../../Scripts/build/BackToFront.debug.js" />

module("WebExpressions.Sanitizer", {
    setup: function () {
    },
    teardown: function () {
    }
});

test("Require Test, all ok", function () {

    // arrange
    // act
    // assert
    WebExpressions.Sanitizer.Require({ prop: 1 }, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: false });

    expect(0);
});

test("Require Test, all ok, allow null", function () {
    // arrange
    // act
    // assert
    WebExpressions.Sanitizer.Require({ prop: null }, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: true });

    expect(0);
});

test("Require Test, null input", function () {
    // arrange
    // act
    // assert
    throws(function () {
        WebExpressions.Sanitizer.Require(null, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: false });
    });
});

test("Require Test, null property", function () {
    // arrange
    // act
    // assert
    throws(function () {
        WebExpressions.Sanitizer.Require({ prop: null }, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: false });
    });
});

test("Require Test, invalid input type", function () {
    // arrange
    // act
    // assert
    throws(function () {
        WebExpressions.Sanitizer.Require({ prop: 1 }, { inputName: "prop", inputType: "string", inputConstructor: Number, allowNull: false });
    });
});

test("Require Test, invalid constructor", function () {
    // arrange
    // act
    // assert
    throws(function () {
        WebExpressions.Sanitizer.Require({ prop: 1 }, { inputName: "prop", inputType: "number", inputConstructor: String, allowNull: false });
    });
});