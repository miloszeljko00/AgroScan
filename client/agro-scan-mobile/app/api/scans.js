import axios from 'axios';

const AGRO_SCAN_API_BASE_URL = process.env.EXPO_PUBLIC_AGRO_SCAN_API_BASE_URL;
export const fetchScans = () => {
    return axios.get(`${AGRO_SCAN_API_BASE_URL}/scans`);
};


export const createScan = async (imageBase64) => {

    const formData = new FormData();
    const image = {
        uri: `data:image/jpg;base64,${imageBase64}`,
        name: 'image.jpg',
        type: 'image/jpeg'
    }
    formData.append('image', image);
    return axios.post(`${AGRO_SCAN_API_BASE_URL}/scans/create`, formData, { headers: { 'Content-Type': 'multipart/form-data' } });
};
