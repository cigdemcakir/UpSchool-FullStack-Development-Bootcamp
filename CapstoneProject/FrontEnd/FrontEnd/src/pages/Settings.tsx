import { useState, ChangeEvent, FormEvent } from 'react';

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
        <div>
            <h1>Settings Page</h1>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '20px' }}>
                    <label>
                        Notification Type:
                        <select value={notificationType} onChange={handleNotificationChange}>
                            <option value="email">E-Posta</option>
                            <option value="in-app">Uygulama İçi Notifikasyon</option>
                            <option value="none">Hiçbiri</option>
                        </select>
                    </label>
                </div>
                <div style={{ marginBottom: '20px' }}>
                    <input type="submit" value="Save Settings" />
                </div>
            </form>
        </div>
    );
}

export default SettingsPage;
