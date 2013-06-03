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
            return FormValidator;
        })(__BTF.Validation.Validator);
        Validation.FormValidator = FormValidator;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));
