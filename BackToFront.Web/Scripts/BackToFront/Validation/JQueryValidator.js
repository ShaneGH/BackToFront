var __extends = this.__extends || function (d, b) {
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
                var entity = {
                };
                var allNames = linq(this.Rules).Select(function (r) {
                    return linq(r.RequiredForValidation || []).Union(r.ValidationSubjects || []).Result;
                }).Aggregate().Result;
                for(var j = 0, jj = allNames.length; j < jj; j++) {
                    var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);
                    if(item.attr("type") === "checkbox") {
                        entity[allNames[j]] = item.is(":checked");
                    } else {
                        entity[allNames[j]] = item.val();
                        if(item.attr("data-val-number")) {
                            if(!entity[allNames[j]]) {
                                entity[allNames[j]] = null;
                            } else {
                                entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ? parseFloat(entity[allNames[j]]) : parseInt(entity[allNames[j]]);
                            }
                        }
                    }
                }
                return entity;
            };
            JQueryValidator.Registered = [];
            JQueryValidator.ValidatorName = "backtofront";
            JQueryValidator.RegisterRule = function RegisterRule(rule) {
                BackToFront.Sanitizer.Require(rule, {
                    inputName: "Rules",
                    inputConstructor: Array
                }, {
                    inputName: "Entity",
                    inputConstructor: String
                });
                JQueryValidator.Registered.push(new JQueryValidator(rule.Rules, rule.Entity));
            };
            JQueryValidator.Setup = function Setup() {
                if(!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }
                if(jQuery.validator.methods[JQueryValidator.ValidatorName]) {
                    return;
                }
                jQuery.validator.addMethod(JQueryValidator.ValidatorName, JQueryValidator.Validate, function (aaaa, bbbb) {
                    if(this.__BTFContext && this.__BTFContext.Errors && this.__BTFContext.Errors.length) {
                        return linq(this.__BTFContext.Errors).Select(function (a) {
                            return a.UserMessage;
                        }).Result.join("\n");
                        return jQuery.validator.format("These have been injected: {0}, {1}", "\"me\"", "\"and me\"");
                    } else {
                        return undefined;
                    }
                });
                if(jQuery.validator.unobtrusive && jQuery.validator.unobtrusive.adapters) {
                    jQuery.validator.unobtrusive.adapters.add("backtofront", [], function (options) {
                        options.rules["backtofront"] = options.params;
                    });
                }
            };
            JQueryValidator.Validate = function Validate(value, element) {
                var params = [];
                for (var _i = 0; _i < (arguments.length - 2); _i++) {
                    params[_i] = arguments[_i + 2];
                }
                var results = linq(JQueryValidator.Registered).Select(function (a) {
                    return a.Validate($(element).attr("name"), false);
                }).Aggregate();
                this.__BTFContext = new JqueryBTFContext(results.Result);
                return results.Result.length === 0;
            };
            return JQueryValidator;
        })(BackToFront.Validation.Validator);
        Validation.JQueryValidator = JQueryValidator;        
    })(BackToFront.Validation || (BackToFront.Validation = {}));
    var Validation = BackToFront.Validation;
})(BackToFront || (BackToFront = {}));
