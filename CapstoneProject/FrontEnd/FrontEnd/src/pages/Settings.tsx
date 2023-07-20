import { useState, ChangeEvent, FormEvent } from 'react';
import {Link} from "react-router-dom";
import {Grid} from "semantic-ui-react";

function SettingsPage() {
    const [notificationType, setNotificationType] = useState('email');

    const handleNotificationChange = (e: ChangeEvent<HTMLSelectElement>) => {
        setNotificationType(e.target.value);
    };

    const handleSubmit = (e: FormEvent) => {
        e.preventDefault();
        // Burada backend ile iletişime geçip ayarları kaydedebilirsiniz.
        console.log(`Settings saved! Notification type: ${notificationType}`);
    };

    return (
        <Grid textAlign='center' style={{ height: '100vh',
            position: 'fixed',
            width: '100%',
            backgroundImage: `url(/bg.jpg)`,
            backgroundSize: 'cover',
            backgroundRepeat: 'no-repeat',
            backgroundAttachment: 'fixed',
            backgroundPosition:'center'}} >
            <Grid.Column style={{ maxWidth: 450 }}>
                <header className="header">
                    <nav className="nav">
                        <a href="#" className="nav_logo">SoftwareHouse</a>
                        <ul className="nav_items">
                            <li className="nav_item">
                                <a href="/" className="nav_link">Home</a>
                                <Link to="/orders" className="nav_link">Orders</Link>
                                <Link to="/settings" className="nav_link">Settings</Link>
                                <Link to="/livelogs" className="nav_link">LiveLogs</Link>
                            </li>
                        </ul>
                        <button className="button" id="form-open">Login</button>
                    </nav>
                </header>
                <section className={`home show`}>
                    <div className="form_container">
                        <div>
                            <h1>Settings</h1>
                            <form onSubmit={handleSubmit}>
                                <div style={{ marginBottom: '20px' }}>
                                    <label>
                                        Notification Type:
                                        <select value={notificationType} onChange={handleNotificationChange}>
                                            <option value="email">Email</option>
                                            <option value="in-app">App-Based Notification</option>
                                            <option value="none">None</option>
                                        </select>
                                    </label>
                                </div>
                                <div style={{ marginBottom: '20px' }}>
                                    <input type="submit" value="Save Settings" />
                                </div>
                            </form>
                        </div>
                    </div>
                </section>
            </Grid.Column>
        </Grid>

    );
}

export default SettingsPage;
