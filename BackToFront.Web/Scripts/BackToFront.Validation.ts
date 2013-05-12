/// <reference path="linq.d.ts" />
/// <reference path="jquery.d.ts" />
/// <reference path="jquery.validation.d.ts" />
/// <reference path="MetaClasses.ts" />

module __BTF {
    export module Validation {

        export interface ExpressionInvokerAction {
            (namedArguments: any, context: ValidationContext): any;
        }

        export class ExpressionInvoker {
            constructor(public Logic: ExpressionInvokerAction, public AffectedProperties: string[]) { }
        }

        export class ValidationContext {
            // TODO
            Break(): bool { return false; }
        }
    }
}