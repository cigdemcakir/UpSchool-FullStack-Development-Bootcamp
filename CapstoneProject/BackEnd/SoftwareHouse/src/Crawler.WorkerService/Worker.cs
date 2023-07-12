using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using Application.Features.Orders.Commands.Add;
using Domain.Enums;
using Application.Features.OrderEvents.Commands.Add;
using Application.Features.Products.Commands.Add;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Dtos;
using Domain.Entities;
using OpenQA.Selenium.DevTools;
using Serilog;
using Log = Serilog.Log;

namespace Crawler.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HubConnection _hubConnection;
    
    #region Locators

    readonly By _productsLocator = By.CssSelector(".card.h-100");
    readonly By _pageNumberLocator = By.CssSelector(".page-link.page-number");
    readonly By _productName = By.CssSelector(".product-name");
    readonly By _productPrice = By.CssSelector(".price");
    readonly By _pictureLink = By.CssSelector(".card-img-top");
    readonly By _onSale = By.CssSelector(".onsale");
    readonly By _productOnSalePrice = By.CssSelector(".sale-price");

    #endregion

    #region Urls

    const string HomePageUrl = "https://4teker.net/";
    const string OrderAddUrl = "https://localhost:7015/api/Orders/Add";
    const string OrderEventsAddUrl = "https://localhost:7015/api/OrderEvents/Add";
    const string ProductsAddUrl = "https://localhost:7015/api/Products/Add";
    const string CrawlerHubUrl = "https://localhost:7015/Hubs/CrawlerHub";

    #endregion
    
    string productType;
    string? salePrice = null;
    string crawlRequestAmount;
    int crawledProductCount;

    private IWebDriver? driver;
    
    HttpClient httpClient = new HttpClient();

    OrderAddCommand orderAddRequest = new OrderAddCommand();

    OrderEventAddCommand orderEventAddRequest = new OrderEventAddCommand();

    public Worker(ILogger<Worker> logger)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()  // İsteğe bağlı
            .WriteTo.Console()
            .CreateLogger();
        
        _logger = logger;
        
        Log.Information("Before");

        new DriverManager().SetUpDriver(new ChromeConfig());
        
        Log.Information("After");
    
        //driver = new ChromeDriver();
        driver = new ChromeDriver();
        
        Thread.Sleep(5);
        
        Log.Information("hi");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(CrawlerHubUrl)
            .WithAutomaticReconnect()
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hubConnection.On<int, string>("SendOrderNotificationAsync", async (productNumber, productCrawlType) =>
        {
            try
            {
                //_logger.LogInformation("Received SendOrderNotificationAsync message with productNumber {productNumber} and productCrawlType {productCrawlType}", productNumber, productCrawlType);
                Log.Information("Received SendOrderNotificationAsync message with productNumber {productNumber} and productCrawlType {productCrawlType}", productNumber, productCrawlType);
                
                crawlRequestAmount = productNumber.ToString();
                productType = productCrawlType;

                await Crawler();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing SendOrderNotificationAsync event");
            }
            
        });
        Log.Information("execute");
        
        await _hubConnection.StartAsync(stoppingToken).ContinueWith(task => 
        {
            if (task.IsFaulted)
            {
                Serilog.Log.Information(task.Exception, "An error occurred while connecting to the hub");
            }
            else
            {
                Log.Information("Connected to the hub successfully");
            }
        });
        
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("....");
            await Task.Delay(1000, stoppingToken);
        }
    }

    async Task Crawler()
    {
        Log.Information("Crawler method started.");
        
        try
        {
            bool continueCrawling = true;
            
            while (continueCrawling)
            {
                driver.Navigate().GoToUrl(HomePageUrl);
                
                Sleep(3);
                crawledProductCount = 0;
        
                //await _hubConnection.StartAsync();

                CreateOrder(productType);

                CreateOrderEvent(OrderStatus.BotStarted);

                
                Sleep(3);

                await _hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Navigated to UpStorage Shop",Guid.Empty));

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

                CreateOrderEvent(OrderStatus.CrawlingCompleted);

                await _hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Mission Accomplished!",Guid.Empty));

                Sleep(2);

                CreateOrderEvent(OrderStatus.OrderCompleted);

                // Console.WriteLine("Do you want to continue crawling? (y/n)");
                // var answer = Console.ReadLine();
                //
                // if (answer?.ToLower() == "y")
                //     await _hubConnection.StopAsync();
                // else
                //     break;
                Console.WriteLine("Do you want to continue crawling? (y/n)");
                var answer = Console.ReadLine();
            
                if (answer?.ToLower() == "y")
                    continueCrawling = true;
                else
                    continueCrawling = false;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    async void CrawlProducts(int? requestNumber = null)
    {
        List<Product> productList = new List<Product>();

        var pageLinks = driver.FindElements(_pageNumberLocator)
                              .Select(x => x.GetAttribute("href"))
                              .ToList();

        driver.Navigate().GoToUrl(pageLinks[0]);

        for (var currentPage = 1; currentPage <= pageLinks.Count; currentPage++)
        {
            var products = driver.FindElements(_productsLocator);

            foreach (var productElement in products)
            {
                if (requestNumber.HasValue && crawledProductCount == requestNumber)
                    break;

                var isOnSale = productElement.FindElements(_onSale).Count != 0;

                if ((productType.ToLower() == "a")
                    || (productType.ToLower() == "b" && isOnSale)
                    || (productType.ToLower() == "c" && !isOnSale))
                {
                    var name = productElement.FindElement(_productName).Text;
                    var price = productElement.FindElement(_productPrice).Text;
                    var picture = productElement.FindElement(_pictureLink).GetAttribute("src");

                    if (isOnSale)
                    {
                        salePrice = productElement.FindElement(_productOnSalePrice).Text;
                        salePrice = salePrice.Replace("$", "");
                    }

                    price = price.Replace("$", "");

                    if (crawledProductCount == 1) await _hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"1. product were crawled",Guid.Empty));

                    crawledProductCount++;

                    var productAddRequest = new ProductAddCommand()
                    {
                        OrderId = orderAddRequest.Id,
                        Name = name,
                        IsOnSale = isOnSale,
                        Price = decimal.Parse(price),
                        SalePrice = decimal.Parse(salePrice),
                        Picture = picture,
                    };

                    await SendHttpPostRequest<ProductAddCommand, object>(httpClient, ProductsAddUrl, productAddRequest);

                    await _hubConnection.InvokeAsync("SendProductNotificationAsync", CreateLog(
                        $"Product Name : {name}" + "   -    " +
                        $"Is On Sale ? :   {isOnSale}" + "   -    " +
                        $"Product Price :   {price}" + "   -    " +
                        $"Product Sale Price :   {salePrice}" + "   -    " +
                        $"Product Picture :   {picture}"+ "   -    "+  
                        $"OrderId :   {productAddRequest.OrderId}",Guid.NewGuid()));

                    Console.WriteLine($"Name: {name} -- OnSale: {isOnSale} -- Price: {price} -- Sale Price: {salePrice} -- Path: {picture}");
                }
            }

            await _hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog($"{crawledProductCount}. product were crawled",Guid.Empty));

            if (requestNumber.HasValue && crawledProductCount == requestNumber)
                break;

            if (currentPage < pageLinks.Count && !string.IsNullOrEmpty(pageLinks[currentPage]))
            {
                driver.Navigate().GoToUrl(pageLinks[currentPage]);
            }
        }
        CreateOrderEvent(OrderStatus.CrawlingCompleted);

        await _hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Mission Accomplished!",Guid.Empty));

        Sleep(2);

        CreateOrderEvent(OrderStatus.OrderCompleted);
    }

    async void CreateOrderEvent(OrderStatus orderStatus)
    {
        orderEventAddRequest = new OrderEventAddCommand()
        {
            OrderId = orderAddRequest.Id,
            Status = orderStatus,
        };

        await SendHttpPostRequest<OrderEventAddCommand, object>(httpClient, OrderEventsAddUrl, orderEventAddRequest);

        await _hubConnection.InvokeAsync("SendLogNotificationAsync", CreateLog("Order Status : " + orderEventAddRequest.Status.ToString(),Guid.Empty));
    }

    async void CreateOrder(string productType)
    {
        switch (productType.ToLower())
        {
            case "all":
                orderAddRequest = new OrderAddCommand()
                {
                    Id = Guid.NewGuid(),
                    ProductCrawlType = ProductCrawlType.All
                };
                break;

            case "discount":
                orderAddRequest = new OrderAddCommand()
                {
                    Id = Guid.NewGuid(),
                    ProductCrawlType = ProductCrawlType.OnDiscount
                };
                break;

            case "nondiscount":
                orderAddRequest = new OrderAddCommand()
                {
                    Id = Guid.NewGuid(),
                    ProductCrawlType = ProductCrawlType.NonDiscount
                };
                break;
        }

        await SendHttpPostRequest<OrderAddCommand, object>(httpClient, OrderAddUrl, orderAddRequest);

        //await _hubConnection.InvokeAsync("SendOrderNotificationAsync", CreateLog($"Order Id : {orderAddRequest.Id}  -  Crawl Type : {orderAddRequest.ProductCrawlType.ToString()}",Guid.Empty));
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
            "---------------------------------------------------------------------------------------------------------\n" +
            "Please enter amount of the product that you want to crawl " +
            "or if you want to crawl all products enter 'All'");

        do
        {
            crawlRequestAmount = Console.ReadLine().Trim();

        } while (string.IsNullOrEmpty(crawlRequestAmount));

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

    CrawlerLogDto CreateLog(string message,Guid id) => new CrawlerLogDto(message,id);
}