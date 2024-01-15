import React, {createContext, useState, useEffect, useContext} from "react";
import axios from "axios"
import * as SecureStorage from "expo-secure-store";

const AUTH_STATE = 'auth_state';
const API_URL = process.env.EXPO_PUBLIC_AGRO_SCAN_API_BASE_URL + '/users';
interface AuthProps {
    authState?: { email: string|null; access_token: string|null; refresh_token: string|null; authenticated: boolean|null };
    onRegister?: (email: string, password: string) => Promise<any>;
    onLogin?: (email: string, password: string) => Promise<any>;
    onLogout?: () => Promise<any>;
}
export const AuthContext = createContext<AuthProps>({});

export const useAuth = () => {
    return useContext(AuthContext);
}

export const AuthProvider = ({children}: any) => {
    const [authState, setAuthState] = useState<
        {
            email: string|null;
            access_token: string|null;
            refresh_token: string|null;
            authenticated: boolean|null
        }>({
            email: null,
            access_token: null,
            refresh_token: null,
            authenticated: null
        });

    useEffect(() => {
        const loadAuthState = async () => {
            const authState = await SecureStorage.getItemAsync(AUTH_STATE);
            if (authState) {
                setAuthState(JSON.parse(authState));
                axios.defaults.headers.common['Authorization'] = `Bearer ${JSON.parse(authState).access_token}`;
            }
        }
        loadAuthState().then();
    }, []);
    const refreshToken = async () => {
        const refreshToken = authState.refresh_token;
        if(!refreshToken) {
            await logout();
            return {error: true, message: 'Session expired!'}
        }
        try{
            const result = await axios.post(`${API_URL}/refresh`,{refreshToken:refreshToken});
            setAuthState({
                ...authState,
                access_token: result.data.accessToken,
                refresh_token: result.data.refreshToken,
                authenticated: true
            });
            axios.defaults.headers.common['Authorization'] = `Bearer ${result.data.accessToken}`;
            await SecureStorage.setItemAsync(AUTH_STATE, JSON.stringify(authState));
        }catch (e) {
            await logout();
            return {error: true, message: 'Session expired!'};
        }
    }
    axios.interceptors.response.use(
        (response) => {
            return response;
        },
        async (error) => {
            const originalRequest = error.config;
            if (error && error.response && error.response.status === 401 && !originalRequest._retry) {
                originalRequest._retry = true;
                const refreshResult = await refreshToken();
                if (refreshResult && refreshResult.error) {
                    console.error('Refresh token failed:', refreshResult.message);
                } else {
                    originalRequest.headers['Authorization'] = `Bearer ${authState.access_token}`;
                    return axios(originalRequest);
                }
            }
            return Promise.reject(error);
        }
    );
    const register = async (email: string, password: string) => {
        try{
            return await axios.post(`${API_URL}/register`,{email, password})
        }catch (e) {
            const message = Object.values((e as any).response?.data?.errors).toString();
            return {error: true, message: message};
        }
    }

    const login = async (email: string, password: string) => {
        try{
            const result= await axios.post(`${API_URL}/login`,{email: email, password: password});
            const authState = {
                email: email,
                access_token: result.data.accessToken,
                refresh_token: result.data.refreshToken,
                authenticated: true
            }
            setAuthState(authState);
            axios.defaults.headers.common['Authorization'] = `Bearer ${result.data.accessToken}`;
            await SecureStorage.setItemAsync(AUTH_STATE, JSON.stringify(authState));
        }catch (e) {
            return {error: true, message: 'Invalid credentials'};
        }
    }

    const logout = async  () => {
        await SecureStorage.deleteItemAsync(AUTH_STATE);
        axios.defaults.headers.common['Authorization'] = ``;
        setAuthState({
            email: null,
            access_token: null,
            refresh_token: null,
            authenticated: false
        });
    }

    const value: AuthProps = {
        onRegister: register,
        onLogin: login,
        onLogout: logout,
        authState
    };
    return (
        <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
    );
};