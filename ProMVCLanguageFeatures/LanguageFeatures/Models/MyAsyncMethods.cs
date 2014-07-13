using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LanguageFeatures.Models
{
    public class MyAsyncMethods
    {
        //public static Task<long?> GetPageLength(){
        public static async Task<long?> GetPageLength(){

            //HttpClient client = new HttpClient();
            //var httpTask = client.GetAsync("http://apress.com");

            //return httpTask.ContinueWith((Task<HttpResponseMessage> antecedent) =>
            //{
            //    return antecedent.Result.Content.Headers.ContentLength;
            //});

            HttpClient client = new HttpClient();

            var httpMessage = await client.GetAsync("http://apress.com");

            var sum = 1 + 2;

            return httpMessage.Content.Headers.ContentLength;
        }
    }
}