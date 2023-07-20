import {useState, ChangeEvent, FormEvent, useEffect} from 'react';
import useSignalR from '../hooks/useSignalR';
import 'tailwindcss/tailwind.css';
import {Grid} from "semantic-ui-react";
import {Link} from "react-router-dom";
import './LoginPage.css';
import * as XLSX from 'xlsx';
import {useSignalRService} from "../context/SignalRContext.tsx";

const BASE_URL = import.meta.env.VITE_CRAWLERHUB_URL;

type CrawlerLogDto = {
    message: string;
    id: string;
};

function OrderPage() {
    const [orderLogs, setOrderLogs] = useState<CrawlerLogDto[]>([]);
    const [productLogs, setProductLogs] = useState<CrawlerLogDto[]>([]);
    const [count, setCount] = useState(0);
    const [crawlType, setCrawlType] = useState('all');
    const { connection, connectionStarted, startConnection} = useSignalR(BASE_URL);

    useEffect(() => {
        startConnection();
    }, [startConnection]);


    useEffect(() => {
        if (!connection) {
            console.error("Connection failed!");
            return;
        }

        if (connectionStarted) {
            console.log("Connected successfully.");

            const handleNewOrderLogAdded = (crawlerLogDto: CrawlerLogDto) => {
                setOrderLogs(prevLogs => [...prevLogs, crawlerLogDto]);
            };

            connection.on("NewOrderAdded", handleNewOrderLogAdded);

            // Olay dinleyicisini temizleme
            return () => {
                connection.off("NewOrderAdded", handleNewOrderLogAdded);
            };

        } else {
            console.error("Connection not started yet.");
        }

    }, [connection, connectionStarted]);

    useEffect(() => {
        if (!connection) {
            console.error("Connection failed!");
            return;
        }

        if (connectionStarted) {
            console.log("Connected successfully.");

            const handleNewProductLogAdded = (crawlerLogDto: CrawlerLogDto) => {
                setProductLogs(prevLogs => [...prevLogs, crawlerLogDto]);
            };

            connection.on("NewProductAdded", handleNewProductLogAdded);

            // Olay dinleyicisini temizleme
            return () => {
                connection.off("NewProductAdded", handleNewProductLogAdded);
            };

        } else {
            console.error("Connection not started yet.");
        }

    }, [connection, connectionStarted]);

    const exportToExcel = () => {
        const ws = XLSX.utils.json_to_sheet(productLogs);

        const wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, "ProductLogs");

        XLSX.writeFile(wb, "product_logs.xlsx");
    };

    const createOrder = async (productNumber: number, type: string) => {
        let productCrawlType = '';

        switch (type.toLowerCase()) {
            case 'all':
                productCrawlType = 'All';
                break;
            case 'discount':
                productCrawlType = 'OnDiscount';
                break;
            case 'nondiscount':
                productCrawlType = 'NonDiscount';
                break;
            default:
                productCrawlType = 'All';
                break;
        }

        const order = {
            ProductNumber: productNumber,
            ProductCrawlType: productCrawlType,
        };

        if (connection) {
            try {
                console.log('Sending order to server:', order);
                await connection.invoke('SendOrderNotificationAsync', productNumber, productCrawlType);
                console.log('Order sent successfully.');
            } catch (error) {
                console.error('Failed to send order:', error);
            }
        }
    };

    const handleCountChange = (e: ChangeEvent<HTMLInputElement>) => {
        setCount(Number(e.target.value));
    };

    const handleCrawlTypeChange = (e: ChangeEvent<HTMLSelectElement>) => {
        setCrawlType(e.target.value);
    };

    const handleSubmit = (e: FormEvent) => {
        e.preventDefault();
        try {
            console.log(`Order created! Count: ${count}, Crawl Type: ${crawlType}`);

            if (connection && connectionStarted) {
                createOrder(count, crawlType);
            }
        } catch (error) {
            console.error('Failed to submit form:', error);
        }
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
                            <form onSubmit={handleSubmit}>
                                <h2 className="text-xl font-bold mb-5">Crawler</h2>
                                <div className="mb-5">
                                    <label style={{ width: '120px', display: 'inline-block', marginTop:'20px' }} className="text-sm font-semibold text-gray-600">Product Count:</label>
                                    <input style={{ width: '120px'}} type="number" value={count} onChange={handleCountChange} className="w-2/3 px-2 py-2 border rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-400"/>
                                </div>
                                <div className="mb-5">
                                    <label style={{ width: '120px', display: 'inline-block' }} className="text-sm font-semibold text-gray-600">Crawl Type:</label>
                                    <select value={crawlType} style={{ width: '120px'}} onChange={handleCrawlTypeChange} className="w-2/3 px-2 py-2 border rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-400">
                                        <option value="all">All</option>
                                        <option value="discount">Discount</option>
                                        <option value="nondiscount">Non-Discount</option>
                                    </select>
                                </div>
                                <input type="submit" value="Create Order" style={{ display: 'block', marginLeft: 'auto', marginRight: 'auto', marginTop: '20px' }} className="py-2 px-4 rounded text-white bg-blue-500 hover:bg-blue-600 focus:outline-none"/>
                            </form>
                        </div>
                        <button onClick={exportToExcel}>Export to Excel</button>
                        <h2>Order Logs</h2>
                        <table className="min-w-full border-collapse">
                            <thead>
                            <tr>
                                <th className="px-6 py-3 border-b-2 border-gray-300 text-left leading-4 text-blue-500 tracking-wider">Order ID</th>
                                <th className="px-6 py-3 border-b-2 border-gray-300 text-left leading-4 text-blue-500 tracking-wider">Order Message</th>
                            </tr>
                            </thead>
                            <tbody>
                            {orderLogs.map(log => (
                                <tr key={log.id}>
                                    <td className="px-6 py-4 whitespace-no-wrap border-b border-gray-500">{log.id}</td>
                                    <td className="px-6 py-4 whitespace-no-wrap border-b border-gray-500">{log.message}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>

                        <h2>Product Logs</h2>
                        <table className="min-w-full border-collapse">
                            <thead>
                            <tr>
                                <th className="px-6 py-3 border-b-2 border-gray-300 text-left leading-4 text-blue-500 tracking-wider">Product ID</th>
                                <th className="px-6 py-3 border-b-2 border-gray-300 text-left leading-4 text-blue-500 tracking-wider">Product Message</th>
                            </tr>
                            </thead>
                            <tbody>
                            {productLogs.map(log => (
                                <tr key={log.id}>
                                    <td className="px-6 py-4 whitespace-no-wrap border-b border-gray-500">{log.id}</td>
                                    <td className="px-6 py-4 whitespace-no-wrap border-b border-gray-500">{log.message}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>

                    </div>
                </section>

            </Grid.Column>

        </Grid>


    );
}

export default OrderPage;