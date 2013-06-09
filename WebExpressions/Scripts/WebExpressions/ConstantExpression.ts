
/// <reference path="Expression.ts" />
/// <reference path="Utils/Dictionary.ts" />

module WebExpressions {

    export class ConstantExpression extends Expression {
        Value: any;

        constructor(meta: Meta.ConstantExpressionMeta) {
            super(meta);

            this.Value = meta.Value;
        }

        ToString(): string {
            return ConstantExpression.ToString(this.Value);
        }

        _Compile(): ExpressionInvokerAction {
            return (ambientContext) => this.Value;
        }

        static ToString(value) {
            if (value === null) {
                return "null";
            } else if (value === undefined) {
                return "undefined";
            } else if (ConstantExpression.RegisteredConverters.ContainsKey(this.Value.constructor)) {
                return ConstantExpression.RegisteredConverters.Value(value.constructor)(value);
            } else {
                return null;
            }
        }

        static RegisteredConverters: WebExpressions.Utils.Dictionary = (function () {
            var dic = new WebExpressions.Utils.Dictionary();

            //TODO: object, date time, regexp + others
            dic.Add(String, function (a) { return "\"" + a + "\"" });
            dic.Add(Number, function (a) { return a.toString(); });
            dic.Add(Boolean, function (a) { return a.toString(); });
            dic.Add(Array, function (a) { return "[" + linq(a).Select(a => ConstantExpression.ToString(a)).Result.join(", ") + "]" });

            return dic;
        })();
    }
}