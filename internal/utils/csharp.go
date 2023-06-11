package utils

import (
	"bytes"
	"context"
	"fmt"
	"os/exec"
	"syscall"
)

func RunCoreConsole(ctx context.Context, mode, pokemon string, extraArgs ...string) (string, error) {
	args := []string{mode}
	if pokemon != "" {
		args = append(args, pokemon)
	}
	args = append(args, extraArgs...)
	cmd := exec.Command("./coreconsole", args...)
	cmd.Dir = "./cc"

	var out bytes.Buffer
	var errBytes bytes.Buffer
	cmd.Stdout = &out
	cmd.Stderr = &errBytes

	ch := make(chan error)
	go func() {
		ch <- cmd.Run()
	}()
	errored := false
	killed := false
	exitCode := 0
	select {
	case <-ctx.Done():
		if err := cmd.Process.Signal(syscall.SIGINT); err != nil {
			return "", err
		}
		killed = true
	case err := <-ch:
		if err != nil {
			if exitError, ok := err.(*exec.ExitError); ok {
				exitCode = exitError.ExitCode()
			}
			errored = true
		}
	}

	if killed {
		return "", fmt.Errorf("process killed due request cancellation")
	}

	if errored {
		return errBytes.String(), fmt.Errorf("CoreConsole exited with code %d", exitCode)
	}

	return out.String(), nil
}
