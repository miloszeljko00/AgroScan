import {View, Text, StyleSheet, SafeAreaView, TouchableOpacity} from 'react-native'
import Welcome from "../components/home/welcome/Welcome";
import ScanList from "../components/home/scan-list/scan-list";
import React, {useState} from "react";
import {COLORS} from "@constants";
const Home = ({navigation}) => {
    const [scans, setScans] = useState([
        {
            id: "1",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2023-09-01T15:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "2",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "3",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "4",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "5",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "6",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "7",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "8",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "9",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        },
        {
            id: "10",
            plant: "Tomato",
            label: "Bacterial Spot",
            date: new Date("2021-09-01T10:43:00"),
            image: "https://www.shutterstock.com/image-photo/tomato-leaves-isolated-on-white-260nw-1251320371.jpg",
        }
    ]);

    const openNewScanScreen = () => {
       navigation.push('NewScan')
    }
    return (
        <SafeAreaView style={styles.container}>
            <View style={styles.welcome}>
                <Welcome></Welcome>
            </View>
            <View style={styles.scanList}>
                <ScanList scans={scans}></ScanList>
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