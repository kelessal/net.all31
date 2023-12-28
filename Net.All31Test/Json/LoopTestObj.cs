using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Json.Test
{
    class LoopTestObj
    {
        public string Name { get; set; }
        public LoopTestObj LoopProp { get; set; }
    }
    public interface IRequestInfo
    {
        string Token { get; set; }
        string TransactionId { get; }
        string EndPoint { get; }
        string Language { get; }
        string IPAddress { get; }
        string Currency { get; }
        string AppId { get; }
        string AccountId { get; }
        string HotelId { get; }
        string ReservationId { get; }
        string DeviceId { get; }
        bool Force { get; set; }
        string UserAccountId { get; }
        DateTime RequestDate { get; }
        long RequestDay { get; }
        TimeSpan TimeOffset { get; }
        string Host { get; }
        string Referer { get; }
        
      
        bool IsTest { get; }
        bool AllLangs { get; set; }
        TimeRange StaffActiveTimeRange { get; set; }
        string TokenIPAddress { get; set; }
        bool TokenExpired { get; set; }
       
    }
}
