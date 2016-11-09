using System.Runtime.Serialization;

namespace Oaiso
{
    [DataContract]
    public class SlackParameters
    {
            [DataMember]
            public string token { get; set; }
            [DataMember]
            public string team_id { get; set; }
            [DataMember]
            public string team_domain { get; set; }
            [DataMember]
            public string channel_id { get; set; }
            [DataMember]
            public string channel_name { get; set; }
            [DataMember]
            public string user_id { get; set; }
            [DataMember]
            public string user_name { get; set; }
            [DataMember]
            public string command { get; set; }
            [DataMember]
            public string text { get; set; }
            [DataMember]
            public string response_url { get; set; }
    }

    [DataContract]
    public class SlackResponseParameter
    {
        [DataMember]
        public string response_type { get; set; }
        [DataMember]
        public string text { get; set; }
    }

}