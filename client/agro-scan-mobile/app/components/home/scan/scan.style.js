import { StyleSheet } from "react-native";
import { COLORS } from "@constants";

const styles = StyleSheet.create({
    container: {
        flexDirection: "row",
        width: "100%",
        borderWidth: 1,
        borderColor: COLORS.primary,
        marginBottom: 10,
        padding: 10
    },
    image: {
        width: 80,
        height: 80,
        resizeMode: "cover",
    },
    info: {
        flexDirection: "row",
        verticalAlign: "center",
        alignItems: "center",
        justifyContent: "space-between",
        flex: 1,
        marginLeft: 10,
    },
    moreInfo: {
        flexDirection: "row",
        verticalAlign: "center",
        justifyContent: "flex-end",
        alignItems: "center",
        marginTop: 10,
    },
    moreInfoBtn: {
        width: 38,
        height: 38,
        justifyContent: 'center',
        alignItems: 'center',
        borderRadius: 100
    }
});

export default styles;
