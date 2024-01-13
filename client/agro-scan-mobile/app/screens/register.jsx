import {View, Text, TextInput, Button, TouchableOpacity, StyleSheet, Image} from 'react-native'
import {useState} from "react";
import { images } from "../constants";
import {useAuth} from "../context/AuthContext";
import Spinner from "react-native-loading-spinner-overlay";
import Toast from "react-native-toast-message";
import {COLORS} from "@constants";

const Register = ({navigation}) => {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')
    const [error, setError] = useState('');

    const [isLoading, setIsLoading] = useState(false)
    const { onLogin, onRegister } = useAuth();

    const validateForm = () => {
        let errors = []
        if (email === '') errors.push('Please enter your email!')
        if (password === '') errors.push('Please enter desired password!')
        if (confirmPassword === '') errors.push('Please confirm desired password!')
        if (password !== confirmPassword) errors.push('Passwords do not match!')
        return {errors}
    }
    const register = async () => {
        setIsLoading(true)
        const errors = validateForm().errors
        if(errors.length > 0){
            Toast.show({
                type: 'error',
                text1: 'Registration failed!',
                text2: errors.join('\n'),
            });
            setIsLoading(false)
            return;
        } else {
            const registerResponse = await onRegister(email, password);
            if(registerResponse && registerResponse.error){
                Toast.show({
                    type: 'error',
                    text1: 'Registration failed!',
                    text2: registerResponse.message,
                });
                setIsLoading(false)
                return;
            }
            const loginResponse = await onLogin(email, password);
            if(loginResponse && loginResponse.error){
                Toast.show({
                    type: 'error',
                    text1: 'Login failed!',
                    text2: loginResponse.message,
                });
            }
        }
        setIsLoading(false)
    }

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
                <Button color={COLORS.primary} onPress={register} title="Register" />
                <View style={styles.already_registered}>
                    <Text>Already have an account? </Text>
                    <TouchableOpacity onPress={() => { navigation.navigate('Login') }}>
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