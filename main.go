package main

import (
	"context"
	"time"

	"github.com/FlagBrew/CoreAPI/internal/models"
	"github.com/apex/log"
	"github.com/getsentry/sentry-go"
	"github.com/lrstanley/chix"
	"github.com/lrstanley/clix"
)

var (
	cli = &clix.CLI[models.Flags]{
		Links: clix.GithubLinks("github.com/FlagBrew/CoreAPI", "master", "https://coreapi.flagbrew.org"),
	}

	logger log.Interface
)

func main() {
	// Setup the Cli and Logger
	cli.Parse()
	logger = cli.Logger

	// Ensure configured is set to true
	if !cli.Flags.Configured {
		logger.Fatal("Not configured yet, please configure")
	}

	sentry.Init(sentry.ClientOptions{
		Dsn: cli.Flags.Logging.SentryDSN,
	})

	ctx := context.Background()

	if err := chix.RunCtx(
		ctx, httpServer(ctx),
	); err != nil {
		defer sentry.Flush(2 * time.Second)
		log.WithError(err).Fatal("shutting down")
	}
}
