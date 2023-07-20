import './App.css'
import {BrowserRouter as Router, Routes, Route} from "react-router-dom";
import Login from './pages/Login';
import Orders from './pages/Orders';
import Settings from './pages/Settings';
import LiveLogs from './pages/LiveLogs.tsx';
import SocialLogin from "./pages/SocialLogin.tsx";
import {useEffect} from "react";
import {useSignalRService} from "./context/SignalRContext.tsx";
/*import NavBar from './components/NavBar.tsx';*/

function App() {
    const { startConnection } = useSignalRService();

    useEffect(() => {
        startConnection().catch(error => {
            console.error("SignalR bağlantısı başlatılamadı:", error);
        });
    }, [startConnection]);


  return (

      <Router>
          <Routes>
              <Route path="/" element={<Login />} />
              <Route path="/social-login" element={<SocialLogin />} />
              <Route path="/orders" element={<Orders />} />
              <Route path="/settings" element={<Settings />} />
              <Route path="/livelogs" element={<LiveLogs />} />
          </Routes>
      </Router>

  )
}

export default App
