
/// <reference path="../Sanitizer.ts" />
/// <reference path="../Expressions/Expression.ts" />

module __BTF {

    export interface IValidate {
        RequiredForValidationNames: String[];
        ValidationSubjectNames: String[];

        Validate(entity: any, breakOnFirstError: bool): Meta.IViolation[];
    }

    export module Validation {
        export class Validator {

            Rules: IValidate[];

            constructor(rules: Meta.RuleMeta[], public Entity: string) {

                this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
            };

            static CreateRule(rule: Meta.RuleMeta): IValidate {
                var r = __BTF.Expressions.Expression.CreateExpression(rule.Expression).Compile();
                return {
                    RequiredForValidationNames: rule.RequiredForValidationNames,
                    ValidationSubjectNames: rule.ValidationSubjectNames,
                    Validate: (entity: any, breakOnFirstError: bool = false) => {

                        var context = {};
                        context[rule.EntityParameter] = entity;
                        context[rule.ContextParameter] = {
                            // TODO: hardcoded names (BreakOnFirstError, Violations, Mocks, Dependencies)
                            // TODO: Mocks and Dependencies
                            Violations: [],
                            BreakOnFirstError: breakOnFirstError,
                            Mocks: [],
                            Dependencies: {}
                        };

                        r(context);
                        return context[rule.ContextParameter].Violations;
                    }
                };
            };

            Validate(propertyName: string, breakOnFirstError: bool = false): Meta.IViolation[] {

                var entity = this.GetEntity();

                return linq(this.Rules)
                    .Where((rule: IValidate) => rule.ValidationSubjectNames.indexOf(propertyName) !== -1)
                    .Select((rule: IValidate) => rule.Validate(entity, breakOnFirstError))
                    .Aggregate().Result;
            };

                // abstract
            GetEntity(): any {
                throw "Invalid operation, this method is abstract";
            };
        }
    }
}