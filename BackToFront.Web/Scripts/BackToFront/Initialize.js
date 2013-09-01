/// <reference path="../../../WebExpressions/Scripts/ref/Exports.ts" />
var BackToFront;
(function (BackToFront) {
    //TODO: test
    var Initialize = (function () {
        function Initialize() {
        }
        Initialize.Bootstrap = function () {
            if (Initialize._Initialized)
                return;

            Initialize._Initialized = true;
            Initialize._InitializeConstructors();
            Initialize._InitializeMethods();
        };

        Initialize._InitializeConstructors = function () {
            ex.registeredConstructors["BackToFront.Utilities.SimpleViolation"] = function (userMessage) {
                this.UserMessage = userMessage;
            };
        };

        Initialize._InitializeMethods = function () {
            //TODO: this
            ex.registeredMethods["System.Collections.Generic.ICollection`1[[BackToFront.IViolation, BackToFront, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].Add"] = function (item) {
                if (!this || !this.push || this.push.constructor !== Function)
                    throw "This method must be called on an array" + "##";

                this.push(item);
            };

            // TODO: move to WebExpressions library
            ex.registeredMethods["System.String.IsNullOrEmpty"] = function (input) {
                return !input;
            };
            ex.registeredMethods["System.String.IsNullOrWhiteSpace"] = function (input) {
                return !input || !input.trim();
            };
        };
        Initialize._Initialized = false;
        return Initialize;
    })();
    BackToFront.Initialize = Initialize;
})(BackToFront || (BackToFront = {}));

BackToFront.Initialize.Bootstrap();
