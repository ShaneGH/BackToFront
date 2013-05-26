/// <reference path="../../ref/linq.d.ts" />
/// <reference path="../../ref/jquery.d.ts" />
/// <reference path="../../ref/jquery.validation.d.ts" />
/// <reference path="../MetaClasses.ts" />
/// <reference path="ValidationContext.ts" />

module __BTF {
    export module Validation {

        export interface ExpressionInvokerAction {
            (ambientContext): any;
        }

        export class ExpressionInvoker {
            constructor(public Logic: ExpressionInvokerAction, public AffectedProperties: string[]) { }
        }
    }
}