# Part 1: Building a CNN
from keras import backend as K
import numpy as np
from keras.layers import Conv2D, MaxPooling2D, Flatten, Dense, Dropout
from keras.preprocessing.image import ImageDataGenerator
# Import Keras packages
from keras.models import Sequential
import os
import tensorflow as tf
from sklearn.metrics import f1_score


from keras.src.callbacks import EarlyStopping

os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'


def f1_macro(y_true, y_pred):
    def f1_per_class(y_true, y_pred, class_id):
        true_positives = K.sum(K.round(K.clip(y_true[:, class_id] * y_pred[:, class_id], 0, 1)))
        possible_positives = K.sum(K.round(K.clip(y_true[:, class_id], 0, 1)))
        predicted_positives = K.sum(K.round(K.clip(y_pred[:, class_id], 0, 1)))

        precision = true_positives / (predicted_positives + K.epsilon())
        recall = true_positives / (possible_positives + K.epsilon())

        f1 = 2 * (precision * recall) / (precision + recall + K.epsilon())
        return f1

    num_classes = K.int_shape(y_pred)[1]
    f1_scores = [f1_per_class(y_true, y_pred, i) for i in range(num_classes)]
    macro_f1 = K.mean(K.stack(f1_scores), axis=0)

    return macro_f1


def custom_f1(y_true, y_pred):
    def recall_m(y_true, y_pred):
        TP = K.sum(K.round(K.clip(y_true * y_pred, 0, 1)))
        Positives = K.sum(K.round(K.clip(y_true, 0, 1)))

        recall = TP / (Positives+K.epsilon())
        return recall


    def precision_m(y_true, y_pred):
        TP = K.sum(K.round(K.clip(y_true * y_pred, 0, 1)))
        Pred_Positives = K.sum(K.round(K.clip(y_pred, 0, 1)))

        precision = TP / (Pred_Positives+K.epsilon())
        return precision

    precision, recall = precision_m(y_true, y_pred), recall_m(y_true, y_pred)

    return 2*((precision*recall)/(precision+recall+K.epsilon()))

# Initializing the CNN
np.random.seed(1337)
classifier = Sequential()

classifier.add(Conv2D(32, (3, 3), input_shape=(256, 256, 3), activation='relu'))
classifier.add(MaxPooling2D(pool_size=(2, 2)))
classifier.add(Conv2D(16, (3, 3), activation='relu'))
classifier.add(MaxPooling2D(pool_size=(2, 2)))
classifier.add(Conv2D(8, (3, 3), activation='relu'))
classifier.add(MaxPooling2D(pool_size=(2, 2)))

classifier.add(Flatten())

# Hidden layer
classifier.add(Dense(units=128, activation='relu'))
classifier.add(Dropout(rate=0.5))  # Use 'rate' instead of 'p'
# Output layer
classifier.add(Dense(units=10, activation='softmax'))

classifier.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy', f1_macro])
print(classifier.summary())

# Part 2: Fitting the dataset


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

label_map = training_set.class_indices
print(label_map)

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
# Adjust steps_per_epoch and validation_steps based on the size of your dataset
classifier.fit(
    training_set,
    steps_per_epoch=len(training_set),
    epochs=1000,
    validation_data=validation_set,
    validation_steps=len(validation_set),
    callbacks=[early_stopping])

classifier.save('inceptionV3.keras')
print('Saved trained model as %s' % 'custom_architecture.keras')
