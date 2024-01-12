import {View, Text, TextInput, Button, TouchableOpacity, StyleSheet, Image} from 'react-native'
import {useContext, useState} from "react";
import { router } from 'expo-router';
import { images } from "../constants";
import {AuthContext, useAuth} from "../context/AuthContext";
import Spinner from "react-native-loading-spinner-overlay";
const Login = () => {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [isLoading, setIsLoading] = useState(false)

    const { onLogin } = useAuth();

    return (
        <View style={styles.container}>
            <Spinner visible={isLoading} />
            <View style={styles.wrapper}>
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
                <Button title="Login" onPress={ async () =>{
                    await onLogin(email, password)
                }}/>
                <View style={styles.no_account}>
                    <Text>Don't have an account? </Text>
                    <TouchableOpacity onPress={() => { router.push('/register') }}>
                        <Text style={styles.link}>Register</Text>
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
    no_account:{
        flexDirection: 'row',
        marginTop: 20,
        alignItems: 'center',
        justifyContent: 'center'
    },
    logo:{
        width: 200, height: 200, alignSelf: 'center', marginBottom: 40, marginTop: -100
    }
})

export default Login;