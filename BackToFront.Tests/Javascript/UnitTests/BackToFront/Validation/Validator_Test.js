
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var exp = __BTF.Expressions.Expression;

module("__BTF.Validation.Validator", {
    setup: function () {
    },
    teardown: function () {
        __BTF.Expressions.Expression = exp;
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
            ValidationSubjectNames: propertyName,
            Validate: function (ent, br) {
                assert.strictEqual(ent, entity);
                assert.ok(!br);
                return [error];
            }
        }]
    };

    // act
    var result = __BTF.Validation.Validator.prototype.Validate.call(subject, propertyName);

    // assert
    assert.deepEqual(result, [error]);
});

test("Create rule test", function () {
    var ex = new tUtil.Expect("exp", "compile");

    // arrange
    var rule = {
        RequiredForValidationNames: {},
        ValidationSubjectNames: {},
        EntityParameter: "LKJBLKJB",
        ContextParameter: "P(HBOH"
    };
    var placeholder = {};
    var r = function (ctxt) { return placeholder.r(ctxt); };
    __BTF.Expressions.Expression.CreateExpression = function () { ex.At("exp"); return { Compile: function () { ex.At("compile"); return r; } } };

    // act
    var result = __BTF.Validation.Validator.CreateRule(rule);

    // assert
    assert.strictEqual(result.RequiredForValidationNames, rule.RequiredForValidationNames);
    assert.strictEqual(result.ValidationSubjectNames, rule.ValidationSubjectNames);
    assert.strictEqual(result.Validate.constructor, Function);

    ex.VerifyOrderedExpectations();


    // arrange
    ex.Expect("run");
    var entity = {};
    var violations = {};
    placeholder.r = function (ctxt) {
        ex.At("run");

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

    ex.VerifyOrderedExpectations();
});