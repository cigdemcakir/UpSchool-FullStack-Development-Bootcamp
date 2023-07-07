import './App.css'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from './pages/Login';
import Orders from './pages/Orders';
import Settings from './pages/Settings';
import Users from './pages/Users';
import NavBar from './components/NavBar.tsx';
function App() {

  return (
      <Router>
          <NavBar/>
          <Routes>
              <Route path="/" element={<Login />} />
              <Route path="/orders" element={<Orders />} />
              <Route path="/settings" element={<Settings />} />
              <Route path="/users" element={<Users />} />
          </Routes>
      </Router>
  )
}

export default App
