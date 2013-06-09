var BackToFront;
(function (BackToFront) {
    (function (Validation) {
        var Validator = (function () {
            function Validator(rules, Entity) {
                this.Entity = Entity;
                this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
            }
            Validator.CreateRule = function CreateRule(rule) {
                var r = ex.createExpression(rule.Expression).Compile();
                return {
                    RequiredForValidation: rule.RequiredForValidation,
                    ValidationSubjects: rule.ValidationSubjects,
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
                    return rule.ValidationSubjects.indexOf(propertyName) !== -1;
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
    })(BackToFront.Validation || (BackToFront.Validation = {}));
    var Validation = BackToFront.Validation;
})(BackToFront || (BackToFront = {}));
