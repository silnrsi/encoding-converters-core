# This file exists because:
# 1. Documentation for using transformers often shows how to download them in Python
# 2. We want the download of the model to happen as part of the building of the image, not when it runs

import os
from settings import MODEL_NAME
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM

checkpoint = MODEL_NAME
hf_token = os.environ.get("HF_TOKEN")

model_kwargs = {"token": hf_token} if hf_token else {}

model = AutoModelForSeq2SeqLM.from_pretrained(checkpoint, **model_kwargs)
tokenizer = AutoTokenizer.from_pretrained(checkpoint, **model_kwargs)
