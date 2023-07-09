import { useEffect, useState } from 'react';

function Users() {
    const [logs, setLogs] = useState<string[]>([]);

    useEffect(() => {
        const fakeLogs = [
            'Sipariş oluşturuldu...',
            'Sipariş bilgileri doğrulanıyor...',
            'Ürünler kazıma işlemi için sıraya alındı...',
            'Kazıma işlemi başladı...',
            'Kazıma işlemi tamamlandı. Ürün bilgileri alındı...',
            'Sipariş tamamlandı!'
        ];

        const addLog = (index: number) => {
            if (index < fakeLogs.length) {
                setLogs((prevLogs) => [...prevLogs, `${new Date().toLocaleString()}: ${fakeLogs[index]}`]);
                setTimeout(() => addLog(index + 1), 2000);
            }
        };

        addLog(0);
    }, []);

    return (
        <div>
            <h1>Live Log Page</h1>
            <div style={{ backgroundColor: 'black', color: 'lime', padding: '10px', fontFamily: 'Courier', whiteSpace: 'pre-line' }}>
                {logs.join('\n')}
            </div>
        </div>
    );
}

export default Users;
