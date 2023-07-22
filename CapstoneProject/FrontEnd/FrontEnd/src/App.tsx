import './App.css';
import { BrowserRouter as Router } from "react-router-dom";
import Login from './pages/Login';
import Orders from './pages/Orders';
import Settings from './pages/Settings';
import LiveLogs from './pages/LiveLogs.tsx';
import SocialLogin from "./pages/SocialLogin.tsx";
import { useEffect, useState } from "react";
import { useSignalRService } from "./context/SignalRContext.tsx";
import { Provider } from 'react-redux';
import store from './store';
import ProtectedRoute from "./components/ProtectedRoute.tsx";
import { AppUserContext } from "./context/StateContext.tsx";
import { LocalUser } from "./types/AuthTypes.ts";
import { useNavigate, Routes, Route } from "react-router-dom";

function AuthHandler() {
    const navigate = useNavigate();

    useEffect(() => {
        const jwtJson = localStorage.getItem("softwarehouse_user");
        if (!jwtJson) {
            navigate("/");
        }
    }, [navigate]);

    useEffect(() => {
        const protectedRoutes = ['/orders', '/settings', '/livelogs'];

        if (!appUser && protectedRoutes.includes(location.pathname)) {
            redirectToLogin();
        }
    }, [location.pathname, appUser]);

    return (
        <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/social-login" element={<SocialLogin />} />
            <Route path="/orders" element={<ProtectedRoute><Orders /></ProtectedRoute>} />
            <Route path="/settings" element={<ProtectedRoute><Settings /></ProtectedRoute>} />
            <Route path="/livelogs" element={<ProtectedRoute><LiveLogs /></ProtectedRoute>} />
        </Routes>
    );
}

function App() {
    const { startConnection } = useSignalRService();

    const [appUser, setAppUser] = useState<LocalUser | undefined>(undefined);

    useEffect(() => {
        startConnection().catch(error => {
            console.error("SignalR bağlantısı başlatılamadı:", error);
        });
    }, [startConnection]);

    return (
        <AppUserContext.Provider value={{ appUser, setAppUser }}>
            <Provider store={store}>
                <Router>
                    <AuthHandler />
                </Router>
            </Provider>
        </AppUserContext.Provider>
    )
}

export default App;
