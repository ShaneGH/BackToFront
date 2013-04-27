/// <reference path="jquery.d.ts" />
/// <reference path="jquery.validation.d.ts" />

if (window["__BTF"]) throw "BackToFront it defined already";

module __BTF {
    export var Initialize = function (data) { };

    export class TestClass {
        Test() {
            return true;
        }
    }
}

module __BTF {

    export module Enum {
        export enum ExpressionWrapperType {
            Binary = 1,
            Constant = 2,
            DependencyInjection = 3,
            //ElementAt,
            //Invocation,
            Member = 4,
            MethodCall = 5,
            Parameter = 6,
            Unary = 7
        }
    }
}