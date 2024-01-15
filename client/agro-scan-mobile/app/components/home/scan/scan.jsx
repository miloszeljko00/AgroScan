import {
    View,
    Text,
    Image, TouchableOpacity
} from "react-native";
import { format } from "date-fns";
import styles from "./scan.style";
import {icons} from "@constants";

const Scan = ({scan, onClicked}) => {
    let openScanInfo = () => {
        onClicked(scan);
    }
    return (
        <View>
            <View style={styles.container}>
                <View>
                    <Image style={styles.image} source={{uri: `data:image/jpeg;base64,${scan.imageBase64}`}}></Image>
                </View>
                <View style={styles.info}>
                    <View>
                        <Text>{scan.plantName} - {scan.diseaseName ? scan.diseaseName : 'Healthy'}</Text>
                        <Text>{format(scan.createdAt, "MMMM do, yyyy HH:mm")}</Text>
                    </View>
                    <TouchableOpacity style={styles.moreInfo} onPress={openScanInfo}>
                        <Image style={styles.moreInfoBtn} source={icons.chevronRight} />
                    </TouchableOpacity>
                </View>

            </View>
        </View>
    );
};

export default Scan;
