using System.Text;
using Domain.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Text.RegularExpressions;
using Application.Features.OrderEvents.Commands.Add;
using Application.Features.Orders.Commands.Add;
using Application.Features.Products.Commands.Add;
using Domain.Enums;
using Newtonsoft.Json;
using OfficeOpenXml;

try
{
    #region Locators
     
    By productsLocator = By.CssSelector(".card.h-100");
    By pageNumberLocator = By.CssSelector(".page-link.page-number");
    By productName = By.CssSelector(".product-name");
    By productPrice = By.CssSelector(".price");
    By pictureLink = By.CssSelector(".card-img-top");
    By onSale = By.CssSelector(".onsale");
    By productOnSalePrice = By.CssSelector(".sale-price");
    
    #endregion

    #region Urls
    
    const string homePageUrl = "https://finalproject.dotnet.gg/";
    const string orderAddUrl = "https://localhost:7015/api/Orders/Add";
    const string orderEventsAddUrl = "https://localhost:7015/api/OrderEvents/Add";
    const string productsAddUrl = "https://localhost:7015/api/Products/Add";
    const string crawlerHubUrl = "https://localhost:7015/Hubs/CrawlerHub";

    #endregion
    
    string productType;
    string? salePrice = null;
    string crawlRequestAmount;
    int crawledProductCount=0;
    bool isCrawlingCountinue = true;
    
    var httpClient = new HttpClient();
    
    var orderAddRequest = new OrderAddCommand();

    var orderEventAddRequest = new OrderEventAddCommand();

    ExcelPackage.LicenseContext = LicenseContext.Commercial;

    new DriverManager().SetUpDriver(new ChromeConfig());
    
    IWebDriver? driver = new ChromeDriver();
    
    var hubConnection = new HubConnectionBuilder()
        .WithUrl(crawlerHubUrl)
        .WithAutomaticReconnect()
        .Build();
    
    HowManyProductDoYouWantToCrawl();
    
    WhatKindOfProductsDoYouWantToCrawl();
    
    CreateOrder();
    
    await hubConnection.StartAsync();
    
    CreateOrderEvent(OrderStatus.BotStarted);
    
    driver.Navigate().GoToUrl(homePageUrl);
    
    Sleep(3);
    
    await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Navigated to UpStorage Shop"));
    
    Sleep(2);
    
    CreateOrderEvent(OrderStatus.CrawlingStarted);
    
    bool isNumber = Regex.IsMatch(crawlRequestAmount, @"^\d+$");
    
    if (isNumber)
    {
        var crawlRequestNumber = Convert.ToInt32(crawlRequestAmount);
        
        CrawlProducts(crawlRequestNumber);
    }
    else
    {
        CrawlProducts();
    }
    
    async void CrawlProducts(int? requestNumber = null)
    {
        List<Product> productList = new List<Product>();
        
        var pageLinks = driver.FindElements(pageNumberLocator)
                              .Select(x => x.GetAttribute("href"))
                              .ToList();
    
        driver.Navigate().GoToUrl(pageLinks[0]);
    
        for (var currentPage = 1; currentPage <= pageLinks.Count; currentPage++)
        {
            var products = driver.FindElements(productsLocator);
    
            foreach (var productElement in products)
            {
                if (requestNumber.HasValue && crawledProductCount == requestNumber)
                    break;
                
                var isOnSale = productElement.FindElements(onSale).Count != 0;
    
                if ((productType.ToLower() == "a")
                    || (productType.ToLower() == "b" && isOnSale)
                    || (productType.ToLower() == "c" && !isOnSale))
                {
                    var name = productElement.FindElement(productName).Text;
                    var price = productElement.FindElement(productPrice).Text;
                    var picture = productElement.FindElement(pictureLink).GetAttribute("src");
    
                    if (isOnSale)
                    {
                        salePrice = productElement.FindElement(productOnSalePrice).Text;
                        salePrice = salePrice.Replace("$", "");
                    }
                    
                    price = price.Replace("$", "");
                    
                    if(crawledProductCount==1) await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"1. product were crawled"));
                    
                    crawledProductCount++;
                    
                    var productAddRequest = new ProductAddCommand()
                    {
                        OrderId =orderAddRequest.Id,
                        Name = name,
                        IsOnSale = isOnSale,
                        Price=decimal.Parse(price),
                        SalePrice=decimal.Parse(salePrice),
                        Picture=picture,
                        

                    };
                    
                    await SendHttpPostRequest<ProductAddCommand, object>(httpClient, productsAddUrl, productAddRequest);

                    await hubConnection.InvokeAsync("SendProductNotificationAsync", CreateLog(
                        $"Product Name : {name}" + "   -    " +
                        $"Is On Sale ? :   {isOnSale}" + "   -    " +
                        $"Product Price :   {price}" + "   -    " +
                        $"Product Sale Price :   {salePrice}"+ "   -    " +
                        $"Product Sale Price :   {price}"));
                    
                    Console.WriteLine($"Name: {name} -- OnSale: {isOnSale} -- Price: {price} -- Sale Price: {salePrice} -- Path: {picture}");
                }
            }
            
            await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"{crawledProductCount}. product were crawled"));

            if (requestNumber.HasValue && crawledProductCount == requestNumber)
                break;
    
            if (currentPage < pageLinks.Count && !string.IsNullOrEmpty(pageLinks[currentPage]))
            {
                driver.Navigate().GoToUrl(pageLinks[currentPage]);
            }
        }
        CreateOrderEvent(OrderStatus.CrawlingCompleted);
        
        CreateExcelSheet(productList);

        await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Mission Accomplished!"));
        
        Sleep(2);
        
        CreateOrderEvent(OrderStatus.OrderCompleted);
    }
    
    Console.ReadKey();
    
    driver.Quit();

    async void CreateOrderEvent(OrderStatus orderStatus)
    {
        orderEventAddRequest = new OrderEventAddCommand()
        {
            OrderId= orderAddRequest.Id,
            Status=orderStatus,
        };
        
        await SendHttpPostRequest<OrderEventAddCommand, object>(httpClient, orderEventsAddUrl, orderEventAddRequest);
    
        await hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Order Status : "+ orderEventAddRequest.Status.ToString()));
    }

    async void CreateOrder()
    {
        switch (productType.ToLower())
        {
            case "a":
                orderAddRequest = new OrderAddCommand()
                {
                    Id = Guid.NewGuid(),
                    ProductCrawlType = ProductCrawlType.All
                };
                break;

            case "b":
                orderAddRequest = new OrderAddCommand()
                {
                    Id = Guid.NewGuid(),
                    ProductCrawlType = ProductCrawlType.All
                };
                break;

            case "c":
                orderAddRequest = new OrderAddCommand()
                {
                    Id = Guid.NewGuid(),
                    ProductCrawlType = ProductCrawlType.All
                };
                break;
        }
        
        await SendHttpPostRequest<OrderAddCommand, object>(httpClient, orderAddUrl, orderAddRequest);
        
        await hubConnection.InvokeAsync("SendOrderNotificationAsync", CreateLog($"Order Id : {orderAddRequest.Id}  -  Crawl Type : {orderAddRequest.ProductCrawlType.ToString()}"));
    }
    
    void WhatKindOfProductsDoYouWantToCrawl()
    {
        string[] validOptions = { "a", "b", "c" };

        Console.WriteLine("What kind of products do you want to crawl? " +
                "\nPlease enter A, B or C." +
                "\nA) All\nB) On Sale\nC) Nondiscount Products");

        do
        {
            productType = Console.ReadLine().ToLower();

            if (!validOptions.Contains(productType))
                Console.WriteLine("You entered an invalid option, try again!");

        } while (!validOptions.Contains(productType));

        Console.WriteLine("Please wait, the process is starting...");

        
    }
    
    void Sleep(int seconds)
    {
        Thread.Sleep(seconds * 1000);
    }
    
    void HowManyProductDoYouWantToCrawl()
    {
        Console.WriteLine(
            "---------------------------------------------------------------------------------------------------------\n"+
            "Please enter amount of the product that you want to crawl " +
            "or if you want to crawl all products enter 'All'");
        
        do
        {
            crawlRequestAmount = Console.ReadLine().Trim();
    
        } while (string.IsNullOrEmpty(crawlRequestAmount));
        
        
    }

    void CreateExcelSheet(List<Product> products)
    {
        try
        {
            //var newFile = new FileInfo("Target File Path");
            using (var package = new ExcelPackage(/*newFile*/))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "IsOnSale";
                worksheet.Cells[1, 3].Value = "Price";
                worksheet.Cells[1, 4].Value = "Sale Price";
                worksheet.Cells[1, 5].Value = "Picture";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].Name;
                    worksheet.Cells[i + 2, 2].Value = products[i].IsOnSale;
                    worksheet.Cells[i + 2, 3].Value = products[i].Price;
                    worksheet.Cells[i + 2, 4].Value = products[i].SalePrice;
                    worksheet.Cells[i + 2, 5].Value = products[i].Picture;
                }

                package.Save();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    async Task<TResponse> SendHttpPostRequest<TRequest, TResponse>(HttpClient httpClient, string url, TRequest payload)
    {
        var jsonPayload = JsonConvert.SerializeObject(payload);
        
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, httpContent);
        
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        
        var responseObject = JsonConvert.DeserializeObject<TResponse>(jsonResponse);

        return responseObject;
    }
    
    CrawlerLogDto CreateLog(string message) => new CrawlerLogDto(message);
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}

