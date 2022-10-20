using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Firebase.Storage;
using FireSharp.Config;
using FireSharp.Interfaces;
using Foundation;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AIO
{
    public partial class StartPage : ContentPage
    {
        //json 가져오기
        public static string json_name = "user_data.json";
        public static string json_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), json_name);

        //파이어베이스 클라이언트 접속
        public IFirebaseClient client = new FireSharp.FirebaseClient(new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        });

        public StartPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(() =>
            {
                //try
                //{
                //    using (var web = new WebClient())
                //    {
                //        var data = await web.OpenReadTaskAsync("https://t1.daumcdn.net/cfile/tistory/24283C3858F778CA2E");

                //        if (data == null)
                //            await DisplayAlert("", "ㅅㅂ?", "");

                //        var image = new MemoryStream();
                //        await data.CopyToAsync(image);

                //        await new FirebaseStorage("sunae-3cf06.appspot.com",
                //        new FirebaseStorageOptions()
                //        {
                //            ThrowOnCancel = true
                //        })
                //        .Child("User_Image")
                //        .Child(RegisterPage.upload.user_id + ".jpg")
                //        .PutAsync(image);
                //    }
                //}
                //catch(Exception e)
                //{
                //    Console.WriteLine(e.ToString());
                //}



                //앱 배포시 아래 주석 풀기
                Log_In();

                //앱 디버그 시 아래 코드 사용
                //Re_Log_In();
            });
        }

        public async void Log_In()
        {
            ////개선 전 로그인 장치

            //--------------------------------------------------------------------------------------------
            ////json 가져오기
            //string json_name = "User_Info.json";
            //string json_path = Path.Combine(Directory.GetCurrentDirectory(), json_name);

            ////파이어베이스 클라이언트 접속
            //FirebaseClient realtime_db = new FirebaseClient("https://sunae-3cf06-default-rtdb.firebaseio.com/",
            //new FirebaseOptions
            //{
            //    AuthTokenAsyncFactory = () =>
            //                Task.FromResult("3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z")
            //});

            ////파이어베이스 정보 가져오기
            //var user_data_task = Task.Run(async () =>
            //{
            //    var get_users_data = (await realtime_db
            //    .Child("users")
            //    .OnceAsync<Download>()).Select(data => new Download()
            //    {
            //        user_id = data.Object.user_id,
            //        user_password = data.Object.user_password,
            //        user_name = data.Object.user_name,
            //        user_birth = data.Object.user_birth,
            //        user_study_info = data.Object.user_study_info,
            //        user_phone_number = data.Object.user_phone_number,
            //    });
            //    //파이어베이스 정보 리스트로 저장하기
            //    users_data = get_users_data.ToList();
            //});
            //user_data_task.Wait();

            //if (!File.Exists(json_path))
            //{
            //    //json 초기화
            //    var json_file = new List<User_Info_Serialize>();
            //    json_file.Add(new User_Info_Serialize()
            //    {
            //        Is_Logined = true,
            //        user_id = "hayward2007",
            //        user_name = "김형석",
            //        user_birth = "",
            //        user_password = "khs-070221",
            //        user_study_info = "",
            //        user_phone_number = "",
            //        button_color_mode = "solid",
            //        button_solid_color = "",
            //        button_gradient_color = "",
            //        background_color_mode = "gradient",
            //        background_solid_color = "",
            //        background_gradient_color = ""
            //        //Is_Logined = false,
            //        //user_id = "",
            //        //user_name = "",
            //        //user_birth = "",
            //        //user_password = "",
            //        //user_study_info = "",
            //        //user_phone_number = "",
            //        //button_color_mode = "",
            //        //button_solid_color = "",
            //        //button_gradient_color = "",
            //        //background_color_mode = "",
            //        //background_solid_color = "",
            //        //background_gradient_color = ""

            //    });
            //    //json 저장
            //    File.WriteAllText(json_path, JsonConvert.SerializeObject(json_file));


            //    if (user_login_data[0].Is_Logined == true)
            //    {
            //        //사용자 데이터하고 저장 데이터 비교하기
            //        foreach (var data in users_data)
            //        {
            //            //아이디 비교하기
            //            if (user_login_data[0].user_id == data.user_id)
            //            {
            //                //비밀번호 비교하기
            //                if (user_login_data[0].user_password == data.user_password)
            //                {
            //                    //메인메뉴페이지로 이동
            //                    NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            //                    MainPage = MainMenuPage;
            //                }
            //                //틀릴시 저장 정보 초기하기
            //                else
            //                {
            //                    user_login_data[0].Is_Logined = false;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //로그인 안할 시 로그인페이지로 이동
            //        NavigationPage LoginPage = new NavigationPage(new LoginPage());
            //        MainPage = LoginPage;
            //    }
            //--------------------------------------------------------------------------------------------


            //개선 후 로그인 장치

            //--------------------------------------------------------------------------------------------
            try
            {
                if (!File.Exists(json_path))
                {
                    Re_Log_In();
                }

                //json 읽기
                var json_text = File.ReadAllText(json_path);

                if (!json_text.Contains("["))
                    json_text = "[" + json_text + "]";

                //json 해석
                App.user_login_data = JsonConvert.DeserializeObject<List<User_Info_Serialize>>(json_text);

                //앱 색상 지정
                if (App.user_login_data[0].background_solid_color != "" || App.user_login_data[0].background_solid_color != null)
                {
                    SettingPage.linearGradientBrush.EndPoint = new Point(0, 1);
                    SettingPage.linearGradientBrush.GradientStops.Add(SettingPage.gradientStop1);
                    SettingPage.linearGradientBrush.GradientStops.Add(SettingPage.gradientStop2);
                    SettingPage.gradientStop1.Color = Color.FromHex(App.user_login_data[0].background_solid_color);
                    if (App.user_login_data[0].background_color_mode == "gradient")
                    {
                        SettingPage.gradientStop2.Color = Color.FromHex(App.user_login_data[0].background_gradient_color);
                    }
                }

                //로그인 확인하기
                if (App.user_login_data[0].Is_Logined == true)
                {
                    //var user_data_task = Task.Run(async () =>
                    //{
                    //    var get_users_data = (await realtime_db
                    //    .Child("users/" + user_login_data[0].user_id + "/")
                    //    .OnceAsync<Download>()).Select(data => new Download()
                    //    {
                    //        user_id = data.Object.user_id,
                    //        user_password = data.Object.user_password,
                    //        user_name = data.Object.user_name,
                    //        user_birth = data.Object.user_birth,
                    //        user_study_info = data.Object.user_study_info,
                    //        user_phone_number = data.Object.user_phone_number,
                    //    });
                    //    //파이어베이스 정보 리스트로 저장하기
                    //    users_data = get_users_data.ToList();
                    //});
                    //user_data_task.Wait();



                    //나중에 고칠거
                    var data = await client.GetAsync("users/" + App.Static_Encrypt_Key(App.user_login_data[0].user_id));

                    App.users_data.Add(JsonConvert.DeserializeObject<Upload>(data.Body));


                    //아이디 비교하기
                    if (App.user_login_data[0].user_id != App.Static_Decipher_Key(App.users_data[0].user_id))
                    {
                        Re_Log_In();
                        return;
                    }
                    //비밀번호 비교하기
                    if (App.user_login_data[0].user_password != App.Dynamic_Decipher_Key(App.users_data[0].user_password))
                    {
                        //틀릴시 저장 정보 초기하기
                        Re_Log_In();
                        return;
                    }

                    App.Logined = true;

                    Console.WriteLine(App.user_login_data[0].user_id);
                    Console.WriteLine(App.user_login_data[0].user_password);
                    Console.WriteLine(App.user_login_data[0].user_name);

                    //메인메뉴페이지로 이동
                    NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
                    Application.Current.MainPage = MainMenuPage;
                    return;
                }

                Re_Log_In();
            }
            catch (Exception e)
            {
                Re_Log_In();

                //await DisplayAlert("Error", e.ToString(), "Okay!");

                //File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "bug.text"), e.ToString());
            }
            //--------------------------------------------------------------------------------------------
        }

        public async void Re_Log_In()
        {
            try
            {
                //json 초기화
                var json_file = new List<User_Info_Serialize>();
                json_file.Add(new User_Info_Serialize()
                {
                    Is_Logined = false,
                    user_id = "",
                    user_name = "",
                    user_birth = "",
                    user_password = "",
                    user_study_info = "",
                    user_phone_number = "",
                    button_color_mode = "",
                    button_solid_color = "",
                    button_gradient_color = "",
                    background_color_mode = "",
                    background_solid_color = "",
                    background_gradient_color = ""

                });
                
                //json 저장
                File.WriteAllText(json_path, JsonConvert.SerializeObject(json_file));

                //loginPage로 이동
                NavigationPage LoginPage = new NavigationPage(new LicenseDetectionPage());
                Application.Current.MainPage = LoginPage;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "Okay!");
            } 
        }
    }
}

