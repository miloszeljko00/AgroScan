import {
    View,
    Text,
    Image, TouchableOpacity
} from "react-native";
import { format } from "date-fns";
import styles from "./scan.style";
import {icons} from "@constants";

const Scan = ({scan}) => {
    return (
        <View>
            <View style={styles.container}>
                <View>
                    <Image style={styles.image} source={{uri: scan.image}}></Image>
                </View>
                <View style={styles.info}>
                    <View>
                        <Text>{scan.plant} - {scan.label}</Text>
                        <Text>{format(scan.date, "MMMM do, yyyy HH:mm")}</Text>
                    </View>
                    <TouchableOpacity style={styles.moreInfo}>
                        <Image style={styles.moreInfoBtn} source={icons.chevronRight} />
                    </TouchableOpacity>
                </View>

            </View>
        </View>
    );
};

export default Scan;
