import {
  View,
  Text,
} from "react-native";

import styles from "./welcome.style";
import {useAuth} from "../../../context/AuthContext";

const Welcome = () => {
    const { authState } = useAuth();
    return (
    <View>
      <View style={styles.container}>
        <Text style={styles.userName}>Hello {authState.email}!</Text>
        <Text style={styles.welcomeMessage}>Scan, Detect, Treat.</Text>
      </View>
    </View>
  );
};

export default Welcome;
