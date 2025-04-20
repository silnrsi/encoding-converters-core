docker build -t nllb{0} .
docker run {2}-p {1}:{1} nllb{0}
# to view the translator in a web browser, in another powershell window run:
# Start-Process http://localhost:{1}/