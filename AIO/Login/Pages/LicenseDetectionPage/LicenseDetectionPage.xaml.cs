using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.ProjectOxford.Face;
using SkiaSharp;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
namespace AIO
{
    public partial class LicenseDetectionPage : ContentPage
    {
        public Stream img;
        public string name;
        public string school;

        public const string ApiKey = "bd4d81c272fb4e0e96d734725de4c86f";
        public const string EndPoint = "https://aio-textrecognition.cognitiveservices.azure.com/";

        public IFirebaseClient client = new FirebaseClient(
        new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        });

        public LicenseDetectionPage()
        {
            InitializeComponent();
        }

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
                    //await Recognize_Text();
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
                await img.CopyToAsync(RegisterPage.user_image);
                RegisterPage.user_image.Position = 0;
                var ocrResult = await client.ReadInStreamAsync(RegisterPage.user_image);


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

                var school = string.Empty;
                var name = string.Empty;

                foreach (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.Line line in textResults[0].Lines)
                {
                    var text = line.Text.Replace(" ", "");

                    //await DisplayAlert("결과", text, "확인");

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

                await CompareImage(school, name);

                //if (!CompareImage())
                //    GoToLoginPage();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!", ex.ToString(), "Okay!");
            }
        }

        public async void GoToLoginPage()
        {
            NavigationPage CameraPage = new NavigationPage(new CameraPage());
            NavigationPage LoginPage = new NavigationPage(new LoginPage());
            await CameraPage.PushAsync(LoginPage);
            Application.Current.MainPage = LoginPage;
        }

        private async void MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            CapturedImage.Source = e.Image;
            await CapturedImage.FadeTo(1, 100);
            await MinimalizeImage(e.ImageData);
        }

        public async Task CompareImage(string school, string name)
        {
            var result = client.Get("schools/" + App.Static_Encrypt_Key(school) + "/" + App.Static_Encrypt_Key(name) + "/");
            var data = result.ResultAs<List<School>>();

            //var faceServiceClient = new FaceServiceClient("d45992ab8a2c4b56a84ed9e3c2d2b7df");

            //// Step 1 - Create Person Group
            //var user_group = Guid.NewGuid().ToString();
            //await faceServiceClient.CreatePersonGroupAsync(name, name + "로그인");

            //// Step 2 - Add persons (and faces) to person group.
            //foreach (var user in data)
            //{
            //    // Step 2a - Create a new person, identified by their name.
            //    var p = await faceServiceClient.CreatePersonAsync(personGroupId, employee.Name);
            //    // Step 3a - Add a face for that person.
            //    await faceServiceClient.AddPersonFaceAsync(personGroupId, p.PersonId, employee.PhotoUrl);
            //}

            //// Step 3 - Train facial recognition model.
            //await faceServiceClient.TrainPersonGroupAsync(personGroupId);


            //var url = new FirebaseStorage("").Child(school).Child(name).GetDownloadUrlAsync();
            //var captured_image = new MemoryStream();
            //img.CopyTo(captured_image);
            //var compare_image = captured_image.ToArray();

            //var dif_pixel = 0;
            //for (int i = 0; i < compare_image.Length; i++)
            //{
            //    var compare = true;

            //    if (image_byte[i] != compare_image[i])
            //    {
            //        dif_pixel++;
            //        compare = false;
            //    }


            //    Console.WriteLine(compare);
            //}

            //double accuracity = (captured_image.Length - dif_pixel) / captured_image.Length;

            //if (accuracity > 0.9d)
            //{
            //    Console.WriteLine("Login Success!");
            //    return true;
            //}
            //return false;


        }

        public async Task MinimalizeImage(byte[] bytes)
        {
            try
            {
                var bitmap = SKBitmap.Decode(bytes);
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
                    NavigationPage _RegisterPage = new NavigationPage(new RegisterPage());
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

