package legalityhandler

import (
	"fmt"
	"net/http"

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
	if err != nil {
		if err.Error() == "file too large" {
			w.Header().Set("Content-Type", "application/json")

			chix.JSON(w, r, http.StatusRequestEntityTooLarge, chix.M{
				"error": "file too large",
			})
			return
		}

		chix.Error(w, r, err)
		sentry.CaptureException(err)
		return
	}

	// Run the coreconsole script
	var extraArgs []string
	if legalityReportRequest.Generation != "" {
		extraArgs = append(extraArgs, fmt.Sprintf("--generation=%s", legalityReportRequest.Generation))
	}

	output, err := utils.RunCoreConsole(r.Context(), "legality", pkmn, extraArgs...)
	if err != nil {
		// if error code 1 or 2, the pkmn provided is invalid.
		if err.Error() == "CoreConsole exited with code 1" || err.Error() == "CoreConsole exited with code 2" {
			w.Header().Set("Content-Type", "application/json")
			w.WriteHeader(http.StatusBadRequest)

			w.Write([]byte(output))
			return
		}

		// if error code 3, something unknown happened, but should've been captured by sentry inside coreconsole itself.
		if err.Error() == "CoreConsole exited with code 3" {
			chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
			return
		}

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
	if err != nil {
		if err.Error() == "file too large" {
			w.Header().Set("Content-Type", "application/json")

			chix.JSON(w, r, http.StatusRequestEntityTooLarge, chix.M{
				"error": "file too large",
			})
			return
		}

		chix.Error(w, r, err)
		sentry.CaptureException(err)
		return
	}

	// Run the coreconsole script
	var extraArgs []string
	if autoLegalityRequest.Generation != "" {
		extraArgs = append(extraArgs, fmt.Sprintf("--generation=%s", autoLegalityRequest.Generation))
	}

	if autoLegalityRequest.ForcedGeneration != "" {
		extraArgs = append(extraArgs, fmt.Sprintf("--legalization-generation=%s", autoLegalityRequest.ForcedGeneration))
	}

	if autoLegalityRequest.ForcedVersion != "" {
		extraArgs = append(extraArgs, fmt.Sprintf("--version=%s", autoLegalityRequest.ForcedVersion))
	}

	output, err := utils.RunCoreConsole(r.Context(), "legalize", pkmn, extraArgs...)
	if err != nil {
		// if error code 1 or 2, the pkmn provided is invalid.
		if err.Error() == "CoreConsole exited with code 1" || err.Error() == "CoreConsole exited with code 2" {
			w.Header().Set("Content-Type", "application/json")
			w.WriteHeader(http.StatusBadRequest)

			w.Write([]byte(output))
			return
		}

		// if error code 3, something unknown happened, but should've been captured by sentry inside coreconsole itself.
		if err.Error() == "CoreConsole exited with code 3" {
			chix.Error(w, r, fmt.Errorf("something went wrong, please try again later"))
			return
		}

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
	// we got JSON back, so we can just write it to the response
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(output))
}
