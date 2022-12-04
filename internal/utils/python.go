package utils

import (
	"bytes"
	"context"
	"fmt"
	"os/exec"
	"syscall"
)

func RunCorePython(mode, pokemon, generation string, ctx context.Context) (string, error) {
	cmd := exec.Command("python", "python/main.py", "--mode", mode, "--pkmn", pokemon)
	if generation != "" {
		cmd.Args = append(cmd.Args, "--generation", generation)
	}

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
