import { Stack } from "expo-router";
import { useFonts } from "expo-font";
import { AuthProvider } from "../context/AuthContext";
import { RootSiblingParent } from 'react-native-root-siblings';

export const unstable_settings = {
  initialRouteName: "home",
};

const Layout = () => {
  const [fontsLoaded] = useFonts({
    DMBold: require("../assets/fonts/DMSans-Bold.ttf"),
    DMMedium: require("../assets/fonts/DMSans-Medium.ttf"),
    DMRegular: require("../assets/fonts/DMSans-Regular.ttf"),
  });

  if (!fontsLoaded) {
    return null;
  }

  return (
      <RootSiblingParent>
          <AuthProvider>
            <Stack initialRouteName="home">
              <Stack.Screen name="home" />
              <Stack.Screen name="login" options={{ headerShown: false}} />
              <Stack.Screen name="register" options={{ headerShown: false}} />
            </Stack>
          </AuthProvider>
      </RootSiblingParent>
  )
};

export default Layout;
