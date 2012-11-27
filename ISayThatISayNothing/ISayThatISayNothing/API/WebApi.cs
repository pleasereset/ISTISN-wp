using ISayThatISayNothing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ISayThatISayNothing.API
{
    public static class WebApi
    {
        public const string ApiRoot = "http://www.isaythatisaynothing.com/api/";
        public const string GetAllMessagesWS = "messages/last/all";

        public static void GetAllMessages(Action<List<MessageModel>> callback)
        {
            var webClient = new WebClient();
            var uri = new Uri(ApiRoot + GetAllMessagesWS);
            webClient.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
            {
                if (e.Cancelled == false && e.Error == null)
                {
                    var messages = ParseMessageString(e.Result);
                    callback(messages);
                }
                else
                {
                    // TODO
                }
            };
            webClient.DownloadStringAsync(uri);
        }

        private static List<MessageModel> ParseMessageString(string messageString)
        {
            List<MessageModel> messages = JsonConvert.DeserializeObject<List<MessageModel>>(messageString);
            return messages;
        }
    }
}
