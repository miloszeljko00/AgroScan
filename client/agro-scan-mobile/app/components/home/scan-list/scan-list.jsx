import {
    View,
    Text, FlatList,
} from "react-native";

import {useState} from "react";
import Scan from "../scan/scan";
import styles from "./scan-list.style";

const ScanList = ({scans, onSelected}) => {
    const renderItem = ({ item }) => {
        return <Scan scan={item} onClicked={onRowClicked}/>;
    };
    const onRowClicked = (scan) => {
        onSelected(scan);
    }
    return (
        <View>
            <FlatList
                style={styles.container}
                data={scans}
                renderItem={renderItem}
                keyExtractor={(scan) => scan.id}
            />
        </View>
    );
};

export default ScanList;
