using System.Text.Json.Serialization;

namespace KomsterHambatAutoClicker
{
    public class GameSyncResponse
    {
        [JsonPropertyName("clickerUser")]
        public ClickerUser ClickerUser { get; set; }
    }

    public class ClickerUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("balanceCoins")]
        public double BalanceCoins { get; set; }

        [JsonPropertyName("availableTaps")]
        public int AvailableTaps { get; set; }

        [JsonPropertyName("lastSyncUpdate")]
        public long LastSyncUpdate { get; set; }

        [JsonPropertyName("maxTaps")]
        public int MaxTaps { get; set; }

        [JsonPropertyName("earnPerTap")]
        public int EarnPerTap { get; set; }

        [JsonPropertyName("tapsRecoverPerSec")]
        public int TapsRecoverPerSec { get; set; }
    }
}
