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
                    RequiredForValidation: linq(rule.RequiredForValidation).Select(function (a) {
                        return Validator.MemberChainItemString(a, true);
                    }).Result,
                    ValidationSubjects: linq(rule.ValidationSubjects).Select(function (a) {
                        return Validator.MemberChainItemString(a, true);
                    }).Result,
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
            Validator.MemberChainItemString = function MemberChainItemString(memberChainItem, skipFirst) {
                if(skipFirst) {
                    memberChainItem = memberChainItem.NextItem;
                }
                var output = [];
                while(memberChainItem) {
                    output.push(memberChainItem.MemberName);
                    memberChainItem = memberChainItem.NextItem;
                }
                return output.join(".");
            };
            Validator.prototype.Validate = function (propertyName, breakOnFirstError) {
                if (typeof breakOnFirstError === "undefined") { breakOnFirstError = false; }
                var entity = this.GetEntity();
                return linq(this.Rules).Where(function (rule) {
                    return rule.ValidationSubjects.indexOf(propertyName) !== -1;
                }).Select(function (rule) {
                    return rule.Validate(entity, breakOnFirstError);
                }).Aggregate().Where(function (violation) {
                    return Validator.FilterViolation(violation, propertyName);
                }).Result;
            };
            Validator.prototype.GetEntity = function () {
                throw "Invalid operation, this method is abstract";
            };
            Validator.FilterViolation = function FilterViolation(violation, propertyName) {
                return linq(violation.Violated).Any(function (member) {
                    return Validator.MemberChainItemString(member, true) === propertyName;
                });
            };
            return Validator;
        })();
        Validation.Validator = Validator;        
    })(BackToFront.Validation || (BackToFront.Validation = {}));
    var Validation = BackToFront.Validation;
})(BackToFront || (BackToFront = {}));
