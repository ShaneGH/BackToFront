
// Chutzpah
/// <reference path="../../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../../Base/testUtils.js" />

var BackToFront = __BTF;

var exp = ex.createExpression;
var mcis = BackToFront.Validation.Validator.MemberChainItemString;

module("BackToFront.Validation.Validator", {
    setup: function () {
    },
    teardown: function () {
        ex.createExpression = exp;
        BackToFront.Validation.Validator.MemberChainItemString = mcis;
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

var MemberChainItemStringTest = function (skipFirst) {

    // arrange
    var subject = {
        MemberName: "LKJHGBLKJHGB",
        NextItem: {
            MemberName: "IHUJBP(*Y(PGUBOH",
            NextItem: {
                MemberName: "IPUGP(*Y(*&GB"
            }
        }
    };

    // act
    var result = BackToFront.Validation.Validator.MemberChainItemString(subject, skipFirst);

    // assert
    if (skipFirst)
        assert.strictEqual(result, subject.NextItem.MemberName + "." + subject.NextItem.NextItem.MemberName)
    else
        assert.strictEqual(result, subject.MemberName + "." + subject.NextItem.MemberName + "." + subject.NextItem.NextItem.MemberName)
}

test("MemberChainItemString useFirst", function () {
    MemberChainItemStringTest(false);
});

test("MemberChainItemString skipFirst", function () {
    MemberChainItemStringTest(true);
});

test("Create rule test", function () {
    var expectations = new tUtil.Expect("exp", "compile", "RequiredForValidation", "ValidationSubject");

    // arrange
    var rule = {
        RequiredForValidation: [{}],
        ValidationSubjects: [{}],
        EntityParameter: "LKJBLKJB",
        ContextParameter: "P(HBOH"
    };
    var placeholder = {};
    var r = function (ctxt) { return placeholder.r(ctxt); }; // placeholder.r() is defined later
    ex.createExpression = function () { expectations.At("exp"); return { Compile: function () { expectations.At("compile"); return r; } } };
    BackToFront.Validation.Validator.MemberChainItemString = function (input1, input2) {
        if (input1 === rule.RequiredForValidation[0]) { expectations.At("RequiredForValidation"); }
        if (input1 === rule.ValidationSubjects[0]) { expectations.At("ValidationSubject"); }

        assert.ok(input2);

        return input1;
    };

    // act
    var result = BackToFront.Validation.Validator.CreateRule(rule);

    // assert
    assert.deepEqual(result.RequiredForValidation, rule.RequiredForValidation);
    assert.deepEqual(result.ValidationSubjects, rule.ValidationSubjects);
    assert.strictEqual(result.Validate.constructor, Function);
    debugger;
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