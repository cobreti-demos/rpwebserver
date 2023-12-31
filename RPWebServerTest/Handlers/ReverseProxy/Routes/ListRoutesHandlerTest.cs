using AutoMapper;
using Moq;
using RPWebServer.Commands.ReverseProxy.Routes;
using RPWebServer.Handlers.ReverseProxy.Routes;
using RPWebServer.Models.RouteConfig;
using RPWebServer.Services.ReverseProxy;
using RPWebServer.Services.ReverseProxy.Config.Route;

namespace RPWebServerTest.Handlers.ReverseProxy.Routes;

public class ListRoutesHandlerTest
{
    public Mock<IRoutesConfigProvider> RoutesConfigProviderMock { get; } = new();
    public Mock<IMapper> MapperMock { get; } = new();

    [Fact]
    public void Construction()
    {
        var handler = new ListRoutesHandler(
            RoutesConfigProviderMock.Object,
            MapperMock.Object);

        Assert.Equal(RoutesConfigProviderMock.Object, handler.RoutesConfigProvider);
        Assert.Equal(MapperMock.Object, handler.Mapper);
    }

    [Fact]
    public async void Handle()
    {
        var mutableRouteA = new MutableRouteConfig("testA");
        var dtoRouteA = new RouteConfigDto()
        {
            RouteId = "testA"
        };
        
        RoutesConfigProviderMock
            .Setup(it => it.ListRoutes())
            .Returns(new List<MutableRouteConfig>()
            {
                mutableRouteA
            });

        MapperMock
            .Setup(it => it.Map<RouteConfigDto>(mutableRouteA))
            .Returns(dtoRouteA);
        
        var handler = new ListRoutesHandler(
            RoutesConfigProviderMock.Object,
            MapperMock.Object);

        var response = await handler.Handle(
            new ListRoutesRequest(),
            new CancellationToken());

        var dtoList = response.Value;
        
        Assert.NotNull(response);
        Assert.True(response.Succeeded);
        Assert.NotNull(dtoList);
        Assert.Single(dtoList);
        Assert.Equal("testA", dtoList.First().RouteId);
    }
}
