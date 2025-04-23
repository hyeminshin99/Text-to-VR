from flask import Flask, request, jsonify
import requests

app = Flask(__name__)

def search_sketchfab_glb(prompt, token):
    headers = {"Authorization": f"Token {token}"}
    params = {
        "q": prompt,
        "type": "models",
        "downloadable": "true"  # 다운로드 가능한 모델만 검색
    }
    response = requests.get("https://api.sketchfab.com/v3/search", headers=headers, params=params)

    data = response.json()
    if "results" in data and len(data["results"]) > 0:
        uid = data["results"][0]["uid"]
        # uid로 다운로드 링크 요청
        download_url = get_download_url(uid, token)
        return download_url
    return None

def get_download_url(uid, token):
    headers = {"Authorization": f"Token {token}"}
    url = f"https://api.sketchfab.com/v3/models/{uid}/download"
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        download_data = response.json()
        return download_data["gltf"]["url"]  # glb도 가능
    return None

@app.route('/get_asset', methods=['POST'])
def get_asset():
    data = request.json
    prompt = data.get("prompt")
    print(f"[서버] 받은 프롬프트: {prompt}")

    token = "0d1ec910cbfd40f98c5b4382aef14adb"
    model_url = search_sketchfab_glb(prompt, token)

    if model_url:
        return jsonify({"url": model_url})
    else:
        return jsonify({"error": f"No downloadable models found for prompt: {prompt}"})
        #return jsonify({"url": "http://localhost:5005/static/models/tatami.glb"}) #https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Avocado/glTF-Binary/Avocado.glb

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5005)
