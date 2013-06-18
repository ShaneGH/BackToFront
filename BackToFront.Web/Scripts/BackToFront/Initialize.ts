
/// <reference path="../../../WebExpressions/Scripts/ref/Exports.ts" />

module BackToFront {
    //TODO: test
    export class Initialize {
        private static _Initialized = false;

        static Bootstrap() {
            if (Initialize._Initialized)
                return;

            Initialize._Initialized = true;
            Initialize._InitializeConstructors();
            Initialize._InitializeMethods();
        }

        private static _InitializeConstructors() {            
            ex.registeredConstructors["BackToFront.Utilities.SimpleViolation"] = function (userMessage) {
                this.UserMessage = userMessage;
            };
        }

        private static _InitializeMethods() {
            //TODO: this
            ex.registeredMethods["System.Collections.Generic.ICollection`1[[BackToFront.IViolation, BackToFront, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].Add"] =
                function (item) {
                    if (!this || !this.push || this.push.constructor !== Function)
                        throw "This method must be called on an array" + "##";

                    this.push(item);
                };

            // TODO: move to WebExpressions library
            ex.registeredMethods["System.String.IsNullOrEmpty"] = function (input) { return !input; };
            ex.registeredMethods["System.String.IsNullOrWhiteSpace"] = function (input) { return !input || !input.trim(); };
        }
    }
}

BackToFront.Initialize.Bootstrap();