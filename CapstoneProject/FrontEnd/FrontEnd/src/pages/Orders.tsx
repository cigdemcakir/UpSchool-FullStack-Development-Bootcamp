import {useState, ChangeEvent, FormEvent, useEffect} from 'react';
import useSignalR from '../hooks/useSignalR'
import 'tailwindcss/tailwind.css'

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

        // Socket eventi tetikleniyor.
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
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
            <h1 className="text-4xl font-bold mb-10 text-gray-700">Order Page</h1>
            <form onSubmit={handleSubmit} className="p-10 bg-white rounded shadow-xl w-1/3">
                <div className="mb-5">
                    <label className="block text-sm font-semibold text-gray-600">Product Count:</label>
                    <input type="number" value={count} onChange={handleCountChange} className="w-full px-2 py-2 border rounded mt-1 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-400"/>
                </div>
                <div className="mb-5">
                    <label className="block text-sm font-semibold text-gray-600">Product Crawl Type:</label>
                    <select value={crawlType} onChange={handleCrawlTypeChange} className="w-full px-2 py-2 border rounded mt-1 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-400">
                        <option value="all">All</option>
                        <option value="discount">Discount</option>
                        <option value="nondiscount">Non-Discount</option>
                    </select>
                </div>
                <input type="submit" value="Create Order" className="w-full py-2 px-4 rounded text-white bg-blue-500 hover:bg-blue-600 focus:outline-none"/>
            </form>
        </div>



    );
}

export default OrderPage;

/*
<div className="container" style={{ backgroundColor: '#333'}}>
                    <div className="row justify-content-center">
            <div className="col-md-7">
                <div className="terminal shadow mt-1 mb-3 bg-light bg-opacity-10 rounded">
                    <div className="top">
                        <div className="title" style={{ color: '#fff3cd' }}>Product Crawler</div>
                    </div>
                    <div className="card-footer d-flex justify-content-between">
                        <label htmlFor="requestedAmountInput" style={{ color: '#cdcbc2' }}>Product Count:</label>
                        <input
                            type="number"
                            name="requestedAmount"
                            value={count}
                            onChange={handleCountChange}
                            placeholder="Product Count"
                            id="requestedAmountInput"
                        />
                        <label htmlFor="crawlTypeSelect" style={{ color: '#cdcbc2' }}>Crawl Type:</label>
                        <select
                            name="crawlType"
                            value={crawlType}
                            onChange={handleCrawlTypeChange}
                            id="crawlTypeSelect"
                        >
                            <option value="All">All</option>
                            <option value="OnSale">Sale</option>
                            <option value="NormalPrice">Normal Price</option>
                        </select>
                    </div>
                    <div className="card-footer d-flex justify-content-center">
                    </div>
                </div>
            </div>
        </div>
    </div>

 */
