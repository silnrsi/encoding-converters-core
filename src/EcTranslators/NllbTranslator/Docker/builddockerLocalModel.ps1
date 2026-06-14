# Build command
docker build -t {0}-{1}-translator .

# Run command with volume mount for local model
# The model directory is mounted from the Windows host into the container
docker run {4}-v "{3}:/app/model" -p {2}:{2} {0}-{1}-translator

# To view the translator in a web browser, in another powershell window run:
# Start-Process http://localhost:{2}/