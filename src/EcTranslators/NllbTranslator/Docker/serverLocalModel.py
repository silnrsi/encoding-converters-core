from flask import Flask, render_template, request, jsonify
from settings import CONTAINER_MODEL_PATH, PORT, API_KEY, DEVICE
from settings import SOURCE_LANGUAGE, TARGET_LANGUAGE, SOURCE_LANGUAGE_NAME, TARGET_LANGUAGE_NAME
from gevent.pywsgi import WSGIServer
import os
import glob

app = Flask(__name__)

# Validate model path exists
if not os.path.exists(CONTAINER_MODEL_PATH):
    raise FileNotFoundError(f"Local Model path not found: {CONTAINER_MODEL_PATH}")

# Also validate that key files exist
required_files = ['config.json', '*.safetensors']
for file in required_files:
    file_path = os.path.join(CONTAINER_MODEL_PATH, file)
    if not glob.glob(file_path):
        print(f"Warning: Expected file not found: {file_path}")
    else:
        print(f"Found: {file}")
print(f"Model path validation successful: {CONTAINER_MODEL_PATH}")

# Load model and tokenizer from local path
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM, pipeline
print(f"Loading model from: {CONTAINER_MODEL_PATH}")
model = AutoModelForSeq2SeqLM.from_pretrained(CONTAINER_MODEL_PATH)
tokenizer = AutoTokenizer.from_pretrained(CONTAINER_MODEL_PATH)

def IsNullOrEmpty(s):
    return s is None or s == ''

@app.route('/')
def send_report():
    # Check if the API key is present in the request headers
    if not IsNullOrEmpty(API_KEY) and ('Authorization' not in request.headers or request.headers['Authorization'] != API_KEY):
        return jsonify({'error': 'Unauthorized'}), 401

    return render_template('index.html', 
                         SOURCE_LANG=SOURCE_LANGUAGE,
                         TARGET_LANG=TARGET_LANGUAGE,
                         SOURCE_LANG_NAME=SOURCE_LANGUAGE_NAME,
                         TARGET_LANG_NAME=TARGET_LANGUAGE_NAME)

@app.route('/api/v1/translate/languages/', methods=['GET'])
def translate_languages():
    # Check if the API key is present in the request headers
    if not IsNullOrEmpty(API_KEY) and ('Authorization' not in request.headers or request.headers['Authorization'] != API_KEY):
        return jsonify({'error': 'Unauthorized'}), 401

    # Return supported language pairs for this model
    languages = {
        'source': {
            'code': SOURCE_LANGUAGE,
            'name': SOURCE_LANGUAGE_NAME
        },
        'target': {
            'code': TARGET_LANGUAGE,
            'name': TARGET_LANGUAGE_NAME
        }
    }
    return jsonify(languages)

@app.route('/api/v1/translate/', methods=['POST'])
def translate_text():
    # Check if the API key is present in the request headers
    if not IsNullOrEmpty(API_KEY) and ('Authorization' not in request.headers or request.headers['Authorization'] != API_KEY):
        return jsonify({'error': 'Unauthorized'}), 401

    try:
        data = request.get_json()
        source_language = data.get('sourceLanguage', SOURCE_LANGUAGE)
        target_language = data.get('targetLanguage', TARGET_LANGUAGE)
        text_to_translate = data['text']

        # This model translates from source to target language pair only
        translator = pipeline('translation', model=model, tokenizer=tokenizer,
                            src_lang=source_language, tgt_lang=target_language,
                            max_length=400, device=DEVICE)

        translated_text = translator(text_to_translate)[0]['translation_text']

        response = {
            'originalText': text_to_translate,
            'translatedText': translated_text
        }

        return jsonify(response)

    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    # Debug/Development:
    # app.run(host="0.0.0.0", port=PORT, debug=True)
    # Production:
    http_server = WSGIServer(('', PORT), app)
    http_server.serve_forever()
