
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
        }
    }
}

BackToFront.Initialize.Bootstrap();