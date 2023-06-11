package generalhandler

import (
	"encoding/json"
	"fmt"
	"net/http"
	"time"

	"github.com/FlagBrew/CoreAPI/internal/models"
	"github.com/FlagBrew/CoreAPI/internal/utils"
	"github.com/getsentry/sentry-go"
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
	r.Get("/kuma-ping", h.kumaPing)
}

func (h *Handler) kumaPing(w http.ResponseWriter, r *http.Request) {
	chix.JSON(w, r, http.StatusOK, chix.M{
		"status": "ok",
	})
}

func (h *Handler) ping(w http.ResponseWriter, r *http.Request) {
	start := time.Now()
	version, err := utils.RunCoreConsole(r.Context(), "version", "")

	if err != nil {
		// if error code 4, the .env is missing for coreconsole, which should only happen if I forget to set it up, meaning we can tell the user that coreconsole is currently being set-up and to try again later
		if err.Error() == "CoreConsole exited with code 4" {
			w.Header().Set("Content-Type", "application/json")

			chix.JSON(w, r, http.StatusServiceUnavailable, chix.M{
				"error": "CoreConsole is currently being set-up, please try again later",
			})
			return
		}

		// If we got here, then something went wrong but likely wasn't caught by coreconsole inside sentry so we'll deal with it here.
		sentry.CaptureException(err)
		chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
		return
	}

	v := &models.ALMVersion{}
	if err := json.Unmarshal([]byte(version), &v); err != nil {
		sentry.CaptureException(err)
		chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
		return
	}

	chix.JSON(w, r, 200, chix.M{
		"alm_version":   v.ALMVersion,
		"pkhex_version": v.PKHeXVersion,
		"response_time": time.Since(start).Milliseconds(),
	})
}
