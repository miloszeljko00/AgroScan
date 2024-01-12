import Toast from "react-native-toast-message";
import {AuthProvider, useAuth} from "./app/context/AuthContext";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import React from "react";
import {NavigationContainer} from "@react-navigation/native";
import Login from "./app/screens/login";
import Home from "./app/screens/home";
import {Button} from "react-native";
import Register from "./app/screens/register";

const Stack = createNativeStackNavigator();

export default function App() {
  return (
      <>
          <AuthProvider>
              <Layout></Layout>
          </AuthProvider>
        <Toast />
      </>
  );
}

export const Layout = () => {
    const { authState, onLogout } = useAuth();
    return (
        <NavigationContainer>
            <Stack.Navigator>
                {
                    authState?.authenticated ?
                        <>
                            <Stack.Screen name="Home" component={Home} options={{
                                headerRight: () => <Button onPress={onLogout} title="Sign Out"/>
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