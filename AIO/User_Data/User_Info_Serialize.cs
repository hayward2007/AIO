using System;
using System.Collections.Generic;
using System.IO;

namespace AIO
{
    public class User_Info_Serialize
    {
        public bool Is_Logined { get; set; }
        public string user_name { get; set; }
        public string user_id { get; set; }
        public string user_password { get; set; }
        //public byte[] user_image { get; set; }
        public string user_birth { get; set; }
        public string user_study_info { get; set; }
        public string user_phone_number { get; set; }
        public string button_color_mode { get; set; }
        public string button_solid_color { get; set; }
        public string button_gradient_color { get; set; }
        public string background_color_mode { get; set; }
        public string background_solid_color { get; set; }
        public string background_gradient_color { get; set; }
    }
}
