from keras import backend as K, Model
import numpy as np
from keras.layers import Dense
from keras.preprocessing.image import ImageDataGenerator
from keras.models import Sequential
import os
from keras.src.applications import InceptionV3
from keras.src.layers import GlobalAveragePooling2D


from keras.src.callbacks import EarlyStopping

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


np.random.seed(1337)
classifier = Sequential()

base_model = InceptionV3(weights='imagenet', include_top=False, input_shape=(256, 256, 3))
x = base_model.output
x = GlobalAveragePooling2D()(x)
x = Dense(1024, activation='relu')(x)
predictions = Dense(10, activation='softmax')(x)
classifier = Model(inputs=base_model.input, outputs=predictions)

for layer in base_model.layers:
    layer.trainable = False


classifier.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy', f1_macro])


train_datagen = ImageDataGenerator(
    rescale=1./255,
    shear_range=0.4,
    zoom_range=0.4,
    horizontal_flip=True,
    rotation_range=45,
    width_shift_range=0.2,
    height_shift_range=0.2,
    brightness_range=[0.5, 1.5],
    channel_shift_range=50,
    vertical_flip=True)
training_set = train_datagen.flow_from_directory(
    '../dataset/train',
    target_size=(256, 256),
    batch_size=32,
    class_mode='categorical')


validation_datagen = ImageDataGenerator(
    rescale=1./255,
    shear_range=0.4,
    zoom_range=0.4,
    horizontal_flip=True,
    rotation_range=45,
    width_shift_range=0.2,
    height_shift_range=0.2,
    brightness_range=[0.5, 1.5],
    channel_shift_range=50,
    vertical_flip=True)
validation_set = validation_datagen.flow_from_directory(
    '../dataset/validation',
    target_size=(256, 256),
    batch_size=32,
    class_mode='categorical')


early_stopping = EarlyStopping(monitor='val_loss', patience=30, restore_best_weights=True)


classifier.fit(
    training_set,
    steps_per_epoch=len(training_set),
    epochs=1000,
    validation_data=validation_set,
    validation_steps=len(validation_set),
    callbacks=[early_stopping])

classifier.save('resnet.keras')
print('Saved trained model as %s' % 'resnet.keras')
