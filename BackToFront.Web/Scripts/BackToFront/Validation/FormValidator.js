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
            }
            FormValidator.prototype.GetEntity = function () {
                var complete = {
                };
                var entity = {
                };
                for(var i in this.Rules) {
                    var allNames = linq(this.Rules[i].RequiredForValidationNames).Union(this.Rules[i].ValidationSubjectNames).Result;
                    for(var j in allNames) {
                        if(!complete[allNames[j]]) {
                            complete[allNames[j]] = true;
                            var item = $$("[name=\"" + allNames[j] + "\"]");
                            entity[allNames[j]] = $$("[name=\"" + allNames[j] + "\"]").val();
                            if(item.attr("data-val-number")) {
                                entity[allNames[j]] = parseInt(entity[allNames[j]]);
                            } else if(item.attr("type") === "checkbox") {
                                entity[allNames[j]] = entity[allNames[j]] && (entity[allNames[j]].toLower() === "true" || parseInt(entity[allNames[j]]) > 0);
                            }
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
