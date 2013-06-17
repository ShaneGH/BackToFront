
/// <reference path="../../../WebExpressions/Scripts/ref/Exports.ts" />

module BackToFront {
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
            
        }

        private static _InitializeMethods() {
            //TODO: this
            ex.registeredMethods["System.Collections.Generic.ICollection`1[[BackToFront.IViolation, BackToFront, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].Add"] =
                function (item) {
                    if (!this || !this.push || this.push.constructor !== Function)
                        throw "This method must be called on an array" + "##";

                    this.push(item);
                };
        }
    }
}

BackToFront.Initialize.Bootstrap();