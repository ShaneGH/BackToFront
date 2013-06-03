var __BTF;
(function (__BTF) {
    (function (Validation) {
        var Validator = (function () {
            function Validator(rules, Entity) {
                this.Entity = Entity;
                this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
            }
            Validator.CreateRule = function CreateRule(rule) {
                var r = __BTF.Expressions.Expression.CreateExpression(rule.Expression).Compile();
                return {
                    RequiredForValidationNames: rule.RequiredForValidationNames,
                    ValidationSubjectNames: rule.ValidationSubjectNames,
                    Validate: function (entity, breakOnFirstError) {
                        if (typeof breakOnFirstError === "undefined") { breakOnFirstError = false; }
                        var context = {
                        };
                        context[rule.EntityParameter] = entity;
                        context[rule.ContextParameter] = {
                            Violations: [],
                            BreakOnFirstError: breakOnFirstError,
                            Mocks: [],
                            Dependencies: {
                            }
                        };
                        r(context);
                        return context[rule.ContextParameter].Violations;
                    }
                };
            };
            Validator.prototype.Validate = function (propertyName, breakOnFirstError) {
                if (typeof breakOnFirstError === "undefined") { breakOnFirstError = false; }
                var entity = this.GetEntity();
                return linq(this.Rules).Where(function (rule) {
                    return rule.ValidationSubjectNames.indexOf(propertyName) !== -1;
                }).Select(function (rule) {
                    return rule.Validate(entity, breakOnFirstError);
                }).Aggregate().Result;
            };
            Validator.prototype.GetEntity = function () {
                throw "Invalid operation, this method is abstract";
            };
            return Validator;
        })();
        Validation.Validator = Validator;        
    })(__BTF.Validation || (__BTF.Validation = {}));
    var Validation = __BTF.Validation;
})(__BTF || (__BTF = {}));
