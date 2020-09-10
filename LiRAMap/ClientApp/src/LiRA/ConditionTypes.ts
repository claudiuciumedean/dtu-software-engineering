//Populate this file from server-side to follow the types. This file could be server-generated based on the database.
//For now, these are based on the old condition types in the database of the previous LiRA Map project.

export interface ConditionType {
	//Name: The name of the condition type
	Name: string;

	//Description: The description of the condition type
	Description: string;

	//ValueDescriptor: A string describing the interpretation of the value (i.e. "Amount" or "Size")
	ValueDescriptor: string;

	//TranslateValue: Function that translates the numerical value to the representative string (i.e. 45.5 => "45.5%")
	TranslateValue(value: number): string;

	//ValueScale: Function that translates the scale of the value. When the result of this is 1, the condition will be considered 100% severe.
	//It can also be an array of two numbers, where the scale will be the linear between the two.
	//It can also be a single number, in which case this is the upper limit and the lower limit is implicitly 0.
	//TODO: Implement [number, number] and (number) => number support.
	ValueScale: number;
}

export var Types: ConditionType[] = [
	{ //0: Unknown
		Name: "Unknown Condition",
		Description: "Unknown Condition type.",
		ValueDescriptor: "Severity",
		TranslateValue: (s: number) => "[" + s + "]",
		ValueScale: 100
	},

	{ //1
		Name: "Flaking/Delamination",
		Description: "Loose or removed parts of the road. Often started as bumps due to water and pressure from underneath.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //2
		Name: "Raveling/Cuttings",
		Description: "Loss of stone material in the surface. Often seen as a collection of many small holes and a general porous asphalt.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //3
		Name: "Cracks",
		Description: "The separation of asphalt that is no longer able to hold together.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //4
		Name: "Rutting",
		Description: "The indentation of the asphalt on a road. Caused by repeated heavy pressure over time.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //5
		Name: "Unstable Wearing Course",
		Description: "Shifting of the road surface as a whole, which has been pushed to the side due to traffic and the curvature of the ground.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "cm",
		ValueScale: 100
	},

	{ //6
		Name: "Aggregate loss OB",
		Description: "The lack of stone in the surface material.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //7
		Name: "Bleeding",
		Description: "Smoothing of the road surface causing a slippery effect.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //8
		Name: "Alligator cracks",
		Description: "A net pattern of connected cracks.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "%",
		ValueScale: 100
	},

	{ //9
		Name: "Depressions and settlement",
		Description: "Portion of the road which has sunk to a degree due to soft underground.",
		ValueDescriptor: "Amount",
		TranslateValue: (s: number): string => s + "cm",
		ValueScale: 100
	},

	{ //10
		Name: "Pot hole",
		Description: "An areal damage in the form of a hole in the road.",
		ValueDescriptor: "Diameter",
		TranslateValue: (s: number): string => s + "cm",
		ValueScale: 100
	},
]