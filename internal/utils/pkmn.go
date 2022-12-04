package utils

import (
	"bytes"
	"encoding/base64"
	"fmt"
	"io"
	"net/http"
)

func GetPkmnFromRequest(w http.ResponseWriter, r *http.Request) (string, error) {
	if r.ContentLength > 1024 {
		return "", fmt.Errorf("file too large")
	}

	r.Body = http.MaxBytesReader(w, r.Body, 1024) // 512 bytes
	if err := r.ParseMultipartForm(500 * 1024); err != nil {
		return "", err
	}

	file, _, err := r.FormFile("pkmn")
	if err != nil {
		return "", err
	}

	defer file.Close()
	// Copy the file to a byte array
	var buf bytes.Buffer

	if _, err := io.Copy(&buf, file); err != nil {
		return "", err
	}

	// Convert the file to base64
	return base64.StdEncoding.EncodeToString(buf.Bytes()), nil
}
