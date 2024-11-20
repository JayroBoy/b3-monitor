using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace b3_monitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string Token = "PETR4"; //args[0]; // Stock token
            float RangeFloor = 37.6f;//float.Parse(args[1]); // If the price goes below this, buy the stock
            float RangeCeiling = 38.7f;//float.Parse(args[2]); // IF the price goes above this, sell the stock
            int MinutesBetweenChecks = Frequency(args);

            while (true)
            {
                Console.WriteLine($"Consultando preço do ativo {Token}");
                Stock? tokenInfo = GetCurrentPrice(Token);
                if (tokenInfo != null)
                {
                    Console.WriteLine($"Preço consultado! Atual: {tokenInfo.CurrentPrice}");
                    if (tokenInfo.CurrentPrice > (float)RangeCeiling)
                    {
                        Console.WriteLine("Valor dentro do intervalo de venda. Enviando e-mail...");
                        SendEmail(tokenInfo, "vender");
                    }
                    else if (tokenInfo.CurrentPrice < (float)RangeFloor)
                    {
                        Console.WriteLine("Valor dentro do intervalo de compra. Enviando e-mail...");
                        SendEmail(tokenInfo, "comprar");
                    }
                } else
                {
                    Console.WriteLine($"Ativo {Token} não encontrado! Por favor, informe um ativo válido.");
                    break;
                }
                Thread.Sleep(MinutesBetweenChecks * 60 * 1000); // 15 minutes by default
            }
        }

        /// <summary>
        /// Get the current price of a stock
        /// </summary>
        /// <param name="token"> Stock token </param>
        /// <returns></returns>
        public static Stock? GetCurrentPrice(string token)
        {
            HttpClient client = new();
            var json = client.GetStringAsync($"https://felipemarinho.vercel.app/api/b3/prices/{token}").Result;
            Stock? result = JsonSerializer.Deserialize<Stock>(json);

            return result;
        }

        /// <summary>
        /// Send an email to the user
        /// </summary>
        /// <param name="token"> Stock Token</param>
        /// <param name="action"> Either "vender" or "comprar" depending on the price</param>
        /// <param name="price"> Current price of the stock. </param>
        public static void SendEmail(Stock token, string action)
        {
            MailAddress Recipient = new("jayrobneto@gmail.com"); // TODO: Retrieve info from configuration file
            string Subject = $"Ação recomendada para o ativo {token.Symbol}";
            string Body = $"Uma consulta do ativo {token.Symbol} " +
                $"retornou o preço {token.CurrentPrice}. " +
                $"Aconselhamos que {action} o ativo.";
            
            Console.WriteLine(Recipient.Address);
            Console.WriteLine(Subject);
            Console.WriteLine(Body);
        }

        /// <summary>
        /// Control structure to define minutes between API calls based on user-given parameter
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Frequency(string[] args)
        {
            switch (args.Length == 4 ? args[3] : "")
            {
                case "M5":
                    return 5;
                case "M10":
                    return 10;
                case "M15":
                    return 15;
                case "M30":
                    return 30;
                case "H1":
                    return 1 * 60;
                case "H2":
                    return 2 * 60;
                case "H3":
                    return 3 * 60;
                case "H4":
                    return 4 * 60;
                default:
                    return 15;
            }
        }
    }

    public class Stock()
    {
        [JsonPropertyName("currentPrice")]
        public required float CurrentPrice { get; set; }
        
        [JsonPropertyName("symbol")]
        public required string Symbol { get; set; }

    }
}

