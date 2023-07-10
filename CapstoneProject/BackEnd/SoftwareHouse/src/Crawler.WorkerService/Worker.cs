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

namespace Crawler.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _crawlerHubUrl = "https://localhost:7015/Hubs/CrawlerHub";
    private readonly HubConnection _hubConnection;
    
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

    const string homePageUrl = "https://4teker.net/";
    const string orderAddUrl = "https://localhost:7015/api/Orders/Add";
    const string orderEventsAddUrl = "https://localhost:7015/api/OrderEvents/Add";
    const string productsAddUrl = "https://localhost:7015/api/Products/Add";
    //const string crawlerHubUrl = "https://localhost:7015/Hubs/CrawlerHub";

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
        _logger = logger;
        
        new DriverManager().SetUpDriver(new ChromeConfig());
    
        driver = new ChromeDriver();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_crawlerHubUrl)
            .WithAutomaticReconnect()
            .Build();
        
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hubConnection.On<int, string>("SendOrderNotificationAsync", async (productNumber, productCrawlType) =>
        {
            try
            {
                crawlRequestAmount = productNumber.ToString();
                productType = productCrawlType;

                await Crawler();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing SendOrderNotificationAsync event");
            }
        });
        
        await _hubConnection.StartAsync(stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    async Task Crawler()
    {
        try
        {
            while (true)
            {
                driver.Navigate().GoToUrl(homePageUrl);
                
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

                    await SendHttpPostRequest<ProductAddCommand, object>(httpClient, productsAddUrl, productAddRequest);

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

        await SendHttpPostRequest<OrderEventAddCommand, object>(httpClient, orderEventsAddUrl, orderEventAddRequest);

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

        await SendHttpPostRequest<OrderAddCommand, object>(httpClient, orderAddUrl, orderAddRequest);

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