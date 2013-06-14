
/// <reference path="../../../../Scripts/build/BackToFront.debug.js" />

var dictionary = WebExpressions.Utils.Dictionary;

module("WebExpressions.Utils.Dictionary", {
    setup: function () {
    },
    teardown: function () {
    }
});

test("ContainsKey test", function () {

    // arrange
    var key = {};
    var d = new dictionary();
    d.Add({}, null);
    d.Add(key, null);

    // act
    // assert
    assert.ok(d.ContainsKey(key));
    assert.ok(!d.ContainsKey({}));
});

test("Add test, ok", function () {

    // arrange
    var key = {};
    var value = {};
    var d = new dictionary();

    // act
    d.Add(key, value);

    // assert
    assert.strictEqual(d._InnerDictionary.length, 1);
    assert.strictEqual(d._InnerDictionary[0].Key, key);
    assert.strictEqual(d._InnerDictionary[0].Value, value);
});

test("Add test, duplicate key", function () {

    // arrange
    var key = {};
    var d = new dictionary();
    d.Add(key, 1);

    // act
    // assert
    assert.throws(function () { d.Add(key, 2) });
});

test("AddOrReplace test, 2 keys", function () {

    // arrange
    var key1 = {};
    var value1 = {};
    var key2 = {};
    var value2 = {};
    var d = new dictionary();
    d.Add(key1, value1);

    // act
    d.AddOrReplace(key2, value2);

    // assert
    assert.strictEqual(d._InnerDictionary.length, 2);
    assert.strictEqual(d._InnerDictionary[0].Key, key1);
    assert.strictEqual(d._InnerDictionary[0].Value, value1);
    assert.strictEqual(d._InnerDictionary[1].Key, key2);
    assert.strictEqual(d._InnerDictionary[1].Value, value2);
});

test("Value test", function () {

    // arrange
    var key1 = {};
    var value1 = {};
    var key2 = {};
    var value2 = {};
    var d = new dictionary();
    d.Add(key1, value1);
    d.Add(key2, value2);

    // act
    var val1 = d.Value(key1);
    var val2 = d.Value(key2);

    // assert
    assert.strictEqual(val1, value1);
    assert.strictEqual(val2, value2);
});

test("Remove test", function () {

    // arrange
    var key1 = {};
    var value1 = {};
    var key2 = {};
    var value2 = {};
    var d = new dictionary();
    d.Add(key1, value1);
    d.Add(key2, value2);

    // act
    // assert
    assert.ok(!d.Remove({}));
    assert.ok(d.Remove(key1));
    assert.strictEqual(d._InnerDictionary.length, 1);
    assert.strictEqual(d._InnerDictionary[0].Key, key2);
    assert.strictEqual(d._InnerDictionary[0].Value, value2);
});

test("Merge test", function () {

    // arrange
    var d = new dictionary();
    var input = {
        _InnerDictionary: [
            { Key: 2, Value: 4 }
        ]
    };

    // act
    d.Merge(input);

    // assert
    assert.strictEqual(d._InnerDictionary.length, 1);
    assert.strictEqual(d._InnerDictionary[0].Key, input._InnerDictionary[0].Key);
    assert.strictEqual(d._InnerDictionary[0].Value, input._InnerDictionary[0].Value);
});