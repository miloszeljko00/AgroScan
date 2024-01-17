import os

from keras.layers import Dense, GlobalAveragePooling2D, BatchNormalization, Dropout
from keras.applications import InceptionV3
from keras.models import Model
from keras.optimizers import Adam
from keras.preprocessing.image import ImageDataGenerator
from keras.callbacks import EarlyStopping, ReduceLROnPlateau
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'


def build_model():
    base_model = InceptionV3(weights='imagenet', include_top=False, input_shape=(256, 256, 3))
    x = base_model.output
    x = GlobalAveragePooling2D()(x)
    x = Dense(256, activation='relu')(x)
    x = BatchNormalization()(x)
    x = Dropout(0.5)(x)
    predictions = Dense(10, activation='softmax')(x)
    model = Model(inputs=base_model.input, outputs=predictions)

    for layer in base_model.layers:
        layer.trainable = False

    return model


def compile_model(model):
    model.compile(optimizer=Adam(), loss='categorical_crossentropy', metrics=['accuracy'])
    return model


def setup_data_generator(directory, augmentation_params, target_size=(256, 256), batch_size=32):
    data_generator = ImageDataGenerator(**augmentation_params)

    return data_generator.flow_from_directory(
        directory,
        target_size=target_size,
        batch_size=batch_size,
        class_mode='categorical'
    )


def main():
    augmentation_params = {
        'rescale': 1. / 255,
        'shear_range': 0.2,
        'zoom_range': 0.2,
        'rotation_range': 20,
        'width_shift_range': 0.2,
        'height_shift_range': 0.2,
        'brightness_range': [0.8, 1.2],
        'channel_shift_range': 25,
        'vertical_flip': True,
        'fill_mode': 'nearest'
    }

    training_set = setup_data_generator('../../dataset/train', augmentation_params)
    validation_set = setup_data_generator('../../dataset/validation', augmentation_params)

    model = build_model()
    model = compile_model(model)

    early_stopping = EarlyStopping(monitor='val_loss', patience=30, restore_best_weights=True)
    learning_rate_scheduler = ReduceLROnPlateau(monitor='val_loss', patience=10, factor=0.5, min_lr=1e-6)

    model.fit(
        training_set,
        steps_per_epoch=len(training_set),
        epochs=1000,
        validation_data=validation_set,
        validation_steps=len(validation_set),
        callbacks=[early_stopping, learning_rate_scheduler]
    )

    model.save('inceptionV3_new.keras')
    print('Saved trained model as resnet.keras')


if __name__ == "__main__":
    main()
