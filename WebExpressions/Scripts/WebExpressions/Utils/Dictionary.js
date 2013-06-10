var WebExpressions;
(function (WebExpressions) {
    (function (Utils) {
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
                if(this.ContainsKey(key)) {
                    throw "Dictionary alredy contains the key " + key;
                }
                this._InnerDictionary.push(new KeyValuePair(key, value));
            };
            Dictionary.prototype.AddOrReplace = function (key, value) {
                var existing = linq(this._InnerDictionary).First(function (a) {
                    return a.Key === key;
                });
                if(existing) {
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
                for(var i = 0, ii = this._InnerDictionary.length; i < ii; i++) {
                    if(this._InnerDictionary[i].Key === key) {
                        this._InnerDictionary.splice(i, 1);
                        return true;
                    }
                }
                return false;
            };
            Dictionary.prototype.Merge = function (dictionary) {
                if(dictionary) {
                    for(var i = 0, ii = dictionary._InnerDictionary.length; i < ii; i++) {
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
