
/// <reference path="Validator.ts" />
/// <reference path="../Sanitizer.ts" />
/// <reference path="../Expressions/Expression.ts" />
/// <reference path="../../ref/jquery.d.ts" />

declare var $$: JQueryStatic;

module __BTF {

    export module Validation {
        export class FormValidator extends __BTF.Validation.Validator{
            constructor(rules: Meta.RuleMeta[], entity: string, public Context: HTMLElement) {
                super(rules, entity);
            }

            /*
        RequiredForValidationNames: String[];
        ValidationSubjectNames: String[];*/

            GetEntity(): any {
                var complete = {};
                var entity = {};
                for (var i in this.Rules) {
                    var allNames = linq(this.Rules[i].RequiredForValidationNames)
                        .Union(this.Rules[i].ValidationSubjectNames).Result;
                    for (var j in allNames) {
                        if (!complete[allNames[j]]) {
                            complete[allNames[j]] = true;
                            entity[allNames[j]] = $$("[name=\"" + allNames[j] + "\"]").val();
                        }
                    }
                }

                return entity;
            }
        }
    }
}