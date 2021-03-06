﻿var WebExpressions;
(function (WebExpressions) {
    (function (Meta) {
        (function (ExpressionWrapperType) {
            ExpressionWrapperType[ExpressionWrapperType["Binary"] = 1] = "Binary";
            ExpressionWrapperType[ExpressionWrapperType["Constant"] = 2] = "Constant";
            ExpressionWrapperType[ExpressionWrapperType["Member"] = 3] = "Member";
            ExpressionWrapperType[ExpressionWrapperType["StaticMember"] = 4] = "StaticMember";
            ExpressionWrapperType[ExpressionWrapperType["MethodCall"] = 5] = "MethodCall";
            ExpressionWrapperType[ExpressionWrapperType["StaticMethodCall"] = 6] = "StaticMethodCall";
            ExpressionWrapperType[ExpressionWrapperType["Parameter"] = 7] = "Parameter";
            ExpressionWrapperType[ExpressionWrapperType["Unary"] = 8] = "Unary";
            ExpressionWrapperType[ExpressionWrapperType["Default"] = 9] = "Default";
            ExpressionWrapperType[ExpressionWrapperType["Block"] = 10] = "Block";
            ExpressionWrapperType[ExpressionWrapperType["Conditional"] = 11] = "Conditional";
            ExpressionWrapperType[ExpressionWrapperType["Invocation"] = 12] = "Invocation";
            ExpressionWrapperType[ExpressionWrapperType["New"] = 13] = "New";
        })(Meta.ExpressionWrapperType || (Meta.ExpressionWrapperType = {}));
        var ExpressionWrapperType = Meta.ExpressionWrapperType;

        (function (ExpressionType) {
            ExpressionType[ExpressionType["Add"] = 0] = "Add";
            ExpressionType[ExpressionType["AddChecked"] = 1] = "AddChecked";
            ExpressionType[ExpressionType["And"] = 2] = "And";
            ExpressionType[ExpressionType["AndAlso"] = 3] = "AndAlso";
            ExpressionType[ExpressionType["ArrayLength"] = 4] = "ArrayLength";
            ExpressionType[ExpressionType["ArrayIndex"] = 5] = "ArrayIndex";
            ExpressionType[ExpressionType["Call"] = 6] = "Call";
            ExpressionType[ExpressionType["Coalesce"] = 7] = "Coalesce";
            ExpressionType[ExpressionType["Conditional"] = 8] = "Conditional";
            ExpressionType[ExpressionType["Constant"] = 9] = "Constant";
            ExpressionType[ExpressionType["Convert"] = 10] = "Convert";
            ExpressionType[ExpressionType["ConvertChecked"] = 11] = "ConvertChecked";
            ExpressionType[ExpressionType["Divide"] = 12] = "Divide";
            ExpressionType[ExpressionType["Equal"] = 13] = "Equal";
            ExpressionType[ExpressionType["ExclusiveOr"] = 14] = "ExclusiveOr";
            ExpressionType[ExpressionType["GreaterThan"] = 15] = "GreaterThan";
            ExpressionType[ExpressionType["GreaterThanOrEqual"] = 16] = "GreaterThanOrEqual";
            ExpressionType[ExpressionType["Invoke"] = 17] = "Invoke";
            ExpressionType[ExpressionType["Lambda"] = 18] = "Lambda";
            ExpressionType[ExpressionType["LeftShift"] = 19] = "LeftShift";
            ExpressionType[ExpressionType["LessThan"] = 20] = "LessThan";
            ExpressionType[ExpressionType["LessThanOrEqual"] = 21] = "LessThanOrEqual";
            ExpressionType[ExpressionType["ListInit"] = 22] = "ListInit";
            ExpressionType[ExpressionType["MemberAccess"] = 23] = "MemberAccess";
            ExpressionType[ExpressionType["MemberInit"] = 24] = "MemberInit";
            ExpressionType[ExpressionType["Modulo"] = 25] = "Modulo";
            ExpressionType[ExpressionType["Multiply"] = 26] = "Multiply";
            ExpressionType[ExpressionType["MultiplyChecked"] = 27] = "MultiplyChecked";
            ExpressionType[ExpressionType["Negate"] = 28] = "Negate";
            ExpressionType[ExpressionType["UnaryPlus"] = 29] = "UnaryPlus";
            ExpressionType[ExpressionType["NegateChecked"] = 30] = "NegateChecked";
            ExpressionType[ExpressionType["New"] = 31] = "New";
            ExpressionType[ExpressionType["NewArrayInit"] = 32] = "NewArrayInit";
            ExpressionType[ExpressionType["NewArrayBounds"] = 33] = "NewArrayBounds";
            ExpressionType[ExpressionType["Not"] = 34] = "Not";
            ExpressionType[ExpressionType["NotEqual"] = 35] = "NotEqual";
            ExpressionType[ExpressionType["Or"] = 36] = "Or";
            ExpressionType[ExpressionType["OrElse"] = 37] = "OrElse";
            ExpressionType[ExpressionType["Parameter"] = 38] = "Parameter";
            ExpressionType[ExpressionType["Power"] = 39] = "Power";
            ExpressionType[ExpressionType["Quote"] = 40] = "Quote";
            ExpressionType[ExpressionType["RightShift"] = 41] = "RightShift";
            ExpressionType[ExpressionType["Subtract"] = 42] = "Subtract";
            ExpressionType[ExpressionType["SubtractChecked"] = 43] = "SubtractChecked";
            ExpressionType[ExpressionType["TypeAs"] = 44] = "TypeAs";
            ExpressionType[ExpressionType["TypeIs"] = 45] = "TypeIs";
            ExpressionType[ExpressionType["Assign"] = 46] = "Assign";
            ExpressionType[ExpressionType["Block"] = 47] = "Block";
            ExpressionType[ExpressionType["DebugInfo"] = 48] = "DebugInfo";
            ExpressionType[ExpressionType["Decrement"] = 49] = "Decrement";
            ExpressionType[ExpressionType["Dynamic"] = 50] = "Dynamic";
            ExpressionType[ExpressionType["Default"] = 51] = "Default";
            ExpressionType[ExpressionType["Extension"] = 52] = "Extension";
            ExpressionType[ExpressionType["Goto"] = 53] = "Goto";
            ExpressionType[ExpressionType["Increment"] = 54] = "Increment";
            ExpressionType[ExpressionType["Index"] = 55] = "Index";
            ExpressionType[ExpressionType["Label"] = 56] = "Label";
            ExpressionType[ExpressionType["RuntimeVariables"] = 57] = "RuntimeVariables";
            ExpressionType[ExpressionType["Loop"] = 58] = "Loop";
            ExpressionType[ExpressionType["Switch"] = 59] = "Switch";
            ExpressionType[ExpressionType["Throw"] = 60] = "Throw";
            ExpressionType[ExpressionType["Try"] = 61] = "Try";
            ExpressionType[ExpressionType["Unbox"] = 62] = "Unbox";
            ExpressionType[ExpressionType["AddAssign"] = 63] = "AddAssign";
            ExpressionType[ExpressionType["AndAssign"] = 64] = "AndAssign";
            ExpressionType[ExpressionType["DivideAssign"] = 65] = "DivideAssign";
            ExpressionType[ExpressionType["ExclusiveOrAssign"] = 66] = "ExclusiveOrAssign";
            ExpressionType[ExpressionType["LeftShiftAssign"] = 67] = "LeftShiftAssign";
            ExpressionType[ExpressionType["ModuloAssign"] = 68] = "ModuloAssign";
            ExpressionType[ExpressionType["MultiplyAssign"] = 69] = "MultiplyAssign";
            ExpressionType[ExpressionType["OrAssign"] = 70] = "OrAssign";
            ExpressionType[ExpressionType["PowerAssign"] = 71] = "PowerAssign";
            ExpressionType[ExpressionType["RightShiftAssign"] = 72] = "RightShiftAssign";
            ExpressionType[ExpressionType["SubtractAssign"] = 73] = "SubtractAssign";
            ExpressionType[ExpressionType["AddAssignChecked"] = 74] = "AddAssignChecked";
            ExpressionType[ExpressionType["MultiplyAssignChecked"] = 75] = "MultiplyAssignChecked";
            ExpressionType[ExpressionType["SubtractAssignChecked"] = 76] = "SubtractAssignChecked";
            ExpressionType[ExpressionType["PreIncrementAssign"] = 77] = "PreIncrementAssign";
            ExpressionType[ExpressionType["PreDecrementAssign"] = 78] = "PreDecrementAssign";
            ExpressionType[ExpressionType["PostIncrementAssign"] = 79] = "PostIncrementAssign";
            ExpressionType[ExpressionType["PostDecrementAssign"] = 80] = "PostDecrementAssign";
            ExpressionType[ExpressionType["TypeEqual"] = 81] = "TypeEqual";
            ExpressionType[ExpressionType["OnesComplement"] = 82] = "OnesComplement";
            ExpressionType[ExpressionType["IsTrue"] = 83] = "IsTrue";
            ExpressionType[ExpressionType["IsFalse"] = 84] = "IsFalse";
        })(Meta.ExpressionType || (Meta.ExpressionType = {}));
        var ExpressionType = Meta.ExpressionType;
    })(WebExpressions.Meta || (WebExpressions.Meta = {}));
    var Meta = WebExpressions.Meta;
})(WebExpressions || (WebExpressions = {}));
