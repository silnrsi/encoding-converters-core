# Define your API key: it can only be alphanumeric characters + '-'; nothing else (i.e. [a-zA-Z-])
# the prefix, 'SIL-NLLB-Auth-Key ' will be added, so if you're using this in the 'Nllb Translator' 
# Setup tab of SILConverters, just enter the part after that (i.e. your-api-key-here)
API_KEY = {0}    # e.g. 'SIL-NLLB-Auth-Key your-api-key-here'

# You can either access the model locally via http://localhost:8000/ or from another machine on the network by using 
#  the IP address of the machine hosting the docker container (e.g. http://192.168.69.156:8000/) AND you can use a 
#  different port if you want, but you might need to open the port in your Firewall (e.g. for Windows, see
#  https://learn.microsoft.com/en-us/sql/reporting-services/report-server/configure-a-firewall-for-report-server-access?view=sql-server-ver16#to-open-port-80)
# Also, see the README.md for how the "docker run" command would be changed if you change the port (e.g. changing to '8080'
#  would make the command, for example, `docker run -p 8080:8080 nllb-600m`)
PORT = {1}        # e.g. 8000

# Local Model Path Configuration
# Adjust this to point to your local model checkpoint directory
LOCAL_MODEL_PATH = r'{2}'
CONTAINER_MODEL_PATH = '/app/model'

# Device selection
# -1 = CPU, 0 = GPU (first device)
# Also, if you change this to use a GPU, then the docker run command will need to be changed to include the --gpus "device=0" option on the docker run statement (see buildDocker.ps1)
# e.g. `docker run --gpus "device=0" -v "C:\vscode\Models\Kangri\hin_xnr:/app/model" -p 8000:8000 hin-xnr-translator`
DEVICE = {3}

# Source and target language codes for this specific model
# Format: string identifiers (not FAIRSEQ codes like the NLLB model)
SOURCE_LANGUAGE = '{4}'
TARGET_LANGUAGE = '{5}'
SOURCE_LANGUAGE_NAME = '{6}'
TARGET_LANGUAGE_NAME = '{7}'