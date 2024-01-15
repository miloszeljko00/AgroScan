import os

from flask import Flask, request, jsonify
from keras.models import load_model
import numpy as np
from keras.preprocessing.image import img_to_array, load_img
from io import BytesIO
import base64

app = Flask(__name__)

os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'

model = load_model('model.keras')

label_mapping = {
    0: 'Tomato___Bacterial_spot',
    1: 'Tomato___Early_blight',
    2: 'Tomato___Late_blight',
    3: 'Tomato___Leaf_Mold',
    4: 'Tomato___Septoria_leaf_spot',
    5: 'Tomato___Spider_mites Two-spotted_spider_mite',
    6: 'Tomato___Target_Spot',
    7: 'Tomato___Tomato_Yellow_Leaf_Curl_Virus',
    8: 'Tomato___Tomato_mosaic_virus',
    9: 'Tomato___healthy'
}


def preprocess_image(base64_string):
    img_data = base64.b64decode(base64_string)
    img = load_img(BytesIO(img_data), target_size=(256, 256))
    img_array = img_to_array(img)
    img_array = np.expand_dims(img_array, axis=0)
    img_array /= 255.0  # Rescale to [0, 1]
    return img_array


@app.route('/tomato/predict', methods=['POST'])
def predict():
    data = request.get_json()
    base64_image = data['image']
    img_array = preprocess_image(base64_image)

    prediction = model.predict(img_array)
    predicted_class = np.argmax(prediction, axis=1)[0]
    predicted_label = label_mapping[predicted_class]

    response = {
        'label': predicted_label
    }

    return jsonify(response)


if __name__ == '__main__':
    app.run(debug=True)
