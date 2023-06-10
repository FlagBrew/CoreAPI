package generalhandler

import (
	"encoding/json"
	"fmt"
	"net/http"
	"time"

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
	r.Get("/ping", h.ping)
}

func (h *Handler) ping(w http.ResponseWriter, r *http.Request) {
	start := time.Now()
	version, err := utils.RunCoreConsole(r.Context(), "version", "")

	if err != nil {
		chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
		return
	}

	v := &models.ALMVersion{}
	if err := json.Unmarshal([]byte(version), &v); err != nil {
		fmt.Println(err)
		chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
		return
	}

	chix.JSON(w, r, 200, chix.M{
		"alm_version":   v.ALMVersion,
		"pkhex_version": v.PKHeXVersion,
		"response_time": time.Since(start).Milliseconds(),
	})
}
