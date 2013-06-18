var BackToFront;
(function (BackToFront) {
    var Initialize = (function () {
        function Initialize() { }
        Initialize._Initialized = false;
        Initialize.Bootstrap = function Bootstrap() {
            if(Initialize._Initialized) {
                return;
            }
            Initialize._Initialized = true;
            Initialize._InitializeConstructors();
            Initialize._InitializeMethods();
        };
        Initialize._InitializeConstructors = function _InitializeConstructors() {
            ex.registeredConstructors["BackToFront.Utilities.SimpleViolation"] = function (userMessage) {
                this.UserMessage = userMessage;
            };
        };
        Initialize._InitializeMethods = function _InitializeMethods() {
            ex.registeredMethods["System.Collections.Generic.ICollection`1[[BackToFront.IViolation, BackToFront, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].Add"] = function (item) {
                if(!this || !this.push || this.push.constructor !== Function) {
                    throw "This method must be called on an array" + "##";
                }
                this.push(item);
            };
        };
        return Initialize;
    })();
    BackToFront.Initialize = Initialize;    
})(BackToFront || (BackToFront = {}));
BackToFront.Initialize.Bootstrap();
