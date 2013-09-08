



module WebExpressions {
	export module Meta {
		export interface ExpressionMeta {
			NodeType: ExpressionType;
			ExpressionType: ExpressionWrapperType;
		}

		export interface BinaryExpressionMeta extends ExpressionMeta {
			Left: ExpressionMeta;
			Right: ExpressionMeta;
		}

		export interface BlockExpressionMeta extends ExpressionMeta {
			Expressions: ExpressionMeta[];
		}

		export interface ConditionalExpressionMeta extends ExpressionMeta {
			IfTrue: ExpressionMeta;
			IfFalse: ExpressionMeta;
			Test: ExpressionMeta;
		}

		export interface ConstantExpressionMeta extends ExpressionMeta {
			Value: any;
		}

		export interface DefaultExpressionMeta extends ExpressionMeta {
		}

		export interface InvocationExpressionMeta extends ExpressionMeta {
			Expression: ExpressionMeta;
			Arguments: ExpressionMeta[];
		}

		export interface MemberExpressionMetaBase extends ExpressionMeta {
			MemberName: string;
		}

		export interface MemberExpressionMeta extends MemberExpressionMetaBase {
			Expression: ExpressionMeta;
		}

		export interface StaticMemberExpressionMeta extends MemberExpressionMetaBase {
			Class: string;
		}

		export interface MethodCallExpressionMetaBase extends ExpressionMeta {
			Arguments: ExpressionMeta[];
			MethodName: string;
		}

		export interface MethodCallExpressionMeta extends MethodCallExpressionMetaBase {
			Object: ExpressionMeta;
		}

		export interface StaticMethodCallExpressionMeta extends MethodCallExpressionMetaBase {
			Class: string;
		}

		export interface NewExpressionMeta extends ExpressionMeta {
			Arguments: ExpressionMeta[];
			IsAnonymous: boolean;
			Members: string[];
			Type: string;
		}

		export interface ParameterExpressionMeta extends ExpressionMeta {
			Name: string;
		}

		export interface UnaryExpressionMeta extends ExpressionMeta {
			Operand: ExpressionMeta;
		}

		export enum ExpressionWrapperType {
			Binary = 1,
			Constant = 2,
			Member = 3,
			StaticMember = 4,
			MethodCall = 5,
			StaticMethodCall = 6,
			Parameter = 7,
			Unary = 8,
			Default = 9,
			Block = 10,
			Conditional = 11,
			Invocation = 12,
			New = 13,
		}

		export enum ExpressionType {
			Add = 0,
			AddChecked = 1,
			And = 2,
			AndAlso = 3,
			ArrayLength = 4,
			ArrayIndex = 5,
			Call = 6,
			Coalesce = 7,
			Conditional = 8,
			Constant = 9,
			Convert = 10,
			ConvertChecked = 11,
			Divide = 12,
			Equal = 13,
			ExclusiveOr = 14,
			GreaterThan = 15,
			GreaterThanOrEqual = 16,
			Invoke = 17,
			Lambda = 18,
			LeftShift = 19,
			LessThan = 20,
			LessThanOrEqual = 21,
			ListInit = 22,
			MemberAccess = 23,
			MemberInit = 24,
			Modulo = 25,
			Multiply = 26,
			MultiplyChecked = 27,
			Negate = 28,
			UnaryPlus = 29,
			NegateChecked = 30,
			New = 31,
			NewArrayInit = 32,
			NewArrayBounds = 33,
			Not = 34,
			NotEqual = 35,
			Or = 36,
			OrElse = 37,
			Parameter = 38,
			Power = 39,
			Quote = 40,
			RightShift = 41,
			Subtract = 42,
			SubtractChecked = 43,
			TypeAs = 44,
			TypeIs = 45,
			Assign = 46,
			Block = 47,
			DebugInfo = 48,
			Decrement = 49,
			Dynamic = 50,
			Default = 51,
			Extension = 52,
			Goto = 53,
			Increment = 54,
			Index = 55,
			Label = 56,
			RuntimeVariables = 57,
			Loop = 58,
			Switch = 59,
			Throw = 60,
			Try = 61,
			Unbox = 62,
			AddAssign = 63,
			AndAssign = 64,
			DivideAssign = 65,
			ExclusiveOrAssign = 66,
			LeftShiftAssign = 67,
			ModuloAssign = 68,
			MultiplyAssign = 69,
			OrAssign = 70,
			PowerAssign = 71,
			RightShiftAssign = 72,
			SubtractAssign = 73,
			AddAssignChecked = 74,
			MultiplyAssignChecked = 75,
			SubtractAssignChecked = 76,
			PreIncrementAssign = 77,
			PreDecrementAssign = 78,
			PostIncrementAssign = 79,
			PostDecrementAssign = 80,
			TypeEqual = 81,
			OnesComplement = 82,
			IsTrue = 83,
			IsFalse = 84,
		}
	}
}