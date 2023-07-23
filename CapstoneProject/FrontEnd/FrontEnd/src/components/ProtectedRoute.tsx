import React, {useContext} from "react";
import {AppUserContext} from "../context/StateContext.tsx";
import {Navigate} from "react-router-dom";


type ProtectedRouteProps = {
    children: React.ReactNode;
}
export default function ProtectedRoute( { children } : ProtectedRouteProps ) {

    const { appUser } = useContext(AppUserContext);

    console.log("ProtectedRoute - appUser Value:", appUser);
    if (!appUser) {
        console.log("No appUser in ProtectedRoute. Navigating to login.");
        return <Navigate to="/" replace />;
    }

    return children;
}

