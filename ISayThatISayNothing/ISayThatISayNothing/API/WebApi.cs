using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ISayThatISayNothing.API
{
    public class WebApi
    {
        public const string ApiRoot = "http://www.isaythatisaynothing.com/api/";
        public const string GetAllMessagesWS = "messages/last/all";

        public void GetAllMessages()
        {
            var webClient = new WebClient();
            var uri = new Uri(ApiRoot + GetAllMessagesWS);
            webClient.DownloadStringCompleted += GetAllMessages_DownloadStringCompleted;
            webClient.DownloadStringAsync(uri);
        }

        void GetAllMessages_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
