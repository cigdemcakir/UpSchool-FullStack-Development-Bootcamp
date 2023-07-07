// Navbar.js
import React from 'react';
import { Link } from 'react-router-dom';

function Navbar() {
    const navStyle: React.CSSProperties = {
        display: 'flex',
        justifyContent: 'space-between',
        listStyle: 'none',
        padding: '2em',
        backgroundColor: '#ddd',
        position: 'fixed',
        top: 0,
        width: '100%',

    };

    const linkStyle: React.CSSProperties = {
        textDecoration: 'none',
        color: 'black',
    };

    return (
        <nav>
            <ul style={navStyle}>
                <li>
                    <Link to="/" style={linkStyle}>Login</Link>
                </li>
                <li>
                    <Link to="/orders" style={linkStyle}>Orders</Link>
                </li>
                <li>
                    <Link to="/settings" style={linkStyle}>Settings</Link>
                </li>
                <li>
                    <Link to="/users" style={linkStyle}>Users</Link>
                </li>
            </ul>
        </nav>
    );
}

export default Navbar;
