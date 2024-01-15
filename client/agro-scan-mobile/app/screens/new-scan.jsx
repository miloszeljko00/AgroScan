import {Image, Text, StyleSheet, SafeAreaView, TouchableOpacity, Button, StatusBar, View} from 'react-native'
import Welcome from "../components/home/welcome/Welcome";
import ScanList from "../components/home/scan-list/scan-list";
import React, {useEffect, useRef,useState} from "react";
import {COLORS, icons} from "@constants";
import { Camera } from 'expo-camera';
import { shareAsync } from 'expo-sharing';
import * as MediaLibrary from 'expo-media-library';
import * as ImagePicker from 'expo-image-picker';
import * as ImageManipulator from 'expo-image-manipulator';
import { createScan } from "../api/scans";
import {AxiosError} from "axios";

const NewScan = ({navigation}) => {
    const [scan, setScans] = useState({
        id: "",
        plant: "Tomato",
        label: "",
        date: new Date(),
        image: "",
    },);
    let cameraRef = useRef();
    const [hasCameraPermission, setHasCameraPermission] = useState(false);
    const [hasMediaLibraryPermission, setHasMediaLibraryPermission] = useState(false);
    const [photo, setPhoto] = useState();
    const [flashMode, setFlashMode] = useState(false);

    useEffect(() => {
        (async () => {
            try {
                const cameraPermission = await Camera.requestCameraPermissionsAsync();
                const mediaLibraryPermission = await MediaLibrary.requestPermissionsAsync();
                if (cameraPermission && cameraPermission.status === "granted") {
                    setHasCameraPermission(true);
                } else {
                    setHasCameraPermission(false);
                }
                if (mediaLibraryPermission && mediaLibraryPermission.status === "granted") {
                    setHasMediaLibraryPermission(true);
                } else {
                    setHasMediaLibraryPermission(false);
                }
            } catch (error) {
                console.error("Error requesting permissions:", error);
            }
        })();
    }, []);


    if (!hasCameraPermission) {
        return <Text>Permission for camera not granted. Please change this in settings.</Text>
    }
    let openGallery = async () => {
        try {
            const result = await ImagePicker.launchImageLibraryAsync({
                mediaTypes: ImagePicker.MediaTypeOptions.Images,
                allowsEditing: true,
                aspect: [1, 1],
                quality: 1,
                base64: true,
            });

            if (!result.canceled) {
                const resizedImage = await ImageManipulator.manipulateAsync(
                    result.assets[0].uri,
                    [],
                    { base64: true }
                );
                setPhoto(resizedImage);
            }
        } catch (error) {
            console.error('Error opening media library:', error);
        }
    }
    let toggleFlash = () => {
        setFlashMode(!flashMode);
    };
    let takePic = async () => {
        let options = {
            quality: 0.5,
            base64: true,
            exif: false,
            aspect: [1, 1],
            skipProcessing: true,
        };
        cameraRef.current.takePictureAsync(options).then((newPhoto)=>{
            setPhoto(newPhoto);
        });
    };

    if (photo) {
        let cancel = () => {
            setPhoto(undefined);
        };

        let confirm = async () => {
            try {
                if (photo.base64) {
                    const resizedImage = await ImageManipulator.manipulateAsync(
                        `data:image/jpg;base64,${photo.base64}`,
                        [{ resize: { width: 256, height: 256 } }],
                        { base64: true }
                    );
                    createScan(resizedImage.base64).then((response) => {
                        navigation.navigate('Home');
                    }).catch((error) => {
                        console.error(error);
                    });
                }
            } catch (error) {
                console.error('Error saving photo:', error);
            }
        };

        return (
            <SafeAreaView style={styles.containerAfterImage}>
                <Image style={styles.preview} source={{ uri: "data:image/jpg;base64," + photo.base64 }} />
                <View style={styles.commandsContainer}>
                    <TouchableOpacity style={styles.cancelContainer} onPress={cancel} >
                        <Image style={styles.cancel} source={icons.cancel}/>
                    </TouchableOpacity>
                    <TouchableOpacity style={styles.confirmContainer} onPress={confirm} >
                        <Image style={styles.confirm} source={icons.confirm}/>
                    </TouchableOpacity>
                </View>
            </SafeAreaView>
        );
    }

    return (
        <View style={{
            justifyContent: 'space-between',
            height:'100%',
            paddingBottom: 50,
        }}>
            <View style={{width: '100%', aspectRatio: 1,}}>
                <Camera style={styles.container} ref={cameraRef} ratio="1:1" flashMode={flashMode? Camera.Constants.FlashMode.torch : Camera.Constants.FlashMode.off}>
                    <StatusBar style="auto" />
                </Camera>
            </View>
            <View style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
                paddingHorizontal: 15
            }}>
                <TouchableOpacity style={styles.buttonContainer} onPress={openGallery}>
                    <Image style={styles.cameraIcon} source={icons.gallery} />
                </TouchableOpacity>

                <TouchableOpacity style={styles.buttonContainer} onPress={takePic}>
                    <Image style={styles.cameraIcon} source={icons.camera} />
                </TouchableOpacity>

                <TouchableOpacity style={styles.buttonContainer} onPress={toggleFlash}>
                    <Image style={styles.cameraIcon} source={icons.flash} />
                </TouchableOpacity>
            </View>


        </View>

    );
}

const styles = StyleSheet.create({
    container: {
        aspectRatio: 1,
        flexDirection: 'column',
        flex: 1,
        alignItems: 'center',
        justifyContent: 'flex-end',
        paddingBottom: 50,
    },
    cameraIcon: {
        width: 60,
        height: 60,
        resizeMode: 'contain',
    },
    containerAfterImage: {
        flexDirection: 'column',
        flex: 1,
        alignItems: 'center',
        justifyContent: 'space-between',
    },
    buttonContainer: {
        width: 80,
        flexDirection: 'column',
        backgroundColor: COLORS.primary,
        justifyContent: 'center',
        alignItems: 'center',
        verticalAlign: 'middle',
        borderRadius: 100,
        padding: 10,
    },
    buttonText: {
        fontSize: 30,
        justifyContent: 'center',
        color: COLORS.primary,
    },
    preview: {
        width: '100%',
        aspectRatio: 1,
        alignSelf: 'center',
    },
    cancel: {
        width: 100,
        height: 100,
        resizeMode: 'contain',
    },
    confirm: {
        width: 100,
        height: 100,
        resizeMode: 'contain',
    },
    cancelContainer: {
        flex: 1,
        backgroundColor: COLORS.warning,
        justifyContent: 'center',
        alignItems: 'center',
        verticalAlign: 'middle',
        paddingTop: 25,
        paddingBottom: 25,
        width: "50%",
    },
    confirmContainer: {
        flex: 1,
        backgroundColor: COLORS.tertiary,
        justifyContent: 'center',
        alignItems: 'center',
        verticalAlign: 'middle',
        paddingTop: 25,
        paddingBottom: 25,
        width: "50%",
    },
    commandsContainer: {
        height: 150,
        flexDirection: 'row',
        justifyContent: 'space-between',
        verticalAlign: 'middle',
        alignItems: 'center',
    }
});

export default NewScan;