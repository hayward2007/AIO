using System;
using System.Collections.Generic;

namespace AIO
{
    public class RoomClass
    {
        public string room_name { get; set; }
        public List<string> user { get; set; }
        public List<string> messages { get; set; }
    }

    public class DownloadRoomClass
    {
        public string room_name { get; set; }
    }

    public class MessageClass
    {
        public string room_name { get; set; }
        public List<User> user { get; set; }
    }

    public class User
    {
        public string user_name { get; set; }
        public string user_id { get; set; }
    }
}
