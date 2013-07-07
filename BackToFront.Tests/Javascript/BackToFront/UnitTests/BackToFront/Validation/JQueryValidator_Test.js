
// Chutzpah
/// <reference path="../../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../../Base/testUtils.js" />
/// <reference path="../../../../Base/jquery-2.0.2.js" />

var BackToFront = __BTF;

var jqv = jQuery.validator;
var reg = BackToFront.Validation.JQueryValidator.Registered;
var setup = BackToFront.Validation.JQueryValidator.Setup;
var join = BackToFront.Validation.JQueryValidator.JoinErrors;
var req = BackToFront.Sanitizer.Require;
var jq = jQuery;
module("BackToFront.Validation.JQueryValidator", {
    setup: function () {
        jQuery = jq;
        BackToFront.Sanitizer.Require = req;
        BackToFront.Validation.JQueryValidator.Registered.length = 0;
        BackToFront.Validation.JQueryValidator.Setup = setup;
        jQuery.validator = jqv;
        BackToFront.Validation.JQueryValidator.JoinErrors = join;
        BackToFront.Validation.JQueryValidator.Registered = reg;
    },
    teardown: function () {
        jQuery = jq;
        BackToFront.Sanitizer.Require = req;
        BackToFront.Validation.JQueryValidator.Registered.length = 0;
        BackToFront.Validation.JQueryValidator.Setup = setup;
        jQuery.validator = jqv;
        BackToFront.Validation.JQueryValidator.JoinErrors = join;
        BackToFront.Validation.JQueryValidator.Registered = reg;
    }
});

test("Constructor, no jQuery", function () {

    // arrange
    jQuery = null;

    // act
    // assert
    assert.throws(function () {
        new BackToFront.Validation.JQueryValidator();
    });
});

test("GetEntity, string", function () {

    // arrange
    var val1 = "JBKJBKJB";
    var name1 = "dsfohy98";
    var val2 = "saiouyg98yad";
    var name2 = "saohdisa0d9fu";

    var subject = {
        Rules: [{ RequiredForValidation: [name1], ValidationSubjects: [name2] }],
        Context: $(
"<div>" +
    "<input type='text' name='" + name1 + "' value='" + val1 + "'></input>" +
    "<input type='text' name='" + name2 + "' value='" + val2 + "'></input>" +
"</div>")[0]
    };

    // act
    var entity = BackToFront.Validation.JQueryValidator.prototype.GetEntity.call(subject);

    // assert
    assert.strictEqual(entity[name1], val1);
    assert.strictEqual(entity[name2], val2);
});

test("GetEntity, data-val-number", function () {

    // arrange
    var val1 = 223;
    var name1 = "dsfohy98";
    var val2 = 345.432;
    var name2 = "saohdisa0d9fu";
    var name3 = "dsfj09";
    var name4 = "asdkfg98";

    var subject = {
        Rules: [{ RequiredForValidation: [name1, name2], ValidationSubjects: [name3, name4] }],
        Context: $(
"<div>" +
    "<input type='text' data-val-number='true' name='" + name1 + "' value='" + val1 + "'></input>" +
    "<input type='text' data-val-number='true' name='" + name2 + "' value='" + val2 + "'></input>" +
    "<input type='text' data-val-number='true' name='" + name3 + "' value=''></input>" +
    "<input type='text' data-val-number='true' name='" + name4 + "'></input>" +
"</div>")[0]
    };

    // act
    var entity = BackToFront.Validation.JQueryValidator.prototype.GetEntity.call(subject);

    // assert
    assert.strictEqual(entity[name1], val1);
    assert.strictEqual(entity[name2], val2);
    assert.strictEqual(entity[name3], null);
    assert.strictEqual(entity[name4], null);
});

test("GetEntity, checkbox", function () {

    // arrange
    var val1 = true;
    var name1 = "dsfohy98";
    var val2 = false;
    var name2 = "saohdisa0d9fu";

    var checked = function (chk) { return chk ? "checked='checked'" : ""; };

    var subject = {
        Rules: [{ RequiredForValidation: [name1], ValidationSubjects: [name2] }],
        Context: $(
"<div>" +
    "<input type='checkbox' name='" + name1 + "' " + checked(val1) + "></input>" +
    "<input type='checkbox' name='" + name2 + "' " + checked(val2) + "></input>" +
"</div>")[0]
    };

    // act
    var entity = BackToFront.Validation.JQueryValidator.prototype.GetEntity.call(subject);

    // assert
    assert.strictEqual(entity[name1], val1);
    assert.strictEqual(entity[name2], val2);
});

test("RegisterRule test", function () {

    var ex = new tUtil.Expect("require", "setup");

    // arrange
    var input = { Rules: [], Entity: "asdasd" };

    BackToFront.Sanitizer.Require = function (input1, input2, input3) {

        ex.ExpectationReached.push("require");

        assert.strictEqual(input1, input);

        assert.deepEqual(input2.inputName, "Rules");
        assert.deepEqual(input2.inputConstructor, Array);

        assert.deepEqual(input3.inputName, "Entity");
        assert.deepEqual(input3.inputConstructor, String);
    };

    // stub out setup to avoiod exceptions
    BackToFront.Validation.JQueryValidator.Setup = function () { ex.At("setup"); }

    // act
    BackToFront.Validation.JQueryValidator.RegisterRule(input);

    // assert
    ex.VerifyOrderedExpectations();

    // can only verify rule was created :(
    assert.strictEqual(BackToFront.Validation.JQueryValidator.Registered.length, 1);
});

test("Setup, no jquery validation", function () {

    // arrange
    // act
    // assert
    assert.throws(function () { BackToFront.Validation.JQueryValidator.Setup(); });
});

test("Setup, already registered", function () {

    // arrange
    $.validator = {
        methods: {},
        addMethod: function () {
            // should not call this method
            assert.ok(false);
        }
    };
    $.validator.methods[BackToFront.Validation.JQueryValidator.ValidatorName] = {};

    // act
    // assert
    BackToFront.Validation.JQueryValidator.Setup();

    // method returned without doing anything (this is good)
    assert.ok(true);
});

test("Setup, do initial registration", function () {

    var ex = new tUtil.Expect("addMethod", "add");
    var v = BackToFront.Validation.JQueryValidator;

    // arrange
    $.validator = {
        methods: {},
        addMethod: function (name, validate, handleErrors) {
            ex.At("addMethod");

            assert.strictEqual(name, v.ValidatorName);
            assert.strictEqual(validate, v.Validate);
            assert.strictEqual(handleErrors, v.HandlerErrors);
        },
        unobtrusive: {
            adapters: {
                add: function (name, params, addOptions) {
                    ex.At("add");

                    assert.strictEqual(name, v.ValidatorName);
                    assert.deepEqual(params, []);
                    assert.strictEqual(addOptions, v.AddOptions);
                }
            }
        }
    };

    // act
    BackToFront.Validation.JQueryValidator.Setup();

    // assert
    ex.VerifyOrderedExpectations();
});

test("HandlerErrors has no errors", function () {

    // arrange
    var _this = {};

    // act
    var result = BackToFront.Validation.JQueryValidator.HandlerErrors.call(_this);

    // assert
    assert.strictEqual(result, undefined);
});

test("HandlerErrors has errors", function () {

    // arrange
    var expected = {};
    var message = "LKJBLKJBLKJB";
    var _this = {
        __BTFContext: {
            Errors: [
                { UserMessage: message }
            ]
        }
    };

    BackToFront.Validation.JQueryValidator.JoinErrors = function (input) {
        assert.strictEqual(1, input.length);
        assert.strictEqual(message, input[0]);

        return expected
    }

    // act
    var actual = BackToFront.Validation.JQueryValidator.HandlerErrors.call(_this);

    // assert
    assert.strictEqual(actual, expected);
});

test("AddOptions", function () {

    // arrange
    var options = {
        rules: {},
        params: {}
    };

    // act
    BackToFront.Validation.JQueryValidator.AddOptions(options);

    // assert
    assert.strictEqual(options.rules[BackToFront.Validation.JQueryValidator.ValidatorName], options.params);
});

var ValidateTest = function (areErrors) {
    // arrange
    var name = "LKJBLKJB";
    var expected = areErrors ? ["IJKJKBLKJHB"] : [];

    var _this = {};

    BackToFront.Validation.JQueryValidator.Registered = [{
        Validate: function (n, breakOnFirst) {
            assert.strictEqual(n, name);
            assert.ok(!breakOnFirst);

            return expected;
        }
    }];

    // act
    var actual = BackToFront.Validation.JQueryValidator.Validate.call(_this, null, "<input name=\"" + name + "\" />");

    // assert
    assert.strictEqual(actual, !areErrors);
    assert.ok(_this.__BTFContext);
    assert.deepEqual(_this.__BTFContext.Errors, expected);
};

test("Validate test, with violations", function () {
    ValidateTest(true);
});

test("Validate test, no violations", function () {
    ValidateTest(false);
});

test("JoinErrors test", function () {

    // arrange
    var errors = ["LJGBLKJG", "BLKJB:KB"];

    // act
    var actual = BackToFront.Validation.JQueryValidator.JoinErrors(errors);

    // assert
    assert.strictEqual(actual, errors[0] + "<br />" + errors[1]);
});