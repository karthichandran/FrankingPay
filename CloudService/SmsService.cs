using System;
using System.Net.Http;

namespace CloudService
{
    public class SmsService
    {
        HttpClient reproClient;
        public SmsService()
        {
            reproClient = new HttpClient();
            reproClient.BaseAddress = new Uri("http://leansyshost-001-site3.itempurl.com/api/");
        }
        public MessageDto GetOTP()
        {
           
            MessageDto msg = null;
            HttpResponseMessage response = new HttpResponseMessage();
            response = reproClient.GetAsync("Message/1").Result;

            if (response.IsSuccessStatusCode)
            {
                //  return JsonConvert.DeserializeObject<MessageDto>(await response.Content.ReadAsStreamAsync());

               // msg = JsonSerializer.DeserializeAsync<MessageDto>(await response.Content.ReadAsStreamAsync()).Result;
                 msg = response.Content.ReadAsAsync<MessageDto>().Result;
            }
            return msg;
        }

        public bool DeleteOTP()
        {
           
            HttpResponseMessage response = new HttpResponseMessage();
            response = reproClient.DeleteAsync("Message/1").Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }

    public class MessageDto
    {
        public int MessageID { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? Verified { get; set; }
        public int? Lane { get; set; }
        public string Message { get; set; }
        public int? Error_code { get; set; }
        public int Opt { get; set; }
    }

}
