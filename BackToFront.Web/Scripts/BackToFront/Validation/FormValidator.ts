
/// <reference path="Validator.ts" />
/// <reference path="../Sanitizer.ts" />
/// <reference path="../Expressions/Expression.ts" />
/// <reference path="../../ref/jquery.d.ts" />

module __BTF {

    export module Validation {
        //TODO: rename to JQueryValidator
        export class FormValidator extends __BTF.Validation.Validator {
            constructor(rules: Meta.RuleMeta[], entity: string, public Context?: HTMLElement) {
                super(rules, entity);

                if (!jQuery) {
                    throw "This item requires jQuery";
                }
            };

            GetEntity(): any {
                var entity = {};

                var allNames = linq(this.Rules).Select(r => linq(r.RequiredForValidationNames || [])
                        .Union(r.ValidationSubjectNames || []).Result).Aggregate().Result;

                for (var j = 0, jj = allNames.length; j < jj; j++) {

                    var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);

                    //TODO: radio buttons
                    //TODO: other boolean
                    if (item.attr("type") === "checkbox") {
                        entity[allNames[j]] = item.is(":checked");
                    } else {
                        entity[allNames[j]] = item.val();

                        if (item.attr("data-val-number")) {
                            entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ?
                                 parseFloat(entity[allNames[j]]) :
                                 parseInt(entity[allNames[j]]);
                        }
                    }
                }

                return entity;
            };

            //TODO: nuit test
            static RegisterRule(rule: any) {
                FormValidator.Registered.push(new FormValidator(rule.Rules, rule.Entity));
            }

            static Registered: FormValidator[] = [];
            private static _Setup = false;
            //TODO: nuit test
            static Setup() {
                if (!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }

                if (FormValidator._Setup)
                    return;
                else
                    FormValidator._Setup = true;

                jQuery.validator.addMethod("backtofront", function (value, element, parmas) {                    
                    var results = linq(FormValidator.Registered).Select((a: FormValidator) => a.Validate($(element).attr("name"), false)).Aggregate();
                    return results.Result.length === 0
                }, "XXX");
            }
        }
    }
}