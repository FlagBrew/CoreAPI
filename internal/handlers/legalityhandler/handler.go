package legalityhandler

import (
	"encoding/json"
	"fmt"
	"net/http"

	"github.com/FlagBrew/CoreAPI/internal/models"
	"github.com/FlagBrew/CoreAPI/internal/utils"
	"github.com/go-chi/chi/v5"
	"github.com/lrstanley/chix"
)

type Handler struct {
}

func NewHandler() *Handler {
	return &Handler{}
}

func (h *Handler) Route(r chi.Router) {
	r.Post("/check", h.legalityReport)
	r.Post("/legalize", h.autoLegality)
}

func (h *Handler) legalityReport(w http.ResponseWriter, r *http.Request) {
	legalityReportRequest := &models.LegalityCheckRequest{}

	if chix.Error(w, r, chix.Bind(r, legalityReportRequest)) {
		return
	}

	// Get the pkmn in base64
	pkmn, err := utils.GetPkmnFromRequest(w, r)
	if chix.Error(w, r, err) {
		return
	}

	// Run the core python script
	output, err := utils.RunCorePython("report", pkmn, legalityReportRequest.Generation, r.Context())
	if err != nil {
		if err.Error() != "exit status 1" {
			chix.Error(w, r, err)
			return
		}

		// try to parse the output as JSON
		var js json.RawMessage
		if json.Unmarshal([]byte(output), &js) != nil {
			// this is not JSON, DO NOT RETURN THIS TO THE USER
			chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
			return
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusBadRequest)

		w.Write([]byte(output))
		return
	}
	// we got JSON back, so we can just write it to the response
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(output))
}

func (h *Handler) autoLegality(w http.ResponseWriter, r *http.Request) {
	autoLegalityRequest := &models.LegalizeRequest{}

	if chix.Error(w, r, chix.Bind(r, autoLegalityRequest)) {
		return
	}

	// Get the pkmn in base64
	pkmn, err := utils.GetPkmnFromRequest(w, r)
	if chix.Error(w, r, err) {
		return
	}

	// Run the core python script
	output, err := utils.RunCorePython("legalize", pkmn, autoLegalityRequest.Generation, r.Context())
	if err != nil {
		if err.Error() != "exit status 1" {
			chix.Error(w, r, err)
			return
		}

		// try to parse the output as JSON
		var js json.RawMessage
		if json.Unmarshal([]byte(output), &js) != nil {
			// this is not JSON, DO NOT RETURN THIS TO THE USER
			chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
			return
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusBadRequest)

		w.Write([]byte(output))
		return
	}
	// we got JSON back, so we can just write it to the response
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(output))
}
