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
	var err bytes.Buffer
	cmd.Stdout = &out
	cmd.Stderr = &err

	ch := make(chan error)
	go func() {
		ch <- cmd.Run()
	}()
	errored := false
	killed := false
	select {
	case <-ctx.Done():
		if err := cmd.Process.Signal(syscall.SIGINT); err != nil {
			fmt.Println("failed to kill process: ", err)
			return "", err
		}
		killed = true
	case err := <-ch:
		if err != nil {
			errored = true
		}
	}

	if killed {
		return "", fmt.Errorf("process killed due request cancellation")
	}

	if errored {
		return err.String(), fmt.Errorf("exit status 1")
	}

	return out.String(), nil
}
