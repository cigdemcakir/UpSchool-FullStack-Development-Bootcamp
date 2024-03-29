import {useState, ChangeEvent, FormEvent, useEffect, useContext} from 'react';
import useSignalR from '../hooks/useSignalR';
import 'tailwindcss/tailwind.css';
import {Grid} from "semantic-ui-react";
import {Link} from "react-router-dom";
import './LoginPage.css';
import emailjs from '@emailjs/browser';
import {useSelector} from "react-redux";
import {RootState} from "../types";
import {getClaimsFromJwt} from "../utils/jwtHelper.ts";
import Login from "./Login.tsx";
import {AppUserContext} from "../context/StateContext.tsx";
import * as ExcelJS from "exceljs";
import { useNavigate } from 'react-router-dom';

const BASE_URL = import.meta.env.VITE_CRAWLERHUB_URL;

type CrawlerLogDto = {
    message: string;
    id: string;
};

function parseProductData(data) {
    const parts = data.split("   -    ");

    const getName = (str) => str.split(": ")[1];

    const regularPrice = parseFloat(getName(parts[2]));
    const salePrice = parseFloat(getName(parts[3]));

    const product = {
        Name: getName(parts[0]),
        IsOnSale: !isNaN(salePrice) && salePrice !== regularPrice, // Adjusted this line
        Price: regularPrice,
        SalePrice: salePrice,
        Picture: getName(parts[4]),
        OrderId: getName(parts[5])
    };

    return product;
}

function OrderPage() {

    const [orderLogs, setOrderLogs] = useState<CrawlerLogDto[]>([]);

    const [productLogs, setProductLogs] = useState<CrawlerLogDto[]>([]);

    const [count, setCount] = useState(0);

    const [showTable, setShowTable] = useState(false);

    const [crawlType, setCrawlType] = useState('all');

    const { connection, connectionStarted, startConnection} = useSignalR(BASE_URL);

    const { appUser } = useContext(AppUserContext);

    const [notifications, setNotifications] = useState<string[]>(['Welcome!']);

    const [isNotificationsOpen, setNotificationsOpen] = useState(false);

    const email = useSelector((state: RootState) => state.email);

    const navigate = useNavigate();


    useEffect(() => {
        startConnection();
    }, [startConnection]);

    useEffect(() => {
        if (!connection) {
            console.error("Connection failed!");
            return;
        }

        if (connectionStarted) {
            console.log('Connected successfully.');

            const handleNewOrderLogAdded = (crawlerLogDto: CrawlerLogDto) => {
                setOrderLogs((prevLogs) => [...prevLogs, crawlerLogDto]);
                setNotifications((prevNotifications) => [...prevNotifications, crawlerLogDto.message]);
            };

            connection.on('NewOrderAdded', handleNewOrderLogAdded);

            return () => {
                connection.off('NewOrderAdded', handleNewOrderLogAdded);
            };
        } else {
            console.error('Connection not started yet.');
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

                    const productLogsStr = productLogs.map(log => log.message).join('\n');

                    const allLogsStr = `Product Logs:\n${productLogsStr}`;

                    emailjs.send("", "", {
                        logs: allLogsStr,
                        to_email: email,
                        message:allLogsStr
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

    const exportToExcel = async () => {
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet("Order");

        const data = productLogs.map(log => {
            const product = parseProductData(log.message);
            return [
                product.Name,
                product.IsOnSale ? 'Yes' : 'No',
                product.Price,
                isNaN(product.SalePrice) ? '-' : product.SalePrice,
                product.Picture
            ];
        });

        data.unshift(["Product Name", "Is On Sale?", "Product Price", "Sale Price", "Product"]);

        data.forEach((row, rowIndex) => {
            const worksheetRow = worksheet.addRow(row);

            if (rowIndex === 0) {
                worksheetRow.eachCell((cell) => {
                    cell.font = {
                        bold: true,
                        size: 16,
                        color: { argb: "FFFFFF" }
                    };
                    cell.fill = {
                        type: "pattern",
                        pattern: "solid",
                        fgColor: { argb: "FFdf9fe0" }
                    };
                    cell.alignment = {
                        vertical: "middle",
                        horizontal: "center"
                    };
                });
            }

            else {
                worksheetRow.eachCell((cell) => {
                    cell.fill = {
                        type: "pattern",
                        pattern: "solid",
                        fgColor: { argb: (rowIndex % 2 === 0) ? "FF4a9af0" : "FFFFFF" }
                    };
                    cell.alignment = {
                        vertical: "middle",
                        horizontal: "center"
                    };
                });
            }
        });

        worksheet.columns = [
            { width: 50 },
            { width: 30 },
            { width: 40 },
            { width: 40 },
            { width: 50 }
        ];

        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = "OrderDetails.xlsx";
        link.click();
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
/*
        setTimeout(() => {
            navigate('/livelogs');
        }, 3000);

 */

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
                                <Link to="/users" className="nav_link">Users</Link>
                                <Link to="/livelogs" className="nav_link">LiveLogs</Link>
                            </li>
                        </ul>
                        { appUser ? (
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <div onClick={() => setNotificationsOpen(prev => !prev)} style={{ marginRight: '20px' }}>
                                    <img src="/bell.png" alt="Notifications Icon" className="notifications-icon" />
                                    {notifications.length > 0 && <span className="notification-count">1</span>}
                                    {isNotificationsOpen && (
                                        <div className="notifications-menu">
                                            {notifications.map((notification, index) => (
                                                <div key={index} className="notification-item">
                                                    {notification}
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                                <Link to="/users">
                                    <img src="/user.png" alt="User Icon" className="user-icon" />
                                </Link>
                            </div>) : (
                            <button className="button" id="form-open" >Login</button>
                        )}
                    </nav>
                </header>
                <section className={`home show crawler`}>
                    <div className="form_container scaleUpAnimation">
                        <div>
                            <form onSubmit={handleSubmit}>
                                <div className="h2Container">
                                    <h2 style={{ marginTop:'10px'}} className="text-xl font-bold mb-5 minBackground">Product Crawler</h2>
                                </div>
                                <div className="mb-5 h2Container">
                                    <label style={{ width: '150px', display: 'inline-block', marginTop:'44px', marginRight:'20px', marginLeft:'20px'}} className="text-sm font-semibold text-gray-600 minBackground">Product Count:</label>
                                    <input style={{ width: '60px', height:'30px',marginTop:'40px', textAlign:'center', borderColor:'purple'}} type="number" min="1"  value={count} onChange={handleCountChange} className="w-2/3 px-2 py-2 border rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-400 "/>
                                    <label style={{ width: '125px', marginLeft:'20px', marginTop:'44px'}} className="text-sm font-semibold text-gray-600 minBackground">Crawl Type:</label>
                                    <select value={crawlType} style={{ width: '120px', marginLeft:'20px', marginTop:'40px', borderColor:'purple', height:'30px'}} onChange={handleCrawlTypeChange} className="w-2/3 px-2 py-2 border rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-400">
                                        <option value="all" >All</option>
                                        <option value="discount">Discount</option>
                                        <option value="nondiscount">Non-Discount</option>
                                    </select>
                                    <input type="submit" value="Create Order" style={{ marginLeft:'40px', marginTop: '40px' }} className="minBackground"/>
                                </div>
                            </form>
                            <div style={{margin:'center', display:'center', marginLeft:'300px'}}>
                                <button style={{marginTop:'38px'}} onClick={() => setShowTable(!showTable)} className="minBackground">Get Order Details</button>
                                <button style={{marginLeft:'10px'}} onClick={exportToExcel} className="minBackground">Export to Excel</button>
                                <button style={{marginLeft:'10px'}} onClick={sendEmail} className="minBackground">Send Mail</button>
                                <button style={{marginLeft:'10px'}} onClick={() => setProductLogs([])}  className="minBackground">Delete Order</button>
                            </div>
                            {showTable && (
                                <div className="tableComponent" style={{ overflowY: 'auto', height: '400px', textAlign:'center', marginTop: '40px', flexDirection: 'column' }}>
                                    <table className="userTable" style={{ textAlign: "center", width: '100%', borderCollapse: 'collapse' }}>
                                        <thead style={{ position: 'sticky', top: 0, zIndex: 1, background: '#f2f2f2' }}>
                                        <tr>
                                            <th style={{ flex: 1, border: '1px solid #ddd', padding: '8px' }}>Product Name</th>
                                            <th style={{ flex: 1, border: '1px solid #ddd', padding: '8px' }}>Is On Sale?</th>
                                            <th style={{ flex: 1, border: '1px solid #ddd', padding: '8px' }}>Product Price</th>
                                            <th style={{ flex: 1, border: '1px solid #ddd', padding: '8px' }}>Sale Price</th>
                                            <th style={{ flex: 1, border: '1px solid #ddd', padding: '8px' }}>Product</th>
                                        </tr>
                                        </thead>
                                        <tbody>
                                        {productLogs.map((log, index) => {
                                            const product = parseProductData(log.message);
                                            return (
                                                <tr key={index}>
                                                    <td>{product.Name}</td>
                                                    <td>{product.IsOnSale ? 'Yes' : 'No'}</td>
                                                    <td>{product.Price}</td>
                                                    <td>{isNaN(product.SalePrice) ? '-' : product.SalePrice}</td>
                                                    <td><img src={product.Picture} alt="Product" width="50" height="50" /></td>
                                                </tr>
                                            );
                                        })}
                                        </tbody>
                                    </table>
                                </div>
                            )}
                        </div>
                        <div>
                        </div>
                    </div>
                </section>
            </Grid.Column>
        </Grid>
    );
}

export default OrderPage;