


/// <reference path="../../../WebExpressions/Scripts/ref/Exports.ts" />

module BackToFront {
	export module Meta {
		export interface RuleMeta {
			Expression: WebExpressions.Meta.ExpressionMeta;
			EntityParameter: string;
			ContextParameter: string;
			ValidationSubjects: BackToFront.Meta.MemberChainItem[];
			RequiredForValidation: BackToFront.Meta.MemberChainItem[];
		}

		export interface RuleCollectionMeta {
			Entity: string;
			Rules: RuleMeta[];
		}

		export interface MemberChainItem {
			NextItem: MemberChainItem;
			MemberName: string;
		}

		export interface IValidationContext {
			BreakOnFirstError: bool;
			Violations: IViolation;
			Mocks: any[];
			Dependencies: any;
		}

		export interface IViolation {
			UserMessage: string;
			ViolatedEntity: any;
			Violated: BackToFront.Meta.MemberChainItem[];
		}

	}
}