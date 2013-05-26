
// Chutzpah
/// <reference path="../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../Base/testUtils.js" />

test("Warmup", function () { expect(0); });

// __BTF.Sanitizer
(function (moduleName) {
    module(moduleName, {
        setup: function () {
        },
        teardown: function () {
        }
    });

    // Require Test, all ok
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            __BTF.Sanitizer.Require({ prop: 1 }, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: false });

            expect(0);
        });
    })("Require Test, all ok");

    // Require Test, all ok, with allow nulls
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            __BTF.Sanitizer.Require({ prop: null }, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: true });

            expect(0);
        });
    })("Require Test, all ok");

    // Require Test, null input
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            throws(function () {
                __BTF.Sanitizer.Require(null, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: false });
            });
        });
    })("Require Test, null property");

    // Require Test, null property
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            throws(function () {
                __BTF.Sanitizer.Require({ prop: null }, { inputName: "prop", inputType: "number", inputConstructor: Number, allowNull: false });
            });
        });
    })("Require Test, null property");

    // Require Test, invalid input type
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            throws(function () {
                __BTF.Sanitizer.Require({ prop: 1 }, { inputName: "prop", inputType: "string", inputConstructor: Number, allowNull: false });
            });
        });
    })("Require Test, invalid input type");

    // Require Test, invalid constructor
    (function (testName) {
        test(testName, function () {
            // arrange
            // act
            // assert
            throws(function () {
                __BTF.Sanitizer.Require({ prop: 1 }, { inputName: "prop", inputType: "number", inputConstructor: String, allowNull: false });
            });
        });
    })("Require Test, invalid constructor");
})("__BTF.Sanitizer");