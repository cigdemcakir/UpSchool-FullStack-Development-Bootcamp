import {useState, ChangeEvent, FormEvent, useEffect} from 'react';
import useSignalR from '../hooks/useSignalR'

const BASE_URL = import.meta.env.VITE_CRAWLERHUB_URL;

function OrderPage() {
    const [count, setCount] = useState(0);
    const [crawlType, setCrawlType] = useState('all');
    const { connection, connectionStarted, startConnection, stopConnection} = useSignalR(BASE_URL);

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
                console.log('Sending order to server:', order);  // order'ın gönderilmeden önceki durumunu logla
                await connection.invoke('SendOrderNotificationAsync', productNumber, productCrawlType);
                console.log('Order sent successfully.');  // order başarıyla gönderildiğinde log at
            } catch (error) {
                console.error('Failed to send order:', error);  // Hata oluşursa, hata mesajını logla
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
                createOrder(count, crawlType);  // sipariş oluşturuluyor.
            }
        } catch (error) {
            console.error('Failed to submit form:', error);
        }
    };

    return (
        <div>
            <h1>Order Page</h1>
            <form onSubmit={handleSubmit}>
                <label>
                    Product Count:
                    <input type="number" value={count} onChange={handleCountChange} />
                </label>
                <label>
                    Product Crawl Type:
                    <select value={crawlType} onChange={handleCrawlTypeChange}>
                        <option value="all">All</option>
                        <option value="discount">Discount</option>
                        <option value="nondiscount">Non-Discount</option>
                    </select>
                </label>
                <input type="submit" value="Create Order" />
            </form>
        </div>
    );
}

export default OrderPage;

