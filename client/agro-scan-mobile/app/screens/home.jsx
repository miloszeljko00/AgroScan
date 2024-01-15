import {View, Text, StyleSheet, SafeAreaView, TouchableOpacity} from 'react-native'
import Welcome from "../components/home/welcome/Welcome";
import ScanList from "../components/home/scan-list/scan-list";
import React, {useCallback, useEffect, useState} from "react";
import {COLORS} from "@constants";
import { fetchScans } from '../api/scans';
import Toast from "react-native-toast-message";

const Home = ({navigation}) => {
    const [scans, setScans] = useState([]);
    const fetchScansData = useCallback(() => {
        fetchScans()
            .then((response) => {
                setScans(response.data);
            })
            .catch((error) => {
                Toast.show({
                    type: "error",
                    text1: "Error",
                    text2: "Could not fetch scans",
                });
            });
    }, []);

    useEffect(() => {
        fetchScansData();
    }, [fetchScansData]);

    useEffect(() => {
        const unsubscribeFocus = navigation.addListener("focus", () => {
            // Fetch scans when the screen comes into focus
            fetchScansData();
        });

        return unsubscribeFocus;
    }, [navigation, fetchScansData]);
    const openNewScanScreen = () => {
       navigation.push('NewScan')
    }
    let onScanSelected = (scan) => {
        navigation.push('ScanInfo', {data: scan})
    }
    return (
        <SafeAreaView style={styles.container}>
            <View style={styles.welcome}>
                <Welcome></Welcome>
            </View>
            <View style={styles.scanList}>
                <ScanList onSelected={onScanSelected} scans={scans}></ScanList>
            </View>
            <TouchableOpacity onPress={openNewScanScreen} style={styles.newScan}>
                <Text style={styles.newScanText}>New Scan</Text>
            </TouchableOpacity>
        </SafeAreaView>
    )
}

const styles = StyleSheet.create({
    container: {
        width: '100%',
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    welcome: {
        width: '100%',
        height: '16%',
        paddingHorizontal: 20,
        paddingTop: 30,
        borderBottomWidth: 1,
        borderColor: 'gray',
        flexWrap: 'nowrap'
    },
    scanList: {
        width: '95%',
        height: '72%'
    },
    newScan: {
        textAlign: 'center',
        justifyContent: 'center',
        width: '100%',
        height: '12%',
        backgroundColor: COLORS.tertiary,
    },
    newScanText: {
        fontSize: 20,
        color: COLORS.primary,
        fontWeight: 'bold',
        textAlign: 'center',
        justifyContent: 'center',
    }
})

export default Home;