#https://www.codetrade.io/blog/grad-cam-a-complete-guide-with-example/
import cv2
import tensorflow as tf
import matplotlib.pyplot as plt
from keras.models import load_model
import PIL
import numpy as np


def get_processed_img(path):
    # Open the image using the local path
    img_from_path = PIL.Image.open(path)
    # Adjust the image dimensions to a standard size.
    new_image = img_from_path.resize((224, 224))
    # Transform the image into a NumPy array.
    processed_image = np.asarray(new_image)
    # Normalize pixel values to the range [0, 1]
    processed_image = processed_image / 255.0
    # Add a batch dimension
    image = np.expand_dims(processed_image, axis=0)
    return image


def make_gradcam_heatmap(img_array, model, last_conv_layer_name, pred_index=None):
    # Create a sub-model that outputs the feature maps and final prediction
    grad_model = tf.keras.models.Model(
        [model.inputs], [model.get_layer(last_conv_layer_name).output, model.output]
    )
    # Use GradientTape to record gradients
    with tf.GradientTape() as tape:
        last_conv_layer_output, preds = grad_model(img_array)
        # If pred_index is not specified, use the predicted class index
        if pred_index is None:
            pred_index = tf.argmax(preds[0])
        class_channel = preds[:, pred_index]

    # Calculate gradients
    grads = tape.gradient(class_channel, last_conv_layer_output)
    pooled_grads = tf.reduce_mean(grads, axis=(0, 1, 2))

    # Compute the heatmap
    last_conv_layer_output = last_conv_layer_output[0]
    heatmap = last_conv_layer_output @ pooled_grads[..., tf.newaxis]
    heatmap = tf.squeeze(heatmap)

    # Normalise the heatmap
    heatmap = tf.maximum(heatmap, 0) / tf.math.reduce_max(heatmap)

    return heatmap.numpy()


def display_gradcam(img, heatmap, alpha=0.5):
    # Resize the heatmap to match the original image size
    heatmap = cv2.resize(heatmap, (img.shape[1], img.shape[0]))  # Fix the order of dimensions

    # Normalize and convert to 8-bit unsigned integer
    img = (img * 255).astype(np.uint8)
    img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    colored_heatmap = plt.cm.jet(heatmap)
    # Now colored_heatmap has dimensions (224, 224, 4) with RGBA channels
    # You can remove the alpha channel if needed
    colored_heatmap = colored_heatmap[:, :, :3]
    # Normalize and convert to 8-bit unsigned integer
    colored_heatmap = (colored_heatmap * 255).astype(np.uint8)

    # Apply the heatmap to the original image
    blended = cv2.addWeighted(img, 1-alpha, colored_heatmap, alpha, 0)

    # Create a figure with two subplots
    fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(12, 5))

    # Display the original image on the left subplot
    ax1.imshow(img)
    ax1.axis('off')
    ax1.set_title('Original')

    ax2.imshow(blended)
    ax2.axis('off')
    ax2.set_title('Grad-CAM')

    # Show the figure
    plt.show()


def main():
    # Your existing main function remains unchanged

    # After training, load the saved model
    print("Loading model...")
    model = load_model('resnet.keras')
    print("Model loaded...")
    model.summary()
    # Visualize Grad-CAM for a specific image in the validation set
    img_path = '../../dataset/lab_train/Tomato___Bacterial_spot/0ed6d8f5-dd9f-4f1d-a33d-2f556be12a27___UF.GRC_BS_Lab Leaf 0291.JPG'  # Replace with the actual path
    image = get_processed_img(img_path)
    grad_cam_heatmap = make_gradcam_heatmap(image, model, "conv3_block8_2_conv", pred_index=0)
    display_gradcam(image[0], grad_cam_heatmap)


if __name__ == "__main__":
    main()
