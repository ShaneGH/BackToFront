
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
            };

            GetEntity(): any {
                var complete = {};
                var entity = {};
                for (var i in this.Rules) {
                    var allNames = linq(this.Rules[i].RequiredForValidationNames)
                        .Union(this.Rules[i].ValidationSubjectNames).Result;
                    for (var j in allNames) {
                        if (!complete[allNames[j]]) {
                            complete[allNames[j]] = true;
                            var item = $$("[name=\"" + allNames[j] + "\"]");
                            entity[allNames[j]] = $$("[name=\"" + allNames[j] + "\"]").val();
                            if (item.attr("data-val-number")) {
                                // TODO: parse int or float?
                                entity[allNames[j]] = parseInt(entity[allNames[j]]);
                            } else if (item.attr("type") === "checkbox") {
                                // TODO: other input type which contains bool
                                entity[allNames[j]] = entity[allNames[j]] &&
                                    (entity[allNames[j]].toLower() === "true" || parseInt(entity[allNames[j]]) > 0);
                            }
                        }
                    }
                }

                return entity;
            };
        }
    }
}