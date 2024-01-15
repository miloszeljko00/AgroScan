import axios from 'axios';

const AGRO_SCAN_API_BASE_URL = process.env.EXPO_PUBLIC_AGRO_SCAN_API_BASE_URL;

export const fetchRecommendations = (diseaseUri) => {
    return axios.post(`${AGRO_SCAN_API_BASE_URL}/agrochemicals/get-recommendation`, { diseaseUri });
};
