// See https://aka.ms/new-console-template for more information
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace b3_monitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string Token = args[0]; // Stock token
            float RangeCeiling = float.Parse(args[1]); // IF the price goes above this, sell the stock
            float RangeFloor = float.Parse(args[2]); // If the price goes below this, buy the stock
            while (true)
            {
                Stock tokenInfo = GetCurrentPrice(Token);
                if (tokenInfo != null)
                {
                    if (tokenInfo.Price > RangeCeiling)
                    {
                        SendEmail(tokenInfo, "vender");
                    }
                    else if (tokenInfo.Price < RangeFloor)
                    {
                        SendEmail(tokenInfo, "comprar");
                    }
                }
                Thread.Sleep(15 * 60 * 1000); // 15 minutes
            }
        }

        /// <summary>
        /// Get the current price of a stock
        /// </summary>
        /// <param name="token"> Stock token </param>
        /// <returns></returns>
        public static Stock GetCurrentPrice(string token)
        {
            // Check the API for the current value of the Token
            return new Stock(token, 28.59f, "2021-09-01 10:00:00");
        }

        /// <summary>
        /// Send an email to the user
        /// </summary>
        /// <param name="token"> Stock Token</param>
        /// <param name="action"> Either "vender" or "comprar" depending on the price</param>
        /// <param name="price"> Current price of the stock. </param>
        public static void SendEmail(Stock token, string action)
        {
            // Send an email to the user
            string emailBody = $"Uma consulta do ativo {token.Token} " +
                $"retornou o preço {token.Price}" +
                $" na data e hora {token.LastUpdated}. " +
                $"Aconselhamos que {action} o ativo.";
            string emailSubject = $"Ação recomendada para o ativo {token.Token}";
            // Send email to user
        }
    }

    public class Stock(string token, float price, string lastUpdated)
    {
        public float Price { get; set; } = price;
        public string Token { get; set; } = token;
        public DateTime LastUpdated { get; set; } = DateTime.Parse(lastUpdated);
    }
}

