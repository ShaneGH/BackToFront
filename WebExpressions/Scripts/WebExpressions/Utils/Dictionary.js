var WebExpressions;
(function (WebExpressions) {
    /// <reference path="../../ref/linq.d.ts" />
    (function (Utils) {
        //Temporary location, typescript build is being a bit problematic
        var CustomClassHandler = (function () {
            function CustomClassHandler() {
            }
            CustomClassHandler.GetClass = function (fullyQualifiedClassName) {
                var className = CustomClassHandler.SplitNamespace(fullyQualifiedClassName);

                var item = window;
                for (var i = 0, ii = className.length; i < ii; i++) {
                    item = item[className[i]];
                    if (item == undefined)
                        throw "Cannot evaluate member " + className;
                }

                return item;
            };

            CustomClassHandler.SplitNamespace = function (input) {
                var output = input.split(".");
                linq(output).Each(function (a) {
                    if (!CustomClassHandler.PropertyRegex.test(a))
                        throw "Invalid namespace part " + a;
                });

                return output;
            };

            CustomClassHandler.AddStaticItem = function (fullyQualifiedName, item) {
                var ns = CustomClassHandler.SplitNamespace(fullyQualifiedName);
                if (ns.length < 2)
                    throw "Invalid namespace";

                var item = window;
                var i = 0;
                for (var ii = ns.length - 2; i < ii; i++) {
                    if (!item[ns[i]])
                        item = (item[ns[i]] = {});
else
                        item = item[ns[i]];
                }

                if (!item[ns[i]])
                    item = (item[ns[i]] = function () {
                    });
            };
            CustomClassHandler.PropertyRegex = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");
            return CustomClassHandler;
        })();
        Utils.CustomClassHandler = CustomClassHandler;

        var KeyValuePair = (function () {
            function KeyValuePair(Key, Value) {
                this.Key = Key;
                this.Value = Value;
            }
            return KeyValuePair;
        })();

        var Dictionary = (function () {
            function Dictionary() {
                this._InnerDictionary = [];
            }
            Dictionary.prototype.ContainsKey = function (key) {
                return linq(this._InnerDictionary).Any(function (a) {
                    return a.Key === key;
                });
            };

            Dictionary.prototype.Add = function (key, value) {
                if (this.ContainsKey(key))
                    throw "Dictionary alredy contains the key " + key;

                this._InnerDictionary.push(new KeyValuePair(key, value));
            };

            Dictionary.prototype.AddOrReplace = function (key, value) {
                var existing = linq(this._InnerDictionary).First(function (a) {
                    return a.Key === key;
                });
                if (existing) {
                    existing.Value = value;
                } else {
                    this._InnerDictionary.push(new KeyValuePair(key, value));
                }
            };

            Dictionary.prototype.Value = function (key) {
                var existing = linq(this._InnerDictionary).First(function (a) {
                    return a.Key === key;
                });
                return existing ? existing.Value : null;
            };

            Dictionary.prototype.Remove = function (key) {
                for (var i = 0, ii = this._InnerDictionary.length; i < ii; i++) {
                    if (this._InnerDictionary[i].Key === key) {
                        this._InnerDictionary.splice(i, 1);
                        return true;
                    }
                }

                return false;
            };

            Dictionary.prototype.Merge = function (dictionary) {
                if (dictionary) {
                    for (var i = 0, ii = dictionary._InnerDictionary.length; i < ii; i++) {
                        var item = dictionary._InnerDictionary[i];
                        this.Add(item.Key, item.Value);
                    }
                }

                return this;
            };
            return Dictionary;
        })();
        Utils.Dictionary = Dictionary;
    })(WebExpressions.Utils || (WebExpressions.Utils = {}));
    var Utils = WebExpressions.Utils;
})(WebExpressions || (WebExpressions = {}));
