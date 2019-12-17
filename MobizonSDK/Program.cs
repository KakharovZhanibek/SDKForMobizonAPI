using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MobizonSDK
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mobizonService = new MobizonService("x");
            var balance = await mobizonService.GetCurrentBalance();

            string numb = "+x";

            numb = HttpUtility.UrlEncode(numb, Encoding.UTF8);

            var x = mobizonService.SendSmsMessage(numb, "hello");

            await x;
            Console.WriteLine(balance);

            Console.ReadLine();
        }
    }
}
