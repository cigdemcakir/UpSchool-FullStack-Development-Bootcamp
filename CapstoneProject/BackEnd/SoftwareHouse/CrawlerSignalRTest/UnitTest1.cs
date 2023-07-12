using FakeItEasy;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace CrawlerSignalRTest;

public class SignalRTest
{
    private CrawlerHub _hub;
    private IHubCallerClients _fakeClients;

    public SignalRTest()
    {
        _fakeClients = A.Fake<IHubCallerClients>();
        _hub = new CrawlerHub()
        {
            Clients = _fakeClients,
        };
    }

    [Fact]
    public async Task TestSendOrderNotificationAsync()
    {
        int expectedProductNumber = 5;
        string expectedProductCrawlType = "TestProductType";
        CancellationToken cancellationToken = CancellationToken.None;

        // SendCoreAsync metodunu taklit ediyoruz
        A.CallTo(() => _fakeClients.All.SendCoreAsync(
                A<string>.That.IsEqualTo("SendOrderNotificationAsync"), 
                A<object[]>.That.Matches(x => (int)x[0] == expectedProductNumber && (string)x[1] == expectedProductCrawlType), 
                A<CancellationToken>.That.IsEqualTo(cancellationToken)))
            .Returns(Task.CompletedTask);

        // Metodu tetikle
        await _hub.SendOrderNotificationAsync(expectedProductNumber, expectedProductCrawlType);

        // Doğru parametrelerle çağrıldığını onayla
        A.CallTo(() => _fakeClients.All.SendCoreAsync(
            A<string>.Ignored,
            A<object[]>.Ignored, 
            A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }

}