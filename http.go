package main

import (
	"context"
	"net/http"
	"strings"
	"time"

	"github.com/FlagBrew/CoreAPI/internal/handlers/generalhandler"
	"github.com/FlagBrew/CoreAPI/internal/handlers/infohandler"
	"github.com/FlagBrew/CoreAPI/internal/handlers/legalityhandler"
	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/go-chi/httprate"
	"github.com/lrstanley/chix"
)

func httpServer(ctx context.Context) *http.Server {
	chix.DefaultAPIPrefix = "/api/"

	r := chi.NewRouter()

	if len(cli.Flags.HTTP.TrustedProxies) > 0 {
		r.Use(chix.UseRealIP(cli.Flags.HTTP.TrustedProxies, chix.OptUseXForwardedFor))
	}

	r.Use(
		chix.UseContextIP,
		middleware.RequestID,
		chix.UseStructuredLogger(logger),
		chix.UseDebug(cli.Debug),
		chix.Recoverer,
		middleware.StripSlashes,
		middleware.Compress(5),
		middleware.Maybe(middleware.StripSlashes, func(r *http.Request) bool {
			return !strings.HasPrefix(r.URL.Path, "/debug/")
		}),
		chix.UseNextURL,
		middleware.Timeout(30*time.Second),
		middleware.Throttle(10),
	)

	r.Use(
		chix.UseHeaders(map[string]string{
			"Content-Security-Policy":          "default-src 'self'; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; img-src *; font-src 'self' https://fonts.gstatic.com; media-src *; object-src 'none'; child-src 'none'; frame-src 'none'; worker-src 'none'",
			"X-Frame-Options":                  "DENY",
			"X-Content-Type-Options":           "nosniff",
			"Referrer-Policy":                  "no-referrer-when-downgrade",
			"Permissions-Policy":               "clipboard-write=(self)",
			"Access-Control-Allow-Origin":      ".flagbrew.org",
			"Access-Control-Allow-Methods":     "GET, POST, OPTIONS, PUT, DELETE, PATCH",
			"Access-Control-Allow-Headers":     "Origin, X-Requested-With, Content-Type, Accept, Authorization",
			"Access-Control-Allow-Credentials": "true",
		}),
		// auth.AddToContext,
		httprate.LimitByIP(100, 1*time.Minute),
	)

	r.Use(
		chix.UseSecurityTxt(&chix.SecurityConfig{
			ExpiresIn: 182 * 24 * time.Hour,
			Contacts: []string{
				"mailto:fm1337@fm1337.com",
				"https://github.com/FM1337",
			},
			KeyLinks:  []string{"https://github.com/FM1337.gpg"},
			Languages: []string{"en"},
		}),
	)

	if cli.Debug {
		r.Mount("/debug", middleware.Profiler())
	}

	r.Route("/api/info", infohandler.NewHandler().Route)
	r.Route("/api/legality", legalityhandler.NewHandler().Route)
	r.Route("/api/general", generalhandler.NewHandler().Route)

	return &http.Server{
		Addr:    cli.Flags.HTTP.Addr,
		Handler: r,

		// Some sane defaults.
		ReadTimeout:  20 * time.Second,
		WriteTimeout: 20 * time.Second,
	}
}
