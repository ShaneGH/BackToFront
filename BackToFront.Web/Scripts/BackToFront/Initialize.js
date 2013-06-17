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
        };
        Initialize._InitializeMethods = function _InitializeMethods() {
        };
        return Initialize;
    })();
    BackToFront.Initialize = Initialize;    
})(BackToFront || (BackToFront = {}));
BackToFront.Initialize.Bootstrap();
