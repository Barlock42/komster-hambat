using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace KomsterHambatAutoClicker
{
    public class AutoClicker
    {
        private readonly string _apiUrl;
        private readonly string _apiUrlTap;
        private readonly string _authorizationToken;

        public AutoClicker()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _apiUrl = configuration["ApiSettings:ApiUrl"];
            _apiUrlTap = configuration["ApiSettings:ApiUrlTap"];
            _authorizationToken = configuration["ApiSettings:AuthorizationToken"];
        }

        private static readonly Random RandomGenerator = new Random();

        public async Task RunAutoClickerAsync()
        {
            while (true)
            {
                var gameSyncResponse = await SyncGameAsync();

                if (gameSyncResponse != null)
                {
                    var clickerUser = gameSyncResponse.ClickerUser;
                    int availableTaps = clickerUser.AvailableTaps;
                    int maxTaps = clickerUser.MaxTaps;
                    int tapsRecoverPerSec = clickerUser.TapsRecoverPerSec;
                    int earnPerTap = clickerUser.EarnPerTap;

                    int maxTapsToSend = availableTaps / earnPerTap;

                    if (maxTapsToSend > 0)
                    {
                        int tapsToSend = RandomGenerator.Next(1, maxTapsToSend + 1);

                        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                        var tapResponse = await SendTapRequestAsync(tapsToSend, tapsToSend * earnPerTap, timestamp);

                        if (tapResponse != null)
                        {
                            Console.WriteLine($"Sent request for {tapsToSend} taps. Tap request was successful!");
                        }
                        else
                        {
                            int retryDelay = RandomGenerator.Next(3000, 5001);
                            Console.WriteLine($"Failed to send tap request. Retrying in {retryDelay / 1000} seconds.");
                            await Task.Delay(retryDelay);
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No available taps. Waiting for partial energy recovery.");
                    }

                    // Ожидаем частичного восстановления энергии
                    int maxRecoveryTime = (maxTaps - availableTaps) / tapsRecoverPerSec;
                    int randomRecoveryTime = RandomGenerator.Next(1, Math.Max(1, maxRecoveryTime - 2));
                    Console.WriteLine($"Waiting for {randomRecoveryTime} seconds for partial energy recovery...");
                    await Task.Delay(randomRecoveryTime * 1000);
                }
                else
                {
                    int retryDelay = RandomGenerator.Next(5000, 10001);
                    Console.WriteLine($"Failed to sync with the server. Retrying in {retryDelay / 1000} seconds.");
                    await Task.Delay(retryDelay);
                }
            }
        }

        // Метод для отправки sync запроса
        public async Task<GameSyncResponse> SyncGameAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationToken);

                    HttpResponseMessage response = await client.GetAsync(_apiUrl);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    GameSyncResponse gameSyncResponse = JsonSerializer.Deserialize<GameSyncResponse>(responseBody);

                    if (gameSyncResponse != null)
                    {
                        Console.WriteLine($"User ID: {gameSyncResponse.ClickerUser.Id}");
                        Console.WriteLine($"Balance Coins: {gameSyncResponse.ClickerUser.BalanceCoins}");
                        Console.WriteLine($"Available Taps: {gameSyncResponse.ClickerUser.AvailableTaps}");
                        Console.WriteLine($"Last Sync Update: {gameSyncResponse.ClickerUser.LastSyncUpdate}");
                        Console.WriteLine($"Max Taps: {gameSyncResponse.ClickerUser.MaxTaps}");
                        Console.WriteLine($"Earn Per Tap: {gameSyncResponse.ClickerUser.EarnPerTap}");
                        Console.WriteLine($"Taps Recover Per Sec: {gameSyncResponse.ClickerUser.TapsRecoverPerSec}");
                    }

                    return gameSyncResponse;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
            }
        }

        // Метод для отправки tap запроса
        public async Task<GameSyncResponse> SendTapRequestAsync(int count, int availableTaps, long timestamp)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationToken);

                    var tapData = new
                    {
                        count,
                        availableTaps,
                        timestamp
                    };
                    string jsonContent = JsonSerializer.Serialize(tapData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(_apiUrlTap, content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    GameSyncResponse gameSyncResponse = JsonSerializer.Deserialize<GameSyncResponse>(responseBody);

                    return gameSyncResponse;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
            }
        }
    }
}
