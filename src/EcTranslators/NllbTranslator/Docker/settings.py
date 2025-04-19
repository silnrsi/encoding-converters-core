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

# You can load whichever version of the nllb model you want as long as you have the memory to run it
# one of  'facebook/nllb-200-3.3B'              # requires > 32GB of ram
#         'facebook/nllb-200-distilled-1.3B'    # can be done w/ 32GB of ram
#         'facebook/nllb-200-distilled-600M'    # can be done w/ 16GB of ram
MODEL_NAME = {2}

# You can have the nllb model use a GPU if your computer has one
# one of  -1    # use cpu
#         0     # use gpu (you can replace 0, if you have multiple GPUs and want to use a different one)
# Also, if you change this to use a GPU, then the docker run command will need to be changed to include the --gpus "device=0" option
#  (or whatever GPU index you want to use, if you have multiple). e.g. `docker --gpus "device=0" run -p 8000:8000 nllb-600m`
DEVICE = {3}
