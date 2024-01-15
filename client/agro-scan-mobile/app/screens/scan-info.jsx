import {View, Text, Image, StyleSheet, FlatList, SafeAreaView} from 'react-native'
import React, {useEffect, useState} from "react";
import {format} from "date-fns";
import { COLORS } from "@constants";
import {FONT, SIZES} from "../constants";
import {fetchRecommendations} from "../api/recommendations";

const ScanInfo = ({navigation, route }) => {
    const scan = route.params?.data || {};
    const [recommendations, setRecommendations] = useState([]);
    useEffect(() => {
        if(!scan?.diseaseUri) return;
        fetchRecommendations(scan.diseaseUri).then((res) => {
            setRecommendations(res.data);
        });
    }, []);

    const renderItem = ({ item }) => {
        return (
            <View style={styles.recommendations}>
                <View style={{width: '100%', flex: 1, flexDirection: 'row', justifyContent: 'space-between'}}>
                    <Text style={{fontSize: SIZES.xLarge, flex: 1}}>Product:</Text>
                    <Text style={{fontSize: SIZES.xLarge, flex: 1}}>{item.name}</Text>
                </View>
                <View style={{width: '100%', flexDirection: 'row', justifyContent: 'space-between'}}>
                    <Text style={{fontSize: SIZES.large, flex: 1}}>Manufacturer:</Text>
                    <Text style={{fontSize: SIZES.large, flex: 1}}>{item.manufacturer}</Text>
                </View>
                <View style={{width: '100%', flexDirection: 'row', justifyContent: 'space-between'}}>
                    <Text style={{fontSize: SIZES.large, flex: 1}}>Representative:</Text>
                    <Text style={{fontSize: SIZES.large, flex: 1}}>{item.representative}</Text>
                </View>
            </View>
    )};
    const renderNoRecommendations = () => {
        return (
            <View style={styles.noRecommendationsContainer}>
                <Text style={styles.noRecommendationsText}>No recommendations available.</Text>
            </View>
        );
    };
    return (
        <SafeAreaView style={{ width: '100%', justifyContent: 'center' }}>
            <FlatList
                style={{width: '100%'}}
                data={recommendations}
                ListEmptyComponent={renderNoRecommendations}
                ListHeaderComponent={() => (
                    <View style={styles.container}>
                        <View>
                            <Image style={styles.image} source={{uri: `data:image/jpeg;base64,${scan.imageBase64}`}}></Image>
                        </View>
                        <View style={{...styles.info, backgroundColor: scan.diseaseName ? COLORS.warning : COLORS.tertiary}}>
                            <View style={{alignItems: 'center', justifyContent: 'center'}}>
                                <Text style={{fontSize: SIZES.medium, color: scan.diseaseName ? COLORS.secondary : COLORS.primary}}>{format(scan.createdAt, "MMMM do, yyyy HH:mm")}</Text>
                                <Text style={{fontSize: SIZES.xxLarge, color: scan.diseaseName ? COLORS.secondary : COLORS.primary}}>{scan.diseaseName ? scan.diseaseName : 'Healthy'}</Text>
                                <Text style={{fontSize: SIZES.large, color: scan.diseaseName ? COLORS.secondary : COLORS.primary}}>{scan.plantName}</Text>
                            </View>
                        </View>
                        { scan?.diseaseUri && recommendations.length > 0 ?
                            (
                                <View style={styles.recommendationsContainer}>
                                    <Text style={{fontSize: SIZES.xLarge, alignContent: 'center', marginBottom: 0, marginTop: 5}}>Recommended agrochemicals:</Text>
                                </View>
                            ):
                            (
                                <></>
                            )
                        }
                    </View>
                )}
                renderItem={renderItem}
                keyExtractor={(item) => item.uri}
            >
            </FlatList>
        </SafeAreaView>
    )
}
const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    image: {
        width: '100%',
        aspectRatio: 1,
        resizeMode: "cover",
    },
    info: {
        alignItems: "center",
        justifyContent: "space-between",
        padding: 10,
        borderBottomWidth: 1,
        borderBottomColor: COLORS.primary,
    },
    date: {
        fontSize: SIZES.medium,
    },
    plantName: {
        fontSize: SIZES.xxLarge,
    },
    diseaseName: {
        fontSize: SIZES.large,
    },
    recommendationsContainer: {
        alignItems: 'center',
        padding: 10,
    },
    recommendationsTitle: {
        fontSize: SIZES.xLarge,
        marginBottom: 20,
        marginTop: 10,
    },
    recommendations: {
        alignItems: 'center',
        justifyContent: "space-between",
        alignSelf: 'center',
        borderTopWidth: 1,
        borderColor: COLORS.primary,
        padding: 10,
        width: '100%',
    },
    row: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        width: '100%',
    },
    label: {
        fontSize: SIZES.xLarge,
        flex: 1,
    },
    value: {
        fontSize: SIZES.xLarge,
        flex: 1,
    },
    noRecommendationsContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        padding: 20,
    },
    noRecommendationsText: {
        fontSize: SIZES.xLarge,
        color: COLORS.gray,
    },
});

export default ScanInfo;