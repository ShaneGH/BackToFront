
/// <reference path="../Sanitizer.ts" />
/// <reference path="../../../../WebExpressions/Scripts/ref/Exports.ts" />

module BackToFront {

    export interface IValidate {
        RequiredForValidation: String[];
        ValidationSubjects: String[];

        Validate(entity: any, breakOnFirstError: boolean): Meta.IViolation[];
    }

    export module Validation {
        export class Validator {

            Rules: IValidate[];

            constructor(rules: Meta.RuleMeta[], public Entity: string) {

                this.Rules = linq(rules || []).Select(Validator.CreateRule).Result;
            }

            static CreateRule(rule: Meta.RuleMeta): IValidate {
                var r = ex.createExpression(rule.Expression).Compile();
                return {
                    RequiredForValidation: linq(rule.RequiredForValidation).Select(a => Validator.MemberChainItemString(a, true)).Result,
                    ValidationSubjects: linq(rule.ValidationSubjects).Select(a => Validator.MemberChainItemString(a, true)).Result,
                    Validate: (entity: any, breakOnFirstError: boolean = false) => {

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
                        return <any>context[rule.ContextParameter].Violations;
                    }
                };
            }

            static MemberChainItemString(memberChainItem, skipFirst): string {
                if (skipFirst) { memberChainItem = memberChainItem.NextItem; }
                var output = [];
                while (memberChainItem) {
                    output.push(memberChainItem.MemberName);
                    memberChainItem = memberChainItem.NextItem;
                }

                return output.join(".");
            }

            Validate(propertyName: string, breakOnFirstError: boolean = false): Meta.IViolation[] {

                var entity = this.GetEntity();

                return linq(this.Rules)
                    // find all relevent rules
                    .Where((rule: IValidate) => rule.ValidationSubjects.indexOf(propertyName) !== -1)
                    // validate all
                    .Select((rule: IValidate) => rule.Validate(entity, breakOnFirstError))
                    .Aggregate()
                    // filter violations which do not apply to this property.
                    .Where((violation: Meta.IViolation) => Validator.FilterViolation(violation, propertyName))
                    .Result;
            }

                // abstract
            GetEntity(): any {
                throw "Invalid operation, this method is abstract";
            }

            static FilterViolation(violation: Meta.IViolation, propertyName: string): boolean {
                return linq(violation.Violated).Any((member: Meta.MemberChainItem) => Validator.MemberChainItemString(member, true) === propertyName);
            }
        }
    }
}