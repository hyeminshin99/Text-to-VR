from flask import Flask, request, jsonify

app = Flask(__name__)


@app.route('/get_asset', methods=['POST'])
def get_asset():
    data = request.json
    prompt = data.get("prompt")
    print(f"[서버] 받은 프롬프트: {prompt}")

    # 테스트용 dummy URL 응답
    return jsonify({"url": "https://example.com/dummy.glb"})


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5005)
