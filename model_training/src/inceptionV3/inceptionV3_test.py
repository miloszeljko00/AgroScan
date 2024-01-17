import keras
from keras import backend as K
from keras.preprocessing import image
from keras.models import load_model
import numpy as np
from keras.preprocessing.image import ImageDataGenerator
from sklearn.metrics import f1_score, accuracy_score, classification_report
import os

os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'


def f1_macro(y_true, y_pred):
    def f1_per_class(y_true, y_pred, class_id):
        true_positives = K.sum(K.round(K.clip(y_true[:, class_id] * y_pred[:, class_id], 0, 1)))
        possible_positives = K.sum(K.round(K.clip(y_true[:, class_id], 0, 1)))
        predicted_positives = K.sum(K.round(K.clip(y_pred[:, class_id], 0, 1)))

        precision = true_positives / (predicted_positives + K.epsilon())
        recall = true_positives / (possible_positives + K.epsilon())

        return 2 * (precision * recall) / (precision + recall + K.epsilon())

    num_classes = K.int_shape(y_pred)[1]
    f1_scores = [f1_per_class(y_true, y_pred, i) for i in range(num_classes)]
    macro_f1 = K.mean(K.stack(f1_scores), axis=0)

    return macro_f1



model = load_model('inceptionV3.keras', custom_objects={'f1_macro': f1_macro})


test_dir = '../../dataset/test'

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


test_datagen = ImageDataGenerator(rescale=1./255)
test_dataset = test_datagen.flow_from_directory(
    test_dir,
    target_size=(256, 256),
    batch_size=32,
    class_mode='categorical',
    shuffle=False
)


y_true = test_dataset.labels
y_pred = model.predict(test_dataset)
y_pred_classes = np.argmax(y_pred, axis=1)


print("Classification Report:")
print(classification_report(y_true, y_pred_classes, target_names=label_mapping.values()))
accuracy = accuracy_score(y_true, y_pred_classes) * 100
f1 = f1_score(y_true, y_pred_classes, average='macro')
print(f"Accuracy on the test set: {accuracy:.2f}%")
print(f"F1 macro on the test set: {f1:.4f}")
