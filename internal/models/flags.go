package models

type Flags struct {
	Configured bool        `long:"configured" env:"CONFIGURED" required:"true" description:"If set to false, the web application will exit, should be set to true when everything is configured correctly"`
	Env        string      `short:"e" long:"env" env:"ENV" required:"true" description:"The environment the program is running in: production/development"`
	HTTP       ConfigHTTP  `group:"HTTP Server Options" namespace:"http" env-namespace:"HTTP"`
	Logging    LoggingKeys `group:"Logging Options" namespace:"logging" env-namespace:"LOGGING"`
}

type ConfigHTTP struct {
	Addr           string   `short:"a" long:"addr" default:":8080" env:"ADDR" description:"ip:port pair to bind to" required:"true"`
	BaseURL        string   `long:"base-url" required:"true" env:"BASE_URL"`
	SessionKey     string   `long:"session-key" env:"SESSION_KEY" description:"HTTP salted session key (change this to logout all users)"`
	TrustedProxies []string `long:"trusted-proxies" env:"TRUSTED_PROXIES" descriotion:"set of CIDR ranges that are allowed to provide an X-Forwarded-For header"`
	ValidationKey  string   `env:"VALIDATION_KEY"  long:"validation-key"  required:"true" description:"key used to validate session cookies (32 or 64 bytes)"`
	EncryptionKey  string   `env:"ENCRYPTION_KEY"  long:"encryption-key"  required:"true" description:"key used to encrypt session cookies (32 bytes)"`
}

type LoggingKeys struct {
	SentryDSN string `env:"SENTRY_DSN" long:"sentry-dsn" description:"Sentry DSN" required:"true"`
}
