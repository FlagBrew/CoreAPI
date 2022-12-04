package models

type GetInfoRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
}

type LegalityCheckRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
}

type LegalizeRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
}
