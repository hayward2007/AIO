using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Drawing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading;
using SceneKit;
using System.ComponentModel;
using Xamarin.Forms.Shapes;
using Newtonsoft.Json;
using FireSharp.Config;
using FireSharp.Interfaces;
using JavaScriptCore;

namespace AIO
{
    public partial class LicenseDetectionPage : ContentPage
    {
        public Stream img;
        public string name;
        public string school;
        //public SKBitmap bitmap;

        public const string ApiKey = "bd4d81c272fb4e0e96d734725de4c86f";
        public const string EndPoint = "https://aio-textrecognition.cognitiveservices.azure.com/";

        public LicenseDetectionPage()
        {
            InitializeComponent();
        }

        FirebaseConfig server = new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient rdb_client;

        async void CaptureImage_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1.1, 100);
        }

        async void CaptureImage_Released(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1, 100);

            if (button.Text == "사진 촬영")
            {
                CameraView.Shutter();
                button.Text = "이 사진 사용하기";
                Function.Text = "다시 촬영하기";
            }
            else
            {
                loading.IsRunning = true;
                await loading.FadeTo(1, 100);
                try
                {
                    await Recognize_Text();
                }
                catch (Exception k)
                {
                    await DisplayAlert("Error", k.ToString(), "Okay!");
                }
            }
        }

        public async Task Recognize_Text()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                await DisplayAlert("Error!", "API Key must be provided.", "Okay!");
            }

            try
            {
                //Init Azure Computer Vision Api Client
                ComputerVisionClient client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(ApiKey),
                new System.Net.Http.DelegatingHandler[] { })
                {
                    Endpoint = EndPoint
                };

                // Recognize Text

                //var stream = new MemoryStream();
                //await img.CopyToAsync(stream);
                RegisterPage.user_image = new MemoryStream();
                await img.CopyToAsync(RegisterPage.user_image);
                RegisterPage.user_image.Position = 0;
                //stream.Position = 0;

                //var url = "https://mblogthumb-phinf.pstatic.net/MjAxOTA0MjhfMzYg/MDAxNTU2NDQ2MTM3Mjc1.Y242NW2EsCqzpYooTxA7dcYpRufjqvOeT1jsLTCh45kg.VC89QH4OrqDu1RNsJ7-coEr4jeBTgs7u_R9SjavbIwgg.PNG.sexyhun1983/1.png?type=w800";
                //var ocrResult = await client.ReadAsync(url);

                var ocrResult = await client.ReadInStreamAsync(RegisterPage.user_image);
                //var ocrResult = await client.ReadInStreamAsync(stream);


                var operationLocation = ocrResult.OperationLocation;
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract text
                ReadOperationResult results;
                do
                {
                    results = await client.GetReadResultAsync(Guid.Parse(operationId));

                }
                while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

                // Display text
                var textResults = results.AnalyzeResult.ReadResults;
                var is_certifiated = false;

                foreach (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.Line line in textResults[0].Lines)
                {
                    var text = line.Text.Replace(" ", "");

                    if (text == "학생증")
                        is_certifiated = true;
                    else if (text.Contains("학교"))
                        school = text;
                    else if (text.Length == 3 || text.Length == 4)
                        name = text;
                }

                // Student License(? 학생증) Certification
                if (!is_certifiated)
                {
                    var alert = await DisplayAlert("오류!", "학생증을 인식하지 못했습니다", "다시 찍기", "건너 뛰기");

                    if (alert)
                    {
                        _ = loading.FadeTo(0, 100);
                        await CapturedImage.FadeTo(0, 100);
                        loading.IsRunning = false;

                        CaptureImage.Text = "사진 촬영";
                        Function.Text = "다시 촬영하기";

                        return;
                    }
                }

                Console.WriteLine("학생증 인식 완료");
                Console.WriteLine(name);
                Console.WriteLine(school);

                if (school.Contains("충"))
                    school.Replace('충', '중');


                rdb_client = new FireSharp.FirebaseClient(server);

                var result = rdb_client.Get("schools/" + App.Static_Encrypt_Key(school) + "/" + App.Static_Encrypt_Key(name) + "/");
                var data = result.ResultAs<School>();

                Console.WriteLine(data.user_id);

                App.users_data.Clear();
                var _data = await rdb_client.GetAsync("users/" + data.user_id);
                Console.WriteLine(_data.Body);

                App.users_data.Add(JsonConvert.DeserializeObject<Upload>(_data.Body));

                Console.WriteLine(App.users_data[0].user_id);
                Console.WriteLine(App.users_data[0].user_password);
                Console.WriteLine(App.users_data[0].user_name);
                Console.WriteLine(App.users_data[0].user_study_info);
                Console.WriteLine(App.users_data[0].user_phone_number);
                Console.WriteLine(App.users_data[0].user_birth);

                App.Logined = true;
                App.user_login_data.Add(new User_Info_Serialize());
                //메인메뉴페이지로 이동
                App.user_login_data[0].user_id = App.Static_Decipher_Key(App.users_data[0].user_id);
                App.user_login_data[0].user_password = App.Dynamic_Decipher_Key(App.users_data[0].user_password);
                App.user_login_data[0].user_name = App.users_data[0].user_name;
                App.user_login_data[0].user_birth = App.Static_Decipher_Key(App.users_data[0].user_birth);
                App.user_login_data[0].user_study_info = App.users_data[0].user_study_info;
                App.user_login_data[0].user_phone_number = App.Static_Decipher_Key(App.users_data[0].user_phone_number);

                Console.WriteLine(App.user_login_data[0].user_id);
                Console.WriteLine(App.user_login_data[0].user_password);
                Console.WriteLine(App.user_login_data[0].user_name);
                Console.WriteLine(App.user_login_data[0].user_study_info);
                Console.WriteLine(App.user_login_data[0].user_phone_number);
                Console.WriteLine(App.user_login_data[0].user_birth);

                //암호화 안된 항목들
                App.user_login_data[0].Is_Logined = true;
                App.user_login_data[0].background_color_mode = App.users_data[0].background_color_mode;
                App.user_login_data[0].background_solid_color = App.users_data[0].background_solid_color;
                App.user_login_data[0].background_gradient_color = App.users_data[0].background_gradient_color;
                App.user_login_data[0].button_color_mode = App.users_data[0].button_color_mode;
                App.user_login_data[0].button_solid_color = App.users_data[0].button_solid_color;
                App.user_login_data[0].button_gradient_color = App.users_data[0].button_gradient_color;

                File.WriteAllText(App.json_path, Newtonsoft.Json.JsonConvert.SerializeObject(App.user_login_data[0]));

                await DisplayAlert("알림", "로그인하였습니다!", "확인");

                NavigationPage LoginPage = new NavigationPage();
                NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
                await LoginPage.PushAsync(MainMenuPage);
                Application.Current.MainPage = MainMenuPage;
            
            }
            catch (Exception ex)
            {
                var alert = await DisplayAlert("오류!", "학생증을 인식하지 못했습니다", "다시 찍기", "건너 뛰기");

                if (alert)
                {
                    _ = loading.FadeTo(0, 100);
                    await CapturedImage.FadeTo(0, 100);
                    loading.IsRunning = false;

                    CaptureImage.Text = "사진 촬영";
                    Function.Text = "다시 촬영하기";

                    return;
                }
                //await DisplayAlert("Error!", ex.ToString(), "Okay!");
            }
        }

        public async void GoToRegisterPage()
        {
            try
            {
                NavigationPage CameraPage = new NavigationPage(new CameraPage());
                NavigationPage RegisterPage = new NavigationPage(new RegisterPage());
                await CameraPage.PushAsync(RegisterPage);
                Application.Current.MainPage = RegisterPage;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "Okay!");
            }

        }

        private async void MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            CapturedImage.Source = e.Image;
            await CapturedImage.FadeTo(1, 100);
            await MinimalizeImage(e.ImageData);
        }

        public async Task MinimalizeImage(byte[] bytes)
        {
            try
            {
                var bitmap = SKBitmap.Decode(bytes);

                Console.WriteLine(bitmap.Width);
                Console.WriteLine(bitmap.Height);

                var info = new SKImageInfo(bitmap.Width / 10, bitmap.Height / 10);
                var mini_bitmap = bitmap.Resize(info, SKFilterQuality.Medium);
                var mini_image = SKImage.FromBitmap(mini_bitmap);
                var data = mini_image.Encode();
                var result = data.AsStream();
                img = result;
                img.Position = 0;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "Okay!");
            }
        }

        async void Function_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;

            await button.ScaleTo(1.1, 100);
        }

        async void Function_Released(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;

            await button.ScaleTo(1, 100);

            if (button.Text == "건너 뛰기")
            {
                try
                {
                    RegisterPage.user_image = null;

                    //loginPage로 이동
                    NavigationPage CameraPage = new NavigationPage(new CameraPage());
                    NavigationPage _RegisterPage = new NavigationPage(new LoginPage());
                    await CameraPage.PushAsync(_RegisterPage);
                    Application.Current.MainPage = _RegisterPage;
                }
                catch (Exception k)
                {
                    await DisplayAlert("Error", k.ToString(), "Okay!");
                }

            }
            else
            {
                await CapturedImage.FadeTo(0, 100);
                button.Text = "건너 뛰기";
                CaptureImage.Text = "사진 촬영";
            }
        }
    }
}

