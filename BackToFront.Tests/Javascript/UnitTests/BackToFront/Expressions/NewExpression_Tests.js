
// Chutzpah
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />
/// <reference path="../../../Base/testUtils.js" />

var rt = __BTF.Expressions.NewExpression.RegisteredTypes;
var createExpression = __BTF.Expressions.NewExpression.CreateExpression;
var require = __BTF.Sanitizer.Require;

module("__BTF.Expressions.NewExpression", {
    setup: function () {
        __BTF.Expressions.Expression.CreateExpression = createExpression;
        __BTF.Sanitizer.Require = require;
        __BTF.Expressions.NewExpression.RegisteredTypes = rt;
    },
    teardown: function () {
        __BTF.Expressions.Expression.CreateExpression = createExpression;
        __BTF.Sanitizer.Require = require;
        __BTF.Expressions.NewExpression.RegisteredTypes = rt;
    }
});

// Constructor test OK
test("Constructor test OK", function () {
    var ex = new tUtil.Expect("require1", "require2", "arg1", "arg2");

    // arrange
    var meta = { Arguments: ["arg1", "arg2"], Members: ["mem1", "mem2"], Type: "type", IsAnonymous: "IAN" };

    // first Sanitizer is in parent class
    var skip = true;
    var number = 0;
    __BTF.Sanitizer.Require = function (input1, input2, input3, input4, input5) {
        if (skip) {
            skip = false;
            return;
        }

        if (number === 0) {
            ex.ExpectationReached.push("require1");

            assert.strictEqual(input1, meta);

            assert.deepEqual(input2.inputName, "Arguments");
            assert.deepEqual(input2.inputConstructor, Array);

            assert.deepEqual(input3.inputName, "IsAnonymous");
            assert.deepEqual(input3.inputConstructor, Boolean);

            assert.deepEqual(input4.inputName, "Type");
            assert.deepEqual(input4.inputConstructor, String);
        } else {
            ex.ExpectationReached.push("require2");

            assert.strictEqual(input1, meta);

            assert.deepEqual(input2.inputName, "Members");
            assert.deepEqual(input2.inputConstructor, Array);
        }

        number++;
    };

    __BTF.Expressions.Expression.CreateExpression = function (input) {
        ex.ExpectationReached.push(input);
        return input + "XX";
    };

    // act
    var actual = new __BTF.Expressions.NewExpression(meta);

    // assert
    ex.VerifyOrderedExpectations();
    assert.strictEqual(2, actual.Arguments.length);
    assert.strictEqual("arg1XX", actual.Arguments[0]);
    assert.strictEqual("arg2XX", actual.Arguments[1]);
    assert.strictEqual(meta.Members, actual.Members);
    assert.strictEqual(meta.Type, actual.Type);
    assert.strictEqual(meta.IsAnonymous, actual.IsAnonymous);
});

// Constructor test OK
test("Constructor test invalid member count", function () {
    var ex = new tUtil.Expect("require1", "require2", "arg1", "arg2");

    // arrange
    var meta = { Arguments: ["arg1"], Members: ["mem1", "mem2"], Type: "type", IsAnonymous: "IAN" };
    __BTF.Sanitizer.Require = function () { };

    // act
    // assert
    assert.throws(function () { new __BTF.Expressions.NewExpression(meta); });
});

// Constructor test OK
test("_Compile test, non anonymous, no constructor defined", function () {

    // arrange
    var subject = {
        Arguments: [],
        IsAnonymous: false,
        Type: "LKJBKJBLK"
    };

    __BTF.Expressions.NewExpression.RegisteredTypes = {};

    // act
    var result = __BTF.Expressions.NewExpression.prototype._Compile.call(subject)();

    // assert
    assert.strictEqual(result.constructor, Object);
});

// Constructor test OK
test("_Compile test, non anonymous, constructor defined", function () {

    // arrange
    var type = ":LKJHN)(YHO";
    var arg = {};
    var newObj = {};
    var ctxt = {};
    var subject = {
        Arguments: [{
            Compile: function () { return function (context) { assert.strictEqual(context, ctxt); return arg; } }
        }],
        IsAnonymous: false,
        Construct: function (constr, params) {
            assert.strictEqual(__BTF.Expressions.NewExpression.RegisteredTypes[type], constr);
            assert.strictEqual(1, params.length);
            assert.strictEqual(arg, params[0]);
            return newObj;
        },
        Type: type
    };

    __BTF.Expressions.NewExpression.RegisteredTypes = {};
    __BTF.Expressions.NewExpression.RegisteredTypes[type] = {};

    // act
    var result = __BTF.Expressions.NewExpression.prototype._Compile.call(subject)(ctxt);

    // assert
    assert.strictEqual(result, newObj);
});

// Constructor test OK
test("_Compile test, anonymous", function () {

    // arrange
    var type = ":LKJHN)(YHO";
    var arg = {};
    var newObj = {};
    var ctxt = {};
    var subject = {
        Arguments: [{
            Compile: function () { return function (context) { assert.strictEqual(context, ctxt); return arg; } }
        }],
        IsAnonymous: true,
        ConstructAnonymous: function (params) {
            assert.strictEqual(1, params.length);
            assert.strictEqual(arg, params[0]);
            return newObj;
        },
        Type: type
    };

    // act
    var result = __BTF.Expressions.NewExpression.prototype._Compile.call(subject)(ctxt);

    // assert
    assert.strictEqual(result, newObj);
});

// Constructor test OK
test("ConstructAnonymous test", function () {

    // arrange
    var arg = {};
    var argName = ":LKH:LKH";
    var subject = {
        Members: [argName]
    };

    // act
    var result = __BTF.Expressions.NewExpression.prototype.ConstructAnonymous.call(subject, [arg]);

    // assert
    assert.strictEqual(result.constructor, Object);
    assert.strictEqual(result[argName], arg);
});

// Constructor test OK
test("Construct test", function () {

    // arrange
    var arg = {};
    var c = function (arg1) { this.arg1 = arg1 };

    // act
    var result = __BTF.Expressions.NewExpression.prototype.Construct.call(null, c, [arg]);

    // assert
    assert.strictEqual(result.constructor, c);
    assert.strictEqual(result.arg1, arg);
});