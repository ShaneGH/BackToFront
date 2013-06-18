
// Chutzpah
/// <reference path="../Scripts/build/BackToFront.debug.js" />
/// <reference path="Base/testUtils.js" />
/// <reference path="Base/qunit.mock.js" />
/// <reference path="Base/jquery-2.0.2.js" />

var BackToFront = __BTF;

var jq = jQuery;
module("BackToFront.Validation.JQueryValidator", {
    setup: function () {
    },
    teardown: function () {
        jQuery = jq;
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
    debugger;
    var entity = BackToFront.Validation.JQueryValidator.prototype.GetEntity.call(subject);

    // assert
    assert.strictEqual(entity[name1], val1);
    assert.strictEqual(entity[name2], val2);
});