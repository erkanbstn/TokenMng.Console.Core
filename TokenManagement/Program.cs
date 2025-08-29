using Newtonsoft.Json;

public class Program
{
    private static string _accessToken = null;
    private static DateTime _tokenExpiryTime;
    private static readonly HttpClient _client = new HttpClient();
    private static int _requestCount = 0; 

    static async Task Main(string[] args)
    {
        // İlk token kontrolü yapılır.
        if (TokenNeedsRefreshing())
        {
            await RefreshTokenAsync();
        }

        while (true)
        {
            if (TokenNeedsRefreshing())
            {
                await RefreshTokenAsync();
            }

            _requestCount++;

            // Eğer 5'ten fazla istek yapıldıysa hata fırlat.
            if (_requestCount > 5)
            {
                Console.WriteLine("Hata: 5'ten fazla istek yapıldı. Token yenilenmeyecek ve işlem durdurulacak.");
                break; 
            }

            await GetOrderListAsync();

            // 5 dakika bekleme süresi
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }

    // Token süre kontrolü
    private static bool TokenNeedsRefreshing()
    {
        // Token yoksa veya süresi dolmuşsa yenilenmesi gerekiyor
        if (string.IsNullOrEmpty(_accessToken) || DateTime.Now >= _tokenExpiryTime)
        {
            return true;
        }
        return false;
    }

    // Token yenileme işlemi
    private static async Task RefreshTokenAsync()
    {
        try
        {
            var requestBody = new StringContent("grant_type=client_credentials", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            // Token isteği yapılacak API URL
            string tokenUrl = "https://api.example.com/token";  // Test URL

            HttpResponseMessage response = await _client.PostAsync(tokenUrl, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                _accessToken = tokenResponse.access_token;
                _tokenExpiryTime = DateTime.Now.AddSeconds(tokenResponse.expires_in);

                Console.WriteLine("Yeni token alındı.");
            }
            else
            {
                Console.WriteLine("Token alınamadı.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token yenileme hatası: {ex.Message}");
        }
    }

    // Sipariş listesi requesti
    private static async Task GetOrderListAsync()
    {
        try
        {
            string orderListUrl = "https://api.example.com/orders";  // Test URL

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            HttpResponseMessage response = await _client.GetAsync(orderListUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Sipariş listesi alındı:\n" + responseBody);
            }
            else
            {
                Console.WriteLine("Sipariş listesi alınamadı.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sipariş listesi hatası: {ex.Message}");
        }
    }

    // Token Response Modeli
    public class TokenResponse
    {
        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("expires_in")]
        public int expires_in { get; set; }

        [JsonProperty("access_token")]
        public string access_token { get; set; }
    }
}
