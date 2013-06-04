var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var __BTF;
(function (__BTF) {
    (function (Validation) {
        var FormValidator = (function (_super) {
            __extends(FormValidator, _super);
            function FormValidator(rules, entity, Context) {
                        _super.call(this, rules, entity);
                this.Context = Context;
                if(!jQuery) {
                    throw "This item requires jQuery";
                }
            }
            FormValidator.prototype.GetEntity = function () {
                var entity = {
                };
                var allNames = linq(this.Rules).Select(function (r) {
                    return linq(r.RequiredForValidationNames || []).Union(r.ValidationSubjectNames || []).Result;
                }).Aggregate().Result;
                for(var j = 0, jj = allNames.length; j < jj; j++) {
                    var item = jQuery("[name=\"" + allNames[j] + "\"]", this.Context);
                    if(item.attr("type") === "checkbox") {
                        entity[allNames[j]] = item.is(":checked");
                    } else {
                        entity[allNames[j]] = item.val();
                        if(item.attr("data-val-number")) {
                            entity[allNames[j]] = entity[allNames[j]].indexOf(".") !== -1 ? parseFloat(entity[allNames[j]]) : parseInt(entity[allNames[j]]);
                        }
                    }
                }
                return entity;
            };
            FormValidator.RegisterRule = function RegisterRule(rule) {
                FormValidator.Registered.push(new FormValidator(rule.Rules, rule.Entity));
            };
            FormValidator.Registered = [];
            FormValidator._Setup = false;
            FormValidator.Setup = function Setup() {
                if(!jQuery || !jQuery.validator) {
                    throw "This item requires jQuery and jQuery validation";
                }
                if(FormValidator._Setup) {
                    return;
                } else {
                    FormValidator._Setup = true;
                }
                jQuery.validator.addMethod("backtofront", function (value, element, parmas) {
                    var results = linq(FormValidator.Registered).Select(function (a) {
                        return a.Validate($(element).attr("name"), false);
                    }).Aggregate();
                    return results.Result.length === 0;
                }, "XXX");
            };
            return FormValidator;
        })(__BTF.Validation.Validator);
        Validation.FormValidator = FormValidator;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));
