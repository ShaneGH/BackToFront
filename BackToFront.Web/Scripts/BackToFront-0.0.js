if(window["__BTF"]) {
    throw "sadsadsa";
}
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
var __BTF;
(function (__BTF) {
    (function (Enum) {
        (function (ExpressionWrapperType) {
            ExpressionWrapperType._map = [];
            ExpressionWrapperType.Binary = 1;
            ExpressionWrapperType.Constant = 2;
            ExpressionWrapperType.DependencyInjection = 3;
            ExpressionWrapperType.Member = 4;
            ExpressionWrapperType.MethodCall = 5;
            ExpressionWrapperType.Parameter = 6;
            ExpressionWrapperType.Unary = 7;
        })(Enum.ExpressionWrapperType || (Enum.ExpressionWrapperType = {}));
        var ExpressionWrapperType = Enum.ExpressionWrapperType;
    })(__BTF.Enum || (__BTF.Enum = {}));
    var Enum = __BTF.Enum;
})(__BTF || (__BTF = {}));
