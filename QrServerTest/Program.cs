// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var server = new QrCodeServer();

        server.StartServer();

    }
}

public class QrCodeServer
{
    private HttpListener _listener;
    private Thread _listenerThread;

    public void StartServer()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://+:8080/");
        _listener.Start();
        _listenerThread = new Thread(HandleRequests);
        _listenerThread.Start();
    }

    private void HandleRequests()
    {
        while (_listener.IsListening)
        {
            Thread.Sleep(1000);

            var context = _listener.GetContext();
            var request = context.Request;
            var response = context.Response;

            try 
            {
                // Check if the request is a GET and the path starts with /scan/
                if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/scan/"))
                {
                    // Extract the QR code data from the URL path
                    var qrCodeData = request.Url.AbsolutePath.Substring(6); // Removes '/scan/'

                    // URL decode the QR code data in case it contains special characters
                    qrCodeData = WebUtility.UrlDecode(qrCodeData);

                    // Process the QR code data
                    ProcessQrCodeData(qrCodeData);

                    // Send a response back to the iPhone
                    var responseString = "QR code received and processed.";
                    var buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            } 
            catch 
            {
                response.Close();

            }


            response.Close();
        }
    }

    private void ProcessQrCodeData(string data)
    {
        // Implement your QR code data processing logic here
        Console.WriteLine(data);
    }

    public void StopServer()
    {
        _listener.Stop();
        _listenerThread.Abort();
    }
}