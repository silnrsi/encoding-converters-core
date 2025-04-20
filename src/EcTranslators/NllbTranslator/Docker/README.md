# Usage: execute these commands in Powershell

PS> docker build -t nllb{0} .            # note: include the space and '.' at the end when copying to Powershell
PS> docker run {2}-p {1}:{1} nllb{0}

Then in a web browser, navigate to http://localhost:{1}/
