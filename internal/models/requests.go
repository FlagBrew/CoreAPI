package models

type GetInfoRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=Gen6 Gen7 Gen8 Gen8b"`
}

type GetInfoRequestB64 struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=Gen6 Gen7 Gen8 Gen8b"`
}

type LegalityCheckRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=Gen6 Gen7 Gen8 Gen8b"`
}

type LegalizeRequest struct {
	Generation       string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=Gen6 Gen7 Gen8 Gen8b"`
	ForcedGeneration string `json:"forced_generation" form:"forced_generation" query:"forced_generation" validate:"omitempty,oneof=Gen1 Gen2 Gen3 Gen4 Gen5 Gen6 Gen7 Gen7b Gen8 Gen8a Gen8b Gen9 MaxInvalid None SplitInvalid"`
	ForcedVersion    string `json:"forced_version" form:"forced_version" query:"forced_version" validate:"omitempty,oneof=Any AS B B2 B2W2 BATREV BD BDSP BU BW C COLO CXD D DP DPPt E FR FRLG GD GE Gen1 Gen2 Gen3 Gen4 Gen5 Gen6 Gen7 Gen7b Gen8 Gen9 GG GN GO GP GS GSC HG HGSS Invalid LG MN OR ORAS ORASDEMO P PLA Pt R RB RBY RD RS RSBOX RSE S SH SI SL SM SN SP SS Stadium Stadium2 StadiumJ SV SW SWSH UM Unknown US USUM VL W W2 X XD XY Y YW"`
}
