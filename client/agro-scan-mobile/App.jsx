import Toast from "react-native-toast-message";
import {AuthProvider, useAuth} from "./app/context/AuthContext";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import React from "react";
import {NavigationContainer} from "@react-navigation/native";
import Login from "./app/screens/login";
import Home from "./app/screens/home";
import {Button} from "react-native";
import Register from "./app/screens/register";
import {useFonts} from "expo-font";
import {COLORS} from "@constants";
import NewScan from "./app/screens/new-scan";

const Stack = createNativeStackNavigator();

export default function App() {
    const [fontsLoaded] = useFonts({
        DMBold: require("./assets/fonts/DMSans-Bold.ttf"),
        DMMedium: require("./assets/fonts/DMSans-Medium.ttf"),
        DMRegular: require("./assets/fonts/DMSans-Regular.ttf"),
    });

    if (!fontsLoaded) {
        return null;
    }

    return (
        <>
            <AuthProvider>
              <Layout></Layout>
            </AuthProvider>
            <Toast />
        </>
    );
}

export const Layout = ({navigation}) => {
    const { authState, onLogout } = useAuth();

    const goBack = () => {
        navigation.goBack();
    }

    return (
        <NavigationContainer>
            <Stack.Navigator>
                {
                    authState?.authenticated ?
                        <>
                            <Stack.Screen name="Home" component={Home} options={{
                                title: 'AgroScan',
                                headerRight: () => <Button color={COLORS.primary} onPress={onLogout} title="Sign Out"/>
                            }}/>
                            <Stack.Screen name="NewScan" component={NewScan} options={{
                                title: 'AgroScan'
                            }}/>
                        </>
                        :
                        <>
                            <Stack.Screen name="Login" component={Login} options={{headerShown:false}} />
                            <Stack.Screen name="Register" component={Register} options={{headerShown:false}} />
                        </>
                }
            </Stack.Navigator>
        </NavigationContainer>
    )
}