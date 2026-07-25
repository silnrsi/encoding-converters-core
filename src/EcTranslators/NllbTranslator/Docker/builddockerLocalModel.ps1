# Build command
docker build {2}-t {0}-{1}-translator .

# Run command with volume mount for local model
# The model directory is mounted from the Windows host into the container
docker run {5}-v "{4}:/app/model" -p {3}:{3} {0}-{1}-translator

# To view the translator in a web browser, in another powershell window run:
# Start-Process http://localhost:{3}/