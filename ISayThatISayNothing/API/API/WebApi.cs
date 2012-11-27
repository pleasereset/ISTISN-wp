using ISayThatISayNothing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace ISayThatISayNothing.API
{
    public static class WebApi
    {
        public const string ApiRoot = "http://www.isaythatisaynothing.com/api/";
        public const string GetAllMessagesWS = "messages/last/all";
        public const string GetMessagesSinceWS = "messages/last/since/"; // + unix timestamp
        public const string GetPaginatedMessagesWS = "messages/last/paginate/"; // + page number

        public static void GetAllMessages(Action<List<MessageModel>> callback)
        {
            var uri = new Uri(ApiRoot + GetAllMessagesWS);
            DoMessageWSRequest(uri, callback);
        }

        public static void GetMessagesSince(DateTime since, Action<List<MessageModel>> callback)
        {
            var uri = new Uri(ApiRoot + GetMessagesSinceWS + Utils.DateTimeToUnixTimeStamp(since));
            DoMessageWSRequest(uri, callback);
        }

        public static void GetPaginatedMessages(int page, Action<List<MessageModel>> callback)
        {
            var uri = new Uri(ApiRoot + GetPaginatedMessagesWS + page);
            DoMessageWSRequest(uri, callback);
        }

        private static void DoMessageWSRequest(Uri WSUri, Action<List<MessageModel>> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
            {
                if (e.Cancelled == false && e.Error == null)
                {
                    var messages = ParseMessageString(e.Result);
                    callback(messages);
                }
                else
                {
                    MessageBox.Show("Shit happened !");
                    callback(null);
                }
            };
            webClient.DownloadStringAsync(WSUri);
        }

        private static List<MessageModel> ParseMessageString(string messageString)
        {
            List<MessageModel> messages = JsonConvert.DeserializeObject<List<MessageModel>>(messageString);
            return messages;
        }
    }
}
