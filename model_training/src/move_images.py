import os
import random
import shutil

def move_random_images(source_dir, destination_dir, num_images):
    # Get a list of all files in the source directory
    all_files = os.listdir(source_dir)

    # Randomly select 'num_images' files
    selected_files = random.sample(all_files, num_images)

    # Create the destination directory if it doesn't exist
    os.makedirs(destination_dir, exist_ok=True)

    # Move selected files to the destination directory
    for file_name in selected_files:
        source_path = os.path.join(source_dir, file_name)
        destination_path = os.path.join(destination_dir, file_name)
        shutil.move(source_path, destination_path)


# Example usage
source_directory = '../dataset/train/Tomato___Tomato_Yellow_Leaf_Curl_Virus'
destination_directory = '../dataset/test/Tomato___Tomato_Yellow_Leaf_Curl_Virus'
num_images_to_move = 100

move_random_images(source_directory, destination_directory, num_images_to_move)
