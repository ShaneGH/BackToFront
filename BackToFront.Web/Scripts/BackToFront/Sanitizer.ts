
/// <reference path="../ref/jquery.d.ts" />
/// <reference path="../ref/jquery.validation.d.ts" />
/// <reference path="MetaClasses.ts" />

module BackToFront {

    export interface RequiredProperty {
        inputName: string;
        inputType?: string;
        inputConstructor?: any;
        allowNull?: bool;
    }

    export class Sanitizer {
        ///<summary>Class which deals with interactions with non ts code</summary>

        static Require(item: any, ...properties: RequiredProperty[]) {
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
        }
    }
}