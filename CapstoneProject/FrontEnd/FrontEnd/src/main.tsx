import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css'
import {SignalRProvider} from "./context/SignalRContext.tsx";

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <React.StrictMode>
        <SignalRProvider>
            <App />
        </SignalRProvider>
    </React.StrictMode>,
)
