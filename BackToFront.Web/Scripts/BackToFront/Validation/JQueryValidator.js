/// <reference path="Validator.ts" />
/// <reference path="../Sanitizer.ts" />
/// <reference path="../../../../WebExpressions/Scripts/ref/Exports.ts" />
/// <reference path="../../ref/jquery.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var BackToFront;
(function (BackToFront) {
    (function (Validation) {
        var JqueryBTFContext = (function () {
            function JqueryBTFContext(Errors) {
                if (typeof Errors === "undefined") { Errors = []; }
                this.Errors = Errors;
            }
            return JqueryBTFContext;
        })();
        Validation.JqueryBTFContext = JqueryBTFContext;

        var JQueryValidator = (function (_super) {
            __extends(JQueryValidator, _super);
            function JQueryValidator(rules, entity, Context) {
                _super.call(this, rules, entity);
                this.Context = Context;

                JQueryValidator.Setup();
            }
            JQueryValidator.prototype.GetEntity = function () {
                var entity = {};

                var allNames = linq(this.Rules).Select(function (r) {
                    return linq(r.RequiredForValidation || []).Union(r.ValidationSubjects || []).Result;
                }).Aggregate().Result;

                for (var j = 0, jj = allNames.length; j < jj; j++) {
                    var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);

                    if (item.attr("type") === "checkbox") {
                        entity[allNames[j]] = item.is(":checked");
                    } else {
                        entity[allNames[j]] = item.val();

                        if (item.attr("data-val-number")) {
                            if (!entity[allNames[j]]) {
                                entity[allNames[j]] = null;
                            } else {
                                entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ? parseFloat(entity[allNames[j]]) : parseInt(entity[allNames[j]]);
                            }
                        }
                    }
                }

                return entity;
            };

            JQueryValidator.RegisterRule = function (rule) {
                BackToFront.Sanitizer.Require(rule, {
                    inputName: "Rules",
                    inputConstructor: Array
                }, {
                    inputName: "Entity",
                    inputConstructor: String
                });

                JQueryValidator.Registered.push(new JQueryValidator(rule.Rules, rule.Entity));
            };

            JQueryValidator.Setup = function () {
                if (!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }

                if (jQuery.validator.methods[JQueryValidator.ValidatorName])
                    return;

                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, JQueryValidator.HandlerErrors);

                if (jQuery.validator.unobtrusive && jQuery.validator.unobtrusive.adapters) {
                    jQuery.validator.unobtrusive.adapters.add(JQueryValidator.ValidatorName, [], JQueryValidator.AddOptions);
                }
            };

            JQueryValidator.HandlerErrors = // assume will be run onther the context of a jQuery validation
            function () {
                if (this["__BTFContext"] && this["__BTFContext"].Errors && this["__BTFContext"].Errors.length) {
                    return JQueryValidator.JoinErrors(linq(this["__BTFContext"].Errors).Select(function (a) {
                        return a.UserMessage;
                    }).Result);
                    // if string.format is needed for custom errors you can use this:
                    // jQuery.validator.format("These have been injected: {0}, {1}", "\"me\"", "\"and me\"");
                } else {
                    return undefined;
                }
            };

            JQueryValidator.AddOptions = function (options) {
                options.rules[JQueryValidator.ValidatorName] = options.params;
            };

            JQueryValidator.Validate = // assume will be run onther the context of a jQuery validation
            function (value, element) {
                var params = [];
                for (var _i = 0; _i < (arguments.length - 2); _i++) {
                    params[_i] = arguments[_i + 2];
                }
                var results = linq(JQueryValidator.Registered).Select(function (a) {
                    return a.Validate($(element).attr("name"), false);
                }).Aggregate();

                this["__BTFContext"] = new JqueryBTFContext(results.Result);
                return results.Result.length === 0;
            };

            JQueryValidator.JoinErrors = // default error message aggregation
            function (errors) {
                return errors.join("<br />");
            };
            JQueryValidator.Registered = [];
            JQueryValidator.ValidatorName = "backtofront";
            return JQueryValidator;
        })(BackToFront.Validation.Validator);
        Validation.JQueryValidator = JQueryValidator;
    })(BackToFront.Validation || (BackToFront.Validation = {}));
    var Validation = BackToFront.Validation;
})(BackToFront || (BackToFront = {}));
