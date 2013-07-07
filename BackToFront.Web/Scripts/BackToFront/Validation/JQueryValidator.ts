
/// <reference path="Validator.ts" />
/// <reference path="../Sanitizer.ts" />
/// <reference path="../../../../WebExpressions/Scripts/ref/Exports.ts" />
/// <reference path="../../ref/jquery.d.ts" />

module BackToFront {
    export module Validation {

        export class JqueryBTFContext {
            constructor(public Errors: Meta.IViolation[] = []) { }
        }

        export class JQueryValidator extends BackToFront.Validation.Validator {

            constructor(rules: Meta.RuleMeta[], entity: string, public Context?: HTMLElement) {
                super(rules, entity);

                JQueryValidator.Setup();
            };

            GetEntity(): any {
                var entity = {};

                var allNames = linq(this.Rules).Select(r => linq(r.RequiredForValidation || [])
                        .Union(r.ValidationSubjects || []).Result).Aggregate().Result;

                for (var j = 0, jj = allNames.length; j < jj; j++) {

                    var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);

                    //TODO: radio buttons
                    //TODO: other boolean
                    if (item.attr("type") === "checkbox") {
                        entity[allNames[j]] = item.is(":checked");
                    } else {
                        entity[allNames[j]] = item.val();

                        if (item.attr("data-val-number")) {

                            if (!entity[allNames[j]]) {
                                entity[allNames[j]] = null;
                            } else {
                                entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ?
                                     parseFloat(entity[allNames[j]]) :
                                     parseInt(entity[allNames[j]]);
                            }
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

            static Setup() {
                if (!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }

                if (jQuery.validator.methods[JQueryValidator.ValidatorName])
                    return;

                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, JQueryValidator.HandlerErrors);

                // asp mvc unobtrusive validation
                if (jQuery.validator.unobtrusive && jQuery.validator.unobtrusive.adapters) {
                    jQuery.validator.unobtrusive.adapters.add(JQueryValidator.ValidatorName, [], JQueryValidator.AddOptions);
                }
            }

            // assume will be run onther the context of a jQuery validation
            static HandlerErrors() {
                if (this.__BTFContext && this.__BTFContext.Errors && this.__BTFContext.Errors.length) {
                    return JQueryValidator.JoinErrors(linq(this.__BTFContext.Errors).Select(a => a.UserMessage).Result);

                    // if string.format is needed for custom errors you can use this:
                    // jQuery.validator.format("These have been injected: {0}, {1}", "\"me\"", "\"and me\"");
                } else {
                    return undefined;
                }
            }

            static AddOptions(options) {
                options.rules[JQueryValidator.ValidatorName] = options.params;
            }

            static Validate(value: any, element: any, ...params: any[]) {

                var results = linq(JQueryValidator.Registered)
                    .Select((a: JQueryValidator) => a.Validate($(element).attr("name"), false))
                    .Aggregate();

                this.__BTFContext = new JqueryBTFContext(results.Result);
                return results.Result.length === 0
            }

            // default error message aggregation
            static JoinErrors(errors: string[]): string {
                return errors.join("<br />");
            }
        }
    }
}