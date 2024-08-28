using System;
using System.Threading.Tasks;

namespace KomsterHambatAutoClicker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Komster Hambat AutoClicker...");

            AutoClicker autoClicker = new AutoClicker();

            await autoClicker.RunAutoClickerAsync();
        }
    }
}
