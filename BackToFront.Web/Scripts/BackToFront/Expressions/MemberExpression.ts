
/// <reference path="../Sanitizer.ts" />
/// <reference path="Expression.ts" />

module __BTF {
    import Validation = __BTF.Validation;
    import Meta = __BTF.Meta;

    export module Expressions {

        export interface MemberExpressionRegex {
            Property: RegExp;
            IndexedProperty: RegExp;
        }

        export class MemberExpression extends Expression {
            static RegexValues: MemberExpressionRegex = (() => {
                var index = "\\[[0-9]+\\]";
                var indexedProperty = "[_a-zA-Z][_a-zA-Z0-9]*(" + index + ")?";

                return {
                    IndexedProperty: new RegExp(index + "$"),
                    Property: new RegExp("^" + indexedProperty + "$")
                };
            })();

            Expression: Expression;
            MemberName: string;

            constructor(meta: Meta.MemberExpressionMeta) {
                super(meta);

                __BTF.Sanitizer.Require(meta, {
                    inputName: "Expression",
                    inputType: "object"
                }, {
                    inputName: "MemberName",
                    inputConstructor: String
                });

                this.Expression = Expression.CreateExpression(meta.Expression);
                this.MemberName = meta.MemberName;
            }

            // TODO: taking pessimistic view of properties with regards to arrays
            // TODO: not throwing null exceptions
            // TODO: not validating at compile time, but at run time
            _Compile(): Validation.ExpressionInvokerAction {
                var expression = this.Expression.Compile();

                return (namedArguments, context) => {
                    if (!MemberExpression.RegexValues.Property.test(this.MemberName)) {
                        throw "Invalid property name: " + this.MemberName;
                    }

                    var base = expression(namedArguments, context);
                    if (MemberExpression.RegexValues.IndexedProperty.test(this.MemberName)) {
                        var property = this.MemberName.substr(0, this.MemberName.indexOf("[") - 1);
                        var index: any = MemberExpression.RegexValues.Property.exec(this.MemberName)[0];
                        index = parseInt(index.substring(1, index.length - 1));

                        base = base[property];
                        if (base == null) return null;
                        return base[index];
                    }

                    return base[this.MemberName];
                };
            }
        }
    }
}