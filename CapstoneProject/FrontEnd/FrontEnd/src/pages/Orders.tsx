import {useState, ChangeEvent, FormEvent, useEffect} from 'react';
import useSignalR from '../hooks/useSignalR';
import 'tailwindcss/tailwind.css';
import {Grid} from "semantic-ui-react";
import {Link} from "react-router-dom";
import './LoginPage.css';

const BASE_URL = import.meta.env.VITE_CRAWLERHUB_URL;



function OrderPage() {
    const [count, setCount] = useState(0);
    const [crawlType, setCrawlType] = useState('all');
    const { connection, connectionStarted, startConnection} = useSignalR(BASE_URL);

    useEffect(() => {
        startConnection();
    }, [startConnection]);

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
                    </div>
                </section>
            </Grid.Column>

        </Grid>


    );
}

export default OrderPage;