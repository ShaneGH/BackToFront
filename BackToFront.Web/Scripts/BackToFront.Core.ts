/// <reference path="linq.d.ts" />
/// <reference path="jquery.d.ts" />
/// <reference path="jquery.validation.d.ts" />
/// <reference path="MetaClasses.ts" />

if (window["__BTF"] != null) throw "BackToFront is defined already";

module __BTF {
    export var Initialize = function (data) { };

    export class TestClass {
        Test() {
            return true;
        }
    }
}