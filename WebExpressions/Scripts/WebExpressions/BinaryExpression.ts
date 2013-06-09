
/// <reference path="Expression.ts" />

module WebExpressions {
    import Meta = WebExpressions.Meta;
    
        export class BinaryExpression extends Expression {
            Left: Expression;
            Right: Expression;

            private static OperatorDictionary: { (left, right): any; }[] = (() => {
                var output: { (left, right): any; }[] = [];

                // TODO: more (all) operators
                output[WebExpressions.Meta.ExpressionType.Add] = (left, right) => left + right;
                output[WebExpressions.Meta.ExpressionType.AndAlso] = (left, right) => left && right;
                output[WebExpressions.Meta.ExpressionType.Divide] = (left, right) => left / right;
                //TODO: is this the right equals?
                output[WebExpressions.Meta.ExpressionType.Equal] = (left, right) => left === right;
                output[WebExpressions.Meta.ExpressionType.GreaterThan] = (left, right) => left > right;
                output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = (left, right) => left >= right;
                output[WebExpressions.Meta.ExpressionType.LessThan] = (left, right) => left < right;
                output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = (left, right) => left <= right;
                output[WebExpressions.Meta.ExpressionType.Multiply] = (left, right) => left * right;
                output[WebExpressions.Meta.ExpressionType.OrElse] = (left, right) => left || right;
                output[WebExpressions.Meta.ExpressionType.Subtract] = (left, right) => left - right;

                return output;
            })();

            private static OperatorStringDictionary: string[] = (() => {
                var output: string[] = [];

                // TODO: more (all) operators
                output[WebExpressions.Meta.ExpressionType.Add] = " + ";
                output[WebExpressions.Meta.ExpressionType.AndAlso] = " && ";
                output[WebExpressions.Meta.ExpressionType.Divide] = " / ";
                //TODO: is this the right equals?
                output[WebExpressions.Meta.ExpressionType.Equal] = " === ";
                output[WebExpressions.Meta.ExpressionType.GreaterThan] = " > ";
                output[WebExpressions.Meta.ExpressionType.GreaterThanOrEqual] = " >= ";
                output[WebExpressions.Meta.ExpressionType.LessThan] = " < ";
                output[WebExpressions.Meta.ExpressionType.LessThanOrEqual] = " left <= ";
                output[WebExpressions.Meta.ExpressionType.Multiply] = " * ";
                output[WebExpressions.Meta.ExpressionType.OrElse] = " || ";
                output[WebExpressions.Meta.ExpressionType.Subtract] = " - ";

                return output;
            })();

            constructor(meta: Meta.BinaryExpressionMeta) {
                super(meta);

                if (!BinaryExpression.OperatorDictionary[this.NodeType])
                    throw "##" + "Invalid Operator";

                WebExpressions.Sanitizer.Require(meta, {
                    inputName: "Left",
                    inputType: "object"
                }, {
                    inputName: "Right",
                    inputType: "object"
                });

                this.Left = Expression.CreateExpression(meta.Left);
                this.Right = Expression.CreateExpression(meta.Right);
            };

            public ToString(): string {
                return this.Left.ToString() + BinaryExpression.OperatorStringDictionary[this.NodeType] + this.Right.ToString();
            }
                 

            _Compile(): ExpressionInvokerAction {
                var left = this.Left.Compile();
                var right = this.Right.Compile();
                return (ambientContext) => BinaryExpression.OperatorDictionary[this.NodeType](left(ambientContext), right(ambientContext))
            };
        
    }
}