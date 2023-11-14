from flask import Flask, render_template, request, jsonify
from settings import MODEL_NAME, PORT, API_KEY
from gevent.pywsgi import WSGIServer

app = Flask(__name__)

checkpoint = MODEL_NAME
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM, pipeline
model = AutoModelForSeq2SeqLM.from_pretrained(checkpoint)
tokenizer = AutoTokenizer.from_pretrained(checkpoint)

def IsNullOrEmpty(s):
    return s is None or s == ''

@app.route('/')
def send_report():
    # Check if the API key is present in the request headers
    if not IsNullOrEmpty(API_KEY) and ('Authorization' not in request.headers or request.headers['Authorization'] != API_KEY):
        return jsonify({'error': 'Unauthorized'}), 401

    return render_template('index.html', MODEL_NAME=MODEL_NAME)

@app.route('/api/v1/translate/languages/', methods=['GET'])
def translate_languages():
    # Check if the API key is present in the request headers
    if not IsNullOrEmpty(API_KEY) and ('Authorization' not in request.headers or request.headers['Authorization'] != API_KEY):
        return jsonify({'error': 'Unauthorized'}), 401

    return list(tokenizer.lang_code_to_id.keys())

@app.route('/api/v1/translate/', methods=['POST'])
def translate_text():
    # Check if the API key is present in the request headers
    if not IsNullOrEmpty(API_KEY) and ('Authorization' not in request.headers or request.headers['Authorization'] != API_KEY):
        return jsonify({'error': 'Unauthorized'}), 401

    try:
        data = request.get_json()
        source_language = data['sourceLanguage']
        target_language = data['targetLanguage']
        text_to_translate = data['text']

        translator = pipeline('translation', model=model, tokenizer=tokenizer, src_lang=source_language, tgt_lang=target_language, max_length=400)

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