package infohandler

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
	r.Post("/", h.getInfo)
}

func (h *Handler) getInfo(w http.ResponseWriter, r *http.Request) {
	infoRequest := &models.GetInfoRequest{}

	if chix.Error(w, r, chix.Bind(r, infoRequest)) {
		return
	}

	// Get the pkmn in base64
	pkmn, err := utils.GetPkmnFromRequest(w, r)
	if chix.Error(w, r, err) {
		return
	}

	// Run the core python script
	output, err := utils.RunCorePython("info", pkmn, infoRequest.Generation, r.Context())
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
