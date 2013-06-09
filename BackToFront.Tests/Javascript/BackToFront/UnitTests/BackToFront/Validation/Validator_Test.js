
// Chutzpah
/// <reference path="../../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../../Base/testUtils.js" />

var BackToFront = __BTF;

var exp = ex.createExpression;

module("BackToFront.Validation.Validator", {
    setup: function () {
    },
    teardown: function () {
        ex.createExpression = exp;
    }
});

test("Validate test", function () {

    // arrange
    var error = "LKJBKLJB";
    var propertyName = "JKBKJB";
    var entity = {};
    var subject = {
        GetEntity: function () { return entity; },
        Rules: [{
            ValidationSubjects: propertyName,
            Validate: function (ent, br) {
                assert.strictEqual(ent, entity);
                assert.ok(!br);
                return [error];
            }
        }]
    };

    // act
    var result = BackToFront.Validation.Validator.prototype.Validate.call(subject, propertyName);

    // assert
    assert.deepEqual(result, [error]);
});

test("Create rule test", function () {
    var expectations = new tUtil.Expect("exp", "compile");

    // arrange
    var rule = {
        RequiredForValidation: {},
        ValidationSubject: {},
        EntityParameter: "LKJBLKJB",
        ContextParameter: "P(HBOH"
    };
    var placeholder = {};
    var r = function (ctxt) { return placeholder.r(ctxt); }; // placeholder.r() is defined later
    ex.createExpression = function () { expectations.At("exp"); return { Compile: function () { expectations.At("compile"); return r; } } };

    debugger;
    // act
    var result = BackToFront.Validation.Validator.CreateRule(rule);

    // assert
    assert.strictEqual(result.RequiredForValidation, rule.RequiredForValidation);
    assert.strictEqual(result.ValidationSubjects, rule.ValidationSubjects);
    assert.strictEqual(result.Validate.constructor, Function);

    expectations.VerifyOrderedExpectations();


    // arrange
    expectations.Expect("run");
    var entity = {};
    var violations = {};
    placeholder.r = function (ctxt) {
        expectations.At("run");

        assert.strictEqual(ctxt[rule.EntityParameter], entity);
        assert.deepEqual(ctxt[rule.ContextParameter], {
            Violations: [],
            BreakOnFirstError: true,
            Mocks: [],
            Dependencies: {}
        });

        ctxt[rule.ContextParameter].Violations = violations;
    }

    // act
    var validated = result.Validate(entity, true);

    // assert
    assert.strictEqual(validated, violations);

    expectations.VerifyOrderedExpectations();
});