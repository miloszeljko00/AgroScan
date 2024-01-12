import {View, Text, StyleSheet, Image} from 'react-native'
import { images } from "../constants";
const Home = () => {
    return (
        <View style={styles.container}>
            <View style={styles.wrapper}>
                <Image source={images.logo} style={styles.logo} />
                <Text>HOME SCREEN</Text>
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
    logo:{
        width: 200, height: 200, alignSelf: 'center', marginBottom: 40, marginTop: -100
    }
})

export default Home;