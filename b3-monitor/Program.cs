using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.Eventing.Reader;

namespace b3_monitor
{
    public class Program
    {
        // Args: string, float, float, string?
        //       token, ceiling, floor, frequency
        public static void Main(string[] args)
        {
            string Token = args[0]; // Stock token
            float SellPrice = float.Parse(args[1]); // IF the price goes above this, sell the stock
            float BuyPrice = float.Parse(args[2]); // If the price goes below this, buy the stock
            int MinutesBetweenChecks = Frequency(args);

            if (BuyPrice > SellPrice || BuyPrice < 0)
            {
                throw new ArgumentOutOfRangeException(paramName: "args[1], args[2]", message: "Por favor, informe um intervalo válido!");
            }

            while (true)
            {
                Console.WriteLine($"Consultando preço do ativo {Token}");
                Stock? tokenInfo = Stock.GetCurrentPrice(Token);
                if (tokenInfo != null)
                {
                    Console.WriteLine($"Preço consultado! Atual: {tokenInfo.CurrentPrice}");
                    if (tokenInfo.CurrentPrice > SellPrice)
                    {
                        Console.WriteLine($"Valor maior do que o mínimo de venda:{SellPrice}. Enviando e-mail...");
                        SendEmail(tokenInfo, "venda");
                    }
                    else if (tokenInfo.CurrentPrice < BuyPrice)
                    {
                        Console.WriteLine($"Valor menor do que o máximo de compra: {BuyPrice}. Enviando e-mail...");
                        SendEmail(tokenInfo, "compre");
                    } else
                    {
                        Console.WriteLine("Valor dentro do intervalo de monitoramento.");
                    }
                } else
                {
                    throw new ArgumentException(paramName: "Token", message: $"Ativo {Token} não encontrado! Por favor, informe um ativo válido.");
                }
                Thread.Sleep(MinutesBetweenChecks * 60 * 1000); // 1 minute by default
            }
        }

        /// <summary>
        /// Send an email to the user
        /// </summary>
        /// <param name="token"> Stock Token</param>
        /// <param name="action"> Either "venda" or "compre" depending on the price</param>
        /// <param name="price"> Current price of the stock. </param>
        public static void SendEmail(Stock token, string action)
        {
            #region Assemble message
            MailboxAddress from = new(
                ConfigurationManager.AppSettings["from"],
                ConfigurationManager.AppSettings["fromAddress"]
            );

            MailboxAddress to = new(
                ConfigurationManager.AppSettings["recipient"],
                ConfigurationManager.AppSettings["recipient"]
            );

            MimeMessage email = new()
            {
                Subject = $"Ação recomendada para o ativo {token.Symbol}",
                Body = new TextPart()
                {
                    Text = $"Uma consulta do ativo {token.Symbol} " +
                    $"retornou o preço {token.CurrentPrice}. " +
                    $"Aconselhamos que {action} o ativo."
                }
            };

            email.From.Add(from);
            email.To.Add(to);
            #endregion

            using (var smtp = new SmtpClient()) {
                try
                {
                    #region Connect and Authenticate
                    smtp.Connect(
                        host: ConfigurationManager.AppSettings["host"],
                        port: Int32.Parse(ConfigurationManager.AppSettings["port"]),
                        useSsl: Boolean.Parse(ConfigurationManager.AppSettings["enableSsl"])
                    );

                    smtp.Authenticate(
                        userName: ConfigurationManager.AppSettings["userName"],
                        password: ConfigurationManager.AppSettings["password"]
                    );
                    #endregion

                    smtp.Send(email);
                    Console.WriteLine("E-mail enviado!");

                    smtp.Disconnect(true);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                } 

            }
        }

        /// <summary>
        /// Control structure to define minutes between API calls based on user-given parameter
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Frequency(string[] args)
        {
            return (args.Length == 4 ? args[3] : "") switch
            {
                "M5" => 5,
                "M10" => 10,
                "M15" => 15,
                "M30" => 30,
                "H1" => 1 * 60,
                "H2" => 2 * 60,
                "H3" => 3 * 60,
                "H4" => 4 * 60,
                _ => 1,
            };
        }
    }

    public class Stock()
    {
        [JsonPropertyName("currentPrice")]
        public required float CurrentPrice { get; set; }
        
        [JsonPropertyName("symbol")]
        public required string Symbol { get; set; }

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

    }
}

