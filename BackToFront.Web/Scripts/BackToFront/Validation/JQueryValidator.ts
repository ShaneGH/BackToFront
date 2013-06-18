
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

                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, function (aaaa, bbbb) {
                    if (this.__BTFContext && this.__BTFContext.Errors && this.__BTFContext.Errors.length) {
                        return linq(this.__BTFContext.Errors).Select(a => a.UserMessage).Result.join("\n");
                        //TODO: this
                        return jQuery.validator.format("These have been injected: {0}, {1}", "\"me\"", "\"and me\"");
                    } else {
                        return undefined;
                    }
                });

                if (jQuery.validator.unobtrusive && jQuery.validator.unobtrusive.adapters) {
                    jQuery.validator.unobtrusive.adapters.add("backtofront", [], function (options) {
                        options.rules["backtofront"] = options.params;
                    });
                }
            }

            //TODO: unit test
            static Validate(value: any, element: any, ...params: any[]) {

                var results = linq(JQueryValidator.Registered).Select((a: JQueryValidator) => a.Validate($(element).attr("name"), false))
                    .Aggregate();

                this.__BTFContext = new JqueryBTFContext(results.Result);
                return results.Result.length === 0
            }
        }
    }
}