import React, {createContext, useState, useEffect, useContext} from "react";
import axios from "axios"
import * as SecureStorage from "expo-secure-store";
import Toast from "react-native-root-toast";


interface AuthProps {
    authState?: { email: string|null; access_token: string|null; refresh_token: string|null; authenticated: boolean|null };
    onRegister?: (email: string, password: string) => Promise<any>;
    onLogin?: (email: string, password: string) => Promise<any>;
    onLogout?: () => Promise<any>;
}
const AUTH_STATE = 'auth_state';
const API_URL = process.env.AGRO_SCAN_API_BASE_URL + '/users';
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
    //loadAuthState().then();
}, []);

    const register = async (email: string, password: string) => {
        try{
            return await axios.post(`${API_URL}/register`,{email, password})
        }catch (e) {
            console.log(e)
            Toast.show(e, {
                duration: Toast.durations.LONG,
            });
            return {error: e.true, msg: e};
        }
    }

    const login = async  (email: string, password: string) => {
        try{
            const result = await axios.post(`${API_URL}/login`,{email, password});
            const authState = {
                email: email,
                access_token: result.data.access_token,
                refresh_token: result.data.refresh_token,
                authenticated: true
            }
            setAuthState(authState);
            axios.defaults.headers.common['Authorization'] = `Bearer ${result.data.access_token}`;
            await SecureStorage.setItemAsync(AUTH_STATE, JSON.stringify(authState));
        }catch (e) {
            console.log(e)
            Toast.show(e, {
                duration: Toast.durations.LONG,
            });
            return {error: e.true, msg: e};
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