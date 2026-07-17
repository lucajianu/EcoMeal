using EcoMeal.Site.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace EcoMeal.Site.Services;

// tine conexiunea SignalR catre backend pe durata circuitului Blazor;
// componentele se aboneaza la evenimente ca sa afiseze alerte si sa-si reincarce datele
public class OrderNotificationService : IAsyncDisposable
{
    private HubConnection? _connection;

    public event Action<OrderNotificationModel>? OrderPlaced;
    public event Action<OrderNotificationModel>? OrderStatusChanged;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public async Task StartAsync(string token)
    {
        if (_connection is not null)
        {
            return;
        }

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7171/hubs/orders", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                // certificatul de dev nu e de incredere pentru apelurile server-to-server
                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is HttpClientHandler clientHandler)
                    {
                        clientHandler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    }
                    return handler;
                };
                options.WebSocketConfiguration = ws =>
                    ws.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<OrderNotificationModel>("OrderPlaced", n => OrderPlaced?.Invoke(n));
        _connection.On<OrderNotificationModel>("OrderStatusChanged", n => OrderStatusChanged?.Invoke(n));

        await _connection.StartAsync();
    }

    public async Task StopAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    public async ValueTask DisposeAsync() => await StopAsync();
}
