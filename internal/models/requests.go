package models

type GetInfoRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
}

type GetInfoRequestB64 struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
	Base64     string `json:"base64" form:"base64" query:"base64" validate:"required"`
}

type LegalityCheckRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
}

type LegalizeRequest struct {
	Generation string `json:"generation" form:"generation" query:"generation" validate:"omitempty,oneof=1 2 3 4 5 6 7 8 9 LGPE BDSP PLA"`
}
