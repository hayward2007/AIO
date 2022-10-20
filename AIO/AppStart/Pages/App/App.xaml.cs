using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AIO
{
    public partial class App : Application
    {
        public static List<User_Info_Serialize> user_login_data = new List<User_Info_Serialize>();
        public static List<Upload> users_data = new List<Upload>();
        public static bool Logined = false;

        //json 가져오기
        public static string json_name = "user_data.json";
        public static string json_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), json_name);

        FirebaseConfig server = new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        public App()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjkxMzgwQDMyMzAyZTMyMmUzME1CYkM0a1hscUxwTExwYWNGYzd4ZGZZUUY5TmNJWjBFQlNSWHh6ZTJOdTA9");
            //MainPage = new NavigationPage(new LicenseDetectionPage());
            //MainPage = new NavigationPage(new CameraPage());
            MainPage = new NavigationPage(new StartPage());
        }

        public static string Dynamic_Encrypt_Key(string password)
        {
            var key = password;
            var key_ASCII = Encoding.ASCII.GetBytes(key);
            var encrypted_key_ASCII = new List<int>();
            var encrypted_key = string.Empty;
            foreach (var text in key_ASCII)
            {
                var _byte_num = int.Parse(text.ToString());
                var random_ASCII = new Random();
                var encrypted_byte_hint_num = random_ASCII.Next(33 - _byte_num, 126 - _byte_num);
                var encrypted_byte_num = _byte_num;
                encrypted_byte_num += ((int)encrypted_byte_hint_num);
                //Console.WriteLine(encrypted_byte_num);
                //Console.WriteLine(encrypted_byte_hint_num);
                encrypted_key_ASCII.Add(((int)encrypted_byte_num)); 
                encrypted_key_ASCII.Add(((int)encrypted_byte_hint_num));
                encrypted_key += (char)encrypted_byte_num;
                if (encrypted_byte_hint_num.ToString().Length == 1)
                    encrypted_key += "00" + encrypted_byte_hint_num;
                else if (encrypted_byte_hint_num.ToString().Length == 2)
                {
                    if (encrypted_byte_hint_num < 0)
                        encrypted_key += "-0" + encrypted_byte_hint_num * -1;
                    else
                        encrypted_key += "0" + encrypted_byte_hint_num;
                }
                else if (encrypted_byte_hint_num.ToString().Length == 3)
                    encrypted_key += encrypted_byte_hint_num;
            }
            return encrypted_key;
        }

        public static string Dynamic_Decipher_Key(string password)
        {
            var detox_key_ASCII = string.Empty;
            for (var index = 0; index < password.Length; index += 4)
            {
                var current_byte_hint_num = 0;
                var current_byte_num = 0;
                var current_byte = Convert.ToByte(password[index]);
                current_byte_num = (int)current_byte;
                if (password[index + 1] == '-')
                {
                    current_byte_hint_num = -1 * (int.Parse(password[index + 2].ToString()) * 10 + int.Parse(password[index + 3].ToString()));
                }
                else
                {
                    current_byte_hint_num = int.Parse(password[index + 1].ToString()) * 100 + int.Parse(password[index + 2].ToString()) * 10 + int.Parse(password[index + 3].ToString());
                }
                //Console.WriteLine(current_byte_num);
                //Console.WriteLine(current_byte_hint_num);
                current_byte_num -= current_byte_hint_num;
                detox_key_ASCII += (char)current_byte_num;
            }
            return detox_key_ASCII;
        }

        public static string Static_Encrypt_Key(string password)
        {
            StringBuilder hex = new StringBuilder(password.Length * 2);
            foreach (byte b in password)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string Static_Decipher_Key(string password)
        {
            byte[] bytes = Enumerable.Range(0, password.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(password.Substring(x, 2), 16))
                    .ToArray();

            return Encoding.ASCII.GetString(bytes);
        }

        protected override void OnStart()
        {

        }

        protected override async void OnSleep()
        {
            var json_file = new List<User_Info_Serialize>();
            if (user_login_data.Count != 0 && Logined == true)
            {
                json_file.Add(user_login_data[0]);
                client = new FireSharp.FirebaseClient(server);
                File.WriteAllText(json_path, JsonConvert.SerializeObject(json_file));
                var send = await client.SetAsync("users/" + user_login_data[0].user_id, user_login_data);
            }
        }

        protected override void OnResume()
        {
        }
    }
}
