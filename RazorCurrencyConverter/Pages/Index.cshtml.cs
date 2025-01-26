using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace RazorCurrencyConverter.Pages
{
    public class IndexModel : PageModel
    {
        public string getUrl = "https://www.cbr.ru/scripts/XML_daily.asp?";
        public decimal? Sum { get; private set; }
        public decimal? Result { get; private set; }
        public string? CurentFrom { get; private set; }
        public string? CurentTo { get; private set; }

        public List<string> Valutes = new List<string>()
        {
            "RUB", "USD", "BYN", "GBP", "AED", "EUR", "KZT"
        };
        public string? MessageError { get; set; }
        public async Task OnGet(decimal? sum, string from_, string to_)
        {
            try
            {
                if (sum != null && from_ != null && to_ != null)
                {
                    Sum = sum;
                    CurentFrom = from_;
                    CurentTo = to_;
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(getUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            string xmlResponse = await response.Content.ReadAsStringAsync();
                            XDocument xml = XDocument.Parse(xmlResponse);

                           decimal rateFrom = Convert.ToDecimal(GetRate(from_, xml));
                            
                           decimal rateTo = Convert.ToDecimal(GetRate(to_, xml));
                            if (rateFrom != 0 && rateTo != 0)
                            {
                                Result = sum * rateFrom / rateTo;
                            }   
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageError = e.Message;
            }
        }
        public string GetRate(string code, XDocument xml_)
        {
            string rate = "1";
            if (code != "RUB")
            {
                var element = xml_.Elements("ValCurs").Elements("Valute").FirstOrDefault(x => x.Element("CharCode").Value == code);
                if (element != null)
                {
                    rate = element.Elements("Value").FirstOrDefault().Value;
                }
                else
                {
                    MessageError = "Ошибка кода валюты!";
                    return null;
                }
            }
            return rate;
        }
    }
}
