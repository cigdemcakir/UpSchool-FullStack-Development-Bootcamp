import {useState, ChangeEvent, FormEvent, useEffect} from 'react';
import useSignalR from '../hooks/useSignalR';
import 'tailwindcss/tailwind.css';
import {Grid} from "semantic-ui-react";
import {Link} from "react-router-dom";
import './LoginPage.css';
import * as XLSX from 'xlsx';
import emailjs from '@emailjs/browser';
import {useSelector} from "react-redux";
import {RootState} from "../types";
import {getClaimsFromJwt} from "../utils/jwtHelper.ts";
import Login from "./Login.tsx";
/*import { useRef } from 'react';*/
/*import {useSignalRService} from "../context/SignalRContext.tsx";*/

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

    const email = useSelector((state: RootState) => state.email);

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

            return () => {
                connection.off("NewProductAdded", handleNewProductLogAdded);
            };

        } else {
            console.error("Connection not started yet.");
        }

    }, [connection, connectionStarted]);

    const jwtJson = localStorage.getItem("softwarehouse_user");

    if (!jwtJson) {
        return <Login showForm={true} />;
    }

    /*const form = useRef();*/

    const sendEmail = (e) => {
        e.preventDefault();

        const localUser = localStorage.getItem("softwarehouse_user");

        if (localUser) {
            const parsedUser = JSON.parse(localUser);
            const accessTokenFromLocalStorage = parsedUser?.accessToken;

            if (accessTokenFromLocalStorage) {
                const { email } = getClaimsFromJwt(accessTokenFromLocalStorage);
                if (email) {
                    console.log("E-posta adresi:", email);

                    const orderLogsStr = orderLogs.map(log => `${log.id} - ${log.message}`).join('\n');
                    const productLogsStr = productLogs.map(log => `${log.id} - ${log.message}`).join('\n');

                    const allLogsStr = `Order Logs:\n${orderLogsStr}\n\nProduct Logs:\n${productLogsStr}`;

                    emailjs.send("", "", {
                        logs: allLogsStr,
                        to_email: email,
                    }, "")

                        .then((result) => {
                            console.log('E-posta başarıyla gönderildi:', result.text);
                        }, (error) => {
                            console.error('E-posta gönderiminde bir hata oluştu:', error.text);
                        });
                } else {
                    console.error("E-posta adresi token içerisinde bulunamadı.");
                }
            } else {
                console.error("localStorage'da accessToken bulunamadı.");
            }
        } else {
            console.error("localStorage'da kullanıcı bilgisi bulunamadı.");
        }


    };
    const exportToExcel = () => {
        const wb = XLSX.utils.book_new();

        const data = productLogs.map(log => [log.id, log.message]);

        data.unshift(["ID", "Message"]);

        const ws = XLSX.utils.aoa_to_sheet(data);

        ws['A1'].s = {
            font: { bold: true },
            fill: { bgColor: { rgb: "FFFF00" } }
        };
        ws['B1'].s = {
            font: { bold: true },
            fill: { bgColor: { rgb: "FFFF00" } }
        };

        XLSX.utils.book_append_sheet(wb, ws, "Order");

        XLSX.writeFile(wb, "OrderDetail.xlsx");
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
                        <button onClick={sendEmail}>Send Mail</button>
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