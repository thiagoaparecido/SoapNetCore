﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;




class Program
{
    public static async Task<XDocument> CallWebServiceAsync()
    {
        var url = @"http://www.webservicex.net/globalweather.asmx";
        var action = @"http://www.webserviceX.NET/GetWeather";
        var result = await CreateWebRequestAsync(url, action).ConfigureAwait(false);
        var resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        return XDocument.Parse(resultString);
    }

    public static XDocument CreateSoapEnvelope()
    {
        var envelopeContent =
            @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:web=""http://www.webserviceX.NET"">
   <soapenv:Header>
<wsse:Security xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd' xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
    <wsu:Timestamp wsu:Id='Timestamp-2016-12-18T20:11:27Z'>
        <wsu:Created>2016-12-18T20:11:27Z</wsu:Created>
        <wsu:Expires>2016-12-18T20:21:27Z</wsu:Expires>
    </wsu:Timestamp>
    <wsse:UsernameToken xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd' wsu:Id='SecurityToken-2016-12-18T20:11:27Z'>
        <wsse:Username>MyUsuario</wsse:Username>
        <wsse:Password>MyPass</wsse:Password>
        <wsu:Created>2016-12-18T20:11:27Z</wsu:Created>
    </wsse:UsernameToken>
</wsse:Security>
</soapenv:Header>
   <soapenv:Body>
      <web:GetWeather>
         <!--Optional:-->
         <web:CityName>?</web:CityName>
         <!--Optional:-->
         <web:CountryName>?</web:CountryName>
      </web:GetWeather>
   </soapenv:Body>
</soapenv:Envelope>";

        return XDocument.Parse(envelopeContent);
    }

    public static void Main(string[] args)
    {
        //try
        //{
        //    var result = CallWebServiceAsync().Result;
        //    Console.WriteLine(result.ToString());
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //}
        var result = CallWebServiceAsync().Result;
        Console.WriteLine(result.ToString());
        Console.WriteLine("End");
        Console.Read();
    }
    private static async Task<HttpResponseMessage> CreateWebRequestAsync(string url, string action)
    {

        HttpContent message = new StringContent(
                                   CreateSoapEnvelope().ToString(),
                                   Encoding.UTF8,
                                "text/xml");



        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
        };



        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

        request.Headers.AcceptEncoding.ParseAdd("gzip,deflate");
        request.Headers.AcceptCharset.ParseAdd("utf-8");
        request.Headers.Date = DateTime.Now;
        request.Headers.Add("SOAPAction", action);
        request.Headers.Add("Keep-Alive", "115");
        request.Headers.Add("Connection", "keep-alive");




        // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // comparable to modern browsers




        using (var handler = new HttpClientHandler())
        using (var client = new HttpClient(handler))
        {

            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            
            return await client.PostAsync(url, message).ConfigureAwait(false);
        }
    }

}