docker build -t nllb{0} .
docker run -p {1}:{1} nllb{0}
Start-Process http://localhost:{1}/

Write-output "Make sure there are no errors and that the page at 'http://localhost:{1}' is able to translate before using in SILConverters"
pause 