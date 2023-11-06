FROM python:3.9

# Install steps mostly taken from https://medium.com/mlearning-ai/text-translation-using-nllb-and-huggingface-tutorial-7e789e0f7816

# Install main dependencies
RUN pip install transformers sentencepiece fasttext
# Install PyTorch. This command varies depending on your OS and other factors; see https://pytorch.org/get-started/locally/ for instructions for your system
RUN pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cpu   # if you have a gpu: remove: https://download.pytorch.org/whl/cpu

COPY settings.py .
COPY import_model.py .

RUN python import_model.py

RUN pip install flask

COPY . .

CMD ["python", "server.py"]
