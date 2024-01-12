import {View, Text, TextInput, Button, TouchableOpacity, StyleSheet, Image} from 'react-native'
import {useState} from "react";
import { images } from "../constants";
import {useAuth} from "../context/AuthContext";
import Spinner from "react-native-loading-spinner-overlay";
import Toast from "react-native-toast-message";

const Login = ({navigation }: any) => {
    const [email, setEmail] = useState('miloszeljko00@gmail.com')
    const [password, setPassword] = useState('Abc.123456')
    const [isLoading, setIsLoading] = useState(false)

    const { onLogin } = useAuth();

    const login = async () => {
        setIsLoading(true)
        const response = await onLogin!(email, password);
        if(response && response.error){
            Toast.show({
                type: 'error',
                text1: 'Login failed!',
                text2: response.message,
            });
        }
        setIsLoading(false)
    }

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
                <Button title="Login" onPress={login}/>
                <View style={styles.no_account}>
                    <Text>Don't have an account? </Text>
                    <TouchableOpacity onPress={() => { navigation.navigate('Register')}}>
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