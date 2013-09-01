var WebExpressions;
(function (WebExpressions) {
    WebExpressions.Initialize = function (data) {
    };

    var Sanitizer = (function () {
        function Sanitizer() {
        }
        Sanitizer.Require = ///<summary>Class which deals with interactions with non ts code</summary>
        function (item) {
            var properties = [];
            for (var _i = 0; _i < (arguments.length - 1); _i++) {
                properties[_i] = arguments[_i + 1];
            }
            if (item == null) {
                throw "Item must have a value";
            }

            for (var i = 0, ii = properties.length; i < ii; i++) {
                var prop = properties[i];

                if (prop.allowNull && item[prop.inputName] == null)
                    return;
                if (!prop.allowNull && item[prop.inputName] == null)
                    throw "Property " + prop.inputName + " cannot be null";
                if (prop.inputType && typeof item[prop.inputName] !== prop.inputType)
                    throw "Property " + prop.inputName + " must be of type " + prop.inputType;
                if (prop.inputConstructor && item[prop.inputName].constructor !== prop.inputConstructor)
                    throw { message: "Property " + prop.inputName + " must be of a give type", type: prop.inputConstructor };
            }
        };
        return Sanitizer;
    })();
    WebExpressions.Sanitizer = Sanitizer;
})(WebExpressions || (WebExpressions = {}));
