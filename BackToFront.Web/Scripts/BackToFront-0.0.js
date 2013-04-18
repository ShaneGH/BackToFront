var __BTF;
(function (__BTF) {
    __BTF.Initialize = function (data) {
    };
    var TestClass = (function () {
        function TestClass() { }
        TestClass.prototype.Test = function () {
            return true;
        };
        return TestClass;
    })();
    __BTF.TestClass = TestClass;    
})(__BTF || (__BTF = {}));
