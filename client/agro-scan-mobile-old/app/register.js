import {View, Text, TextInput, Button, TouchableOpacity, StyleSheet, Image} from 'react-native'
import {useContext, useState} from "react";
import { router } from 'expo-router';
import { images } from "../constants";
import {AuthContext, useAuth} from "../context/AuthContext";
import Spinner from "react-native-loading-spinner-overlay";
import Toast from "react-native-root-toast";

const Register = () => {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')
    const [isLoading, setIsLoading] = useState(false)
    const { onLogin, onRegister } = useAuth();
    return (
        <View style={styles.container}>
            <Spinner visible={isLoading} />
            <View style={styles.wrapper} >
                <Image source={images.logo} style={styles.logo} />
                <TextInput
                    value={email}
                    onChangeText={text => setEmail(text)}
                    style={styles.input}
                    placeholder="Email"
                />
                <TextInput
                    value={password}
                    onChangeText={text => setPassword(text)}
                    style={styles.input}
                    placeholder="Password"
                    secureTextEntry
                />
                <TextInput
                    value={confirmPassword}
                    onChangeText={text => setConfirmPassword(text)}
                    style={styles.input}
                    placeholder="Confirm password"
                    secureTextEntry
                />
                <Button onPress={ async () => {
                    setIsLoading(true)
                    if (email === '') {
                        setIsLoading(false)
                        Toast.show('Please enter your email!', {
                            duration: Toast.durations.LONG,
                        });
                        return;
                    }
                    if (password === '') {
                        setIsLoading(false)
                        Toast.show('Please enter desired password!', {
                            duration: Toast.durations.LONG,
                        });
                        return;
                    }
                    if (confirmPassword === '') {
                        setIsLoading(false)
                        Toast.show('Please confirm desired password!', {
                            duration: Toast.durations.LONG,
                        });
                        return;
                    }
                    if (password !== confirmPassword) {
                        setIsLoading(false)
                        Toast.show('Passwords do not match!', {
                            duration: Toast.durations.LONG,
                        });
                        return;
                    }
                    const response = await onRegister(email, password);
                    if(response && response.error){
                        alert(result.msg);
                        Toast.show(result.msg, {
                            duration: Toast.durations.LONG,
                        });
                    }else {
                        await onLogin(email, password);
                    }
                    setIsLoading(false)
                    console.log('registering');
                }} title="Register" />
                <View style={styles.already_registered}>
                    <Text>Already have an account? </Text>
                    <TouchableOpacity onPress={() => { router.push('/login') }}>
                        <Text style={styles.link}>Login</Text>
                    </TouchableOpacity>
                </View>
            </View>
        </View>
    )
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    wrapper:{
        width: '80%'
    },
    input:{
        marginBottom: 10,
        paddingTop: 12,
        paddingBottom: 12,
        borderWidth: 1,
        borderColor: '#bbb',
        borderRadius: 5,
        paddingHorizontal: 14,
    },
    link:{
        color: 'blue'
    },
    already_registered:{
        flexDirection: 'row',
        marginTop: 20,
        alignItems: 'center',
        justifyContent: 'center'
    },
    logo:{
        width: 200, height: 200, alignSelf: 'center', marginBottom: 40, marginTop: -100
    }
})

export default Register;