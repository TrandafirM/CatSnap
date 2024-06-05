using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CatSnap
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string outputFilePath = null;
            string textToOverlay = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-o" && i + 1 < args.Length)
                {
                    outputFilePath = args[i + 1];
                    i++;
                }
                else if (args[i] == "-t" && i + 1 < args.Length)
                {
                    textToOverlay = args[i + 1];
                    i++;
                }
            }

            if (string.IsNullOrEmpty(outputFilePath))
            {
                Console.WriteLine("Without a path, where should the cat go? Please provide an output file path with -o.");
                return;
            }
            if(textToOverlay == ""){
                Console.WriteLine("The cat tried to speak but it couldn’t. Maybe you forgot something?");
            }
            string baseUrl = "https://cataas.com/cat";
            if (!string.IsNullOrEmpty(textToOverlay))
            {
                baseUrl += $"/says/{Uri.EscapeDataString(textToOverlay)}";
            }
            try
            {
                var handler = new HttpClientHandler();
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl);
                    response.EnsureSuccessStatusCode();
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    await File.WriteAllBytesAsync(outputFilePath, imageBytes);
                    Console.WriteLine($"Caught the cat! Check your folder for the furry proof at {outputFilePath}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Oops! I couldn't fetch your cat because it’s probably stuck in a tree (or the internet is down). {e.Message}");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Unable to save the cat picture, maybe the disk is as full as a cat on a lazy Sunday afternoon. {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went weirdly wrong! It's not you, it's me. Actually, it’s probably the cat. {e.Message}");
            }
        }
    }
}