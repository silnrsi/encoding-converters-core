docker build -t nllb{0} .
docker run -p {1}:{1} nllb{0}
# Start-Process http://localhost:{1}/