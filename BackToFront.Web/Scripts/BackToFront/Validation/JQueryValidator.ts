
/// <reference path="Validator.ts" />
/// <reference path="../Sanitizer.ts" />
/// <reference path="../../../../WebExpressions/Scripts/ref/Exports.ts" />
/// <reference path="../../ref/jquery.d.ts" />

module BackToFront {
    export module Validation {
        export class JQueryValidator extends BackToFront.Validation.Validator {

            constructor(rules: Meta.RuleMeta[], entity: string, public Context?: HTMLElement) {
                super(rules, entity);

                JQueryValidator.Setup();
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

            //#######################################################
            //###### Static
            //#######################################################

            static Registered: JQueryValidator[] = [];
            private static ValidatorName = "backtofront";

            //TODO: unit test
            static RegisterRule(rule: Meta.RuleCollectionMeta) {
                BackToFront.Sanitizer.Require(rule, {
                    inputName: "Rules",
                    inputConstructor: Array
                }, {
                    inputName: "Entity",
                    inputConstructor: String
                });

                JQueryValidator.Registered.push(new JQueryValidator(rule.Rules, rule.Entity));
            }

            //TODO: unit test
            static Setup() {
                if (!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }

                if (jQuery.validator.methods[JQueryValidator.ValidatorName])
                    return;

                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, "XXX");
            }

            //TODO: unit test
            static Validate(value: any, element: any, ...params: any[]) {
                debugger;
                var results = linq(JQueryValidator.Registered).Select((a: JQueryValidator) => a.Validate($(element).attr("name"), false)).Aggregate();
                return results.Result.length === 0
            }
        }
    }
}