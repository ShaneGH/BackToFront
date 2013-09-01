
/// <reference path="Expression.ts" />

module WebExpressions {

    export class NewExpression extends Expression {
        Arguments: Expression[];
        Members: string[];
        Type: string;
        IsAnonymous: bool;

        constructor(meta: Meta.NewExpressionMeta) {
            super(meta);

            WebExpressions.Sanitizer.Require(meta, {
                inputName: "Arguments",
                inputConstructor: Array
            }, {
                inputName: "IsAnonymous",
                inputConstructor: Boolean
            }, {
                inputName: "Type",
                inputConstructor: String
            });

            if (meta.IsAnonymous) {
                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Members",
                    inputConstructor: Array
                });

                if (meta.Members.length !== meta.Arguments.length)
                    throw "If members are defined, each must have a corresponding argument";
            }

            this.Arguments = linq(meta.Arguments).Select(a => WebExpressions.Expression.CreateExpression(a)).Result;
            this.Members = meta.Members;
            this.Type = meta.Type;
            this.IsAnonymous = meta.IsAnonymous;
        }

        //EvalExpression(): CreateEvalExpression {
        //    var args = <CreateEvalExpression[]>linq(this.Arguments).Select(a => a.EvalExpression()).Result;
        //    var constants = new WebExpressions.Utils.Dictionary();
        //    linq(args).Each(a => constants.Merge(a.Constants));
        //    var argsString = linq(args).Select(a => a.Expression).Result.join(", ");

        //    if (this.IsAnonymous) {
        //        return {
        //            Constants: constants,
        //            Expression: "{ " + argsString + " }"
        //        };
        //    } else if (NewExpression.RegisteredTypes[this.Type]) {
        //        return {
        //            Constants: constants,
        //            Expression: "new ex.ns.NewExpression.RegisteredTypes[\"" + this.Type + "\"](" + argsString + ")"
        //        }
        //    } else {
        //        return {
        //            Constants: constants,
        //            Expression: "{}"
        //        };
        //    }
        //}

        // TODO: register types
        _Compile(): ExpressionInvokerAction {

            var args = linq(this.Arguments).Select(a => a.Compile()).Result;
            return (ambientContext) => {
                var params = linq(args).Select(a => a(ambientContext)).Result;
                if (this.IsAnonymous)
                    return this.ConstructAnonymous(params);
                else if (NewExpression.RegisteredTypes[this.Type])
                    return NewExpression.Construct(NewExpression.RegisteredTypes[this.Type], params);
                else
                    return {};
            };
        }

        ConstructAnonymous(params: any[]) {
            var obj = {};
            for (var i = 0, ii = this.Members.length; i < ii; i++)
                obj[this.Members[i]] = params[i];

            return obj;
        }

        static Construct(constr: Function, args: any[]) {
            var obj = Object.create(constr.prototype);
            var result = constr.apply(obj, args);

            //TODO: this
            return typeof result === 'object' ? result : obj;
        }

        static RegisteredTypes: Object = {};
    }
}