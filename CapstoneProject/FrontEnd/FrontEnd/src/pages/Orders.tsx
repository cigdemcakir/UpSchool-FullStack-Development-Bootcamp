import  { useState, ChangeEvent, FormEvent } from 'react';

function OrderPage() {
    const [count, setCount] = useState(0);
    const [crawlType, setCrawlType] = useState('all');

    const handleCountChange = (e: ChangeEvent<HTMLInputElement>) => {
        setCount(Number(e.target.value));
    };

    const handleCrawlTypeChange = (e: ChangeEvent<HTMLSelectElement>) => {
        setCrawlType(e.target.value);
    };

    const handleSubmit = (e: FormEvent) => {
        e.preventDefault();
        // Burada backend ile iletişime geçip siparişi oluşturabilirsiniz.
        console.log(`Order created! Count: ${count}, Crawl Type: ${crawlType}`);
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
