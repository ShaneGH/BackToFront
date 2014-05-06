
/// <reference path="../../ref/linq.d.ts" />

module WebExpressions.Utils {

    //Temporary location, typescript build is being a bit problematic
    export class CustomClassHandler {

        static PropertyRegex: RegExp = new RegExp("^[_a-zA-Z][_a-zA-Z0-9]*$");

        static GetClass(fullyQualifiedClassName: string): Function {

            var className = CustomClassHandler.SplitNamespace(fullyQualifiedClassName);

            var item = <any>window;
            for (var i = 0, ii = className.length; i < ii; i++) {
                item = item[className[i]];
                if (item == undefined)
                    throw "Cannot evaluate member " + className;
            }

            return item;
        }

        static SplitNamespace(input: string): string[] {

            var output = input.split(".");
            linq(output).Each(a => {
                if (!CustomClassHandler.PropertyRegex.test(a))
                    throw "Invalid namespace part " + a;
            });

            return output;
        }

        static AddStaticItem(fullyQualifiedName: string, item: any) {
            var ns = CustomClassHandler.SplitNamespace(fullyQualifiedName);
            if (ns.length < 2)
                throw "Invalid namespace";

            var item = <any>window;
            var i = 0;
            for (var ii = ns.length - 2; i < ii; i++) {
                if (!item[ns[i]])
                    item = (item[ns[i]] = {});
                else
                    item = item[ns[i]];
            }

            if (!item[ns[i]])
                item = (item[ns[i]] = function () { });
        }
    }

    class KeyValuePair {
        constructor(public Key, public Value) { }
    }

    export class Dictionary {

        private _InnerDictionary: KeyValuePair[] = [];

        public ContainsKey(key): boolean {
            return linq(this._InnerDictionary).Any(a => a.Key === key);
        }

        public Add(key, value) {
            if (this.ContainsKey(key))
                throw "Dictionary alredy contains the key " + key;

            this._InnerDictionary.push(new KeyValuePair(key, value));
        }

        public AddOrReplace(key, value) {
            var existing = <KeyValuePair>linq(this._InnerDictionary).First(a => a.Key === key);
            if (existing) {
                existing.Value = value;
            } else {
                this._InnerDictionary.push(new KeyValuePair(key, value));
            }
        }

        public Value(key): any {
            var existing = <KeyValuePair>linq(this._InnerDictionary).First(a => a.Key === key);
            return existing ? existing.Value : null;
        }

        public Remove(key): any {
            for (var i = 0, ii = this._InnerDictionary.length; i < ii; i++) {
                if (this._InnerDictionary[i].Key === key) {
                    this._InnerDictionary.splice(i, 1);
                    return true;
                }
            }

            return false;
        }

        public Merge(dictionary: Dictionary): Dictionary {
            if (dictionary) {
                for (var i = 0, ii = dictionary._InnerDictionary.length; i < ii; i++) {
                    var item = dictionary._InnerDictionary[i];
                    this.Add(item.Key, item.Value);
                }
            }

            return this;
        }
    }
}