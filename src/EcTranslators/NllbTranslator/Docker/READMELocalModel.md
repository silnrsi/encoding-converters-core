# Local Model Translation Server: {5} ({0}) to {6} ({1})

A containerized Flask web service for translating from {5} ({0}) to {6} ({1}) using a local fine-tuned model.

## Usage: Execute these commands in PowerShell

```powershell
# Build the Docker image (note the final '.' for current directory context)
PS> docker build -t {0}-{1}-translator .

# Run the container with model directory mounted from host
PS> docker run {4}-v "{3}:/app/model" -p {2}:{2} {0}-{1}-translator

# Or use the build script
PS> .\buildDocker.ps1
```

Then in a web browser, navigate to http://localhost:{2}/

## Key Differences from NLLB Project

This project loads a local fine-tuned model instead of downloading one from HuggingFace:

- **Model path**: `{3}`
- **Language pair**: Model will translate from {0} to {1} (only)
- **Docker approach**: Uses volume mount (`-v` flag) to point the Docker container to where the local model resides
- **Build time**: Fast - no large model download during build
- **Runtime**: Model files stay on host system; container uses them via mount

## Configuration

Edit `settings.py` to:
- Change `DEVICE` to -1 for CPU-only (no GPU required)
- Change `PORT` if 8000 is already in use
- Add `API_KEY` for authentication

## API Endpoints (on localhost:{2})

### GET /api/v1/translate/languages/
Returns supported language pair:
```json
{{
  "source": {{"code": "{0}", "name": "{5}"}},
  "target": {{"code": "{1}", "name": "{6}"}}
}}
```

### POST /api/v1/translate/
Translates text from {0} ({5}) to {1} ({6}):
```json
{{
  "sourceLanguage": "{0}",
  "targetLanguage": "{1}",
  "text": "Hello"
}}
```

Response:
```json
{{
  "originalText": "Hello",
  "translatedText": "नमस्ते"
}}
```

## Troubleshooting

- **Volume mount error**: Ensure `{3}` exists
- **Model not loading**: Check that settings.py `LOCAL_MODEL_PATH` should match path to downloaded,
                         fine-tuned model and folder should contain config.json, config.yml, and one or more *.safetensors files                            
                         AND `CONTAINER_MODEL_PATH` matches mounted path in the container (default: /app/model)
- **GPU not available**: Rebuild with `DEVICE = -1 in settings.py` for CPU-only mode
- **Port conflict**: Change `PORT` in settings.py and update the docker run `-p` flag

## Local Development (No Docker)

```powershell
# Install dependencies
pip install transformers sentencepiece fasttext-wheel torch flask gevent

# Run Flask directly
python server.py
```

Set `DEVICE = -1` in settings.py for CPU mode if you don't have a GPU.
