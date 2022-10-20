using System;
using System.Collections.Generic;
using FireSharp.Config;
using FireSharp.Interfaces;
using Firebase.Storage;
using CoolSms;
using Xamarin.Forms;
using System.Runtime.ExceptionServices;
using CoreFoundation;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;

namespace AIO
{
    public partial class CertificationPage : ContentPage
    {
        Button[] buttons = new Button[2];
        Entry user_phone_number = new Entry();
        Entry certification = new Entry();
        StackLayout page = new StackLayout();
        ImageButton go_back = new ImageButton();
        Label re_certification_message = new Label();

        public bool Key_Sent = false;
        public Random random_key = new Random();
        public string certificate_key;

        public CertificationPage()
        {
            InitializeComponent();
            buttons[0] = FindByName("re_certificate") as Button;
            buttons[1] = FindByName("register") as Button;
            go_back = FindByName("close") as ImageButton;
            user_phone_number = FindByName("user_phone_number_entry") as Entry;
            certification = FindByName("certification_number_entry") as Entry;
            re_certification_message = FindByName("message") as Label;
            page = FindByName("certification_page") as StackLayout;
            page.FadeTo(1, 100);
            Network_Error();
        }

        FirebaseConfig server = new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        SmsApi api = new SmsApi(new SmsApiOptions
        {
            ApiKey = "NCSAQ8MEVSRRRTSE",
            ApiSecret = "YKPARRWVDRZTHB447JQPBNB347ZVYM2F",
            DefaultSenderId = "01097063115"
        });

        void Button_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            var button_id = button.ClassId;
            if (button_id == "re_certificatebutton")
            {
                ScaleUp_Animation(0);
            }
            else if (button_id == "registerbutton")
            {
                ScaleUp_Animation(1);
            }
        }

        void Button_Released(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            var button_id = button.ClassId;
            if (button_id == "re_certificatebutton")
            {
                ScaleDown_Animation(0);
                Re_CertificateButton_Pressed();
            }
            else if (button_id == "registerbutton")
            {
                ScaleDown_Animation(1);
                RegisterButton_Pressed();
            }
        }

        void Re_CertificateButton_Pressed()
        {
            SendSMS();
            buttons[0].Text = "인증 번호 재발송";
            re_certification_message.Text = "만약 인증 번호가 오지 않았다면?";
        }

        async void RegisterButton_Pressed()
        {
            if (!Key_Sent)
            {
                await DisplayAlert("오류", "인증번호를 발급받아주세요!", "확인");
                return;
            }

            //네트워크 연결 확인
            Network_Error();
            if (certification.Text != certificate_key)
            {
                await DisplayAlert("알림", "인증 번호가 틀렸습니다.", "확인");
                return;
            }


            //var result = client.Get("schools/" + App.Static_Encrypt_Key(RegisterPage.upload.user_study_info) + "/" + App.Static_Encrypt_Key(RegisterPage.upload.user_name) + "/");
            //var data = result.ResultAs<List<School>>();

            
            RegisterPage.upload.user_phone_number = App.Static_Encrypt_Key(user_phone_number.Text);
            var school = client.Set("schools/" + App.Static_Encrypt_Key(RegisterPage.upload.user_study_info) + "/" + App.Static_Encrypt_Key(RegisterPage.upload.user_name) + "/" // + data.Count
                , RegisterPage.school);
            var users = client.Set("users/" + RegisterPage.upload.user_id, RegisterPage.upload);

            //var faceServiceClient = new FaceServiceClient("d45992ab8a2c4b56a84ed9e3c2d2b7df");

            //if(faceServiceClient.GetPersonsAsync(App.Static_Encrypt_Key(RegisterPage.upload.user_name)) == null)
            //{
            //    await faceServiceClient.CreatePersonGroupAsync(App.Static_Encrypt_Key(RegisterPage.upload.user_name), RegisterPage.upload.user_name + "로그인");
            //}

            //var p = await faceServiceClient.CreatePersonAsync(App.Static_Encrypt_Key(RegisterPage.upload.user_name), RegisterPage.upload.user_id);
           
            //await faceServiceClient.AddPersonFaceAsync(App.Static_Encrypt_Key(RegisterPage.upload.user_name), p.PersonId, new MemoryStream(RegisterPage.school.user_image));




            await DisplayAlert("알림", "회원 가입 성공!", "확인");
            GoTo_LoginPage();
        }

        async void Network_Error()
        {
            try
            {
                client = new FireSharp.FirebaseClient(server);
            }
            catch
            {
                await DisplayAlert("오류가 발생했습니다", "네트워크 연결을 확인해주세요.", "확인");
            }
        }

        private void SendSMS()
        {
            certificate_key = random_key.Next(100000, 999999).ToString();

            Console.WriteLine(certificate_key);

            //var result = api.SendMessageAsync(user_phone_number.Text, "AIO 회원 가입\n인증 번호 : " + certificate_key.ToString());
            Key_Sent = true;
        }

        async void GoTo_LoginPage()
        {
            NavigationPage LoginPage = new NavigationPage(new LoginPage());
            NavigationPage CertificationPage = new NavigationPage(new CertificationPage());
            await CertificationPage.PopToRootAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = LoginPage;
        }

        async void GoTo_RegisterPage()
        {
            NavigationPage CertificationPage = new NavigationPage(new CertificationPage());
            NavigationPage RegisterPage = new NavigationPage(new RegisterPage());
            await CertificationPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = RegisterPage;
        }

        async void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            await go_back.ScaleTo(2.6, 50);
        }
        async void ImageButton_Released(System.Object sender, System.EventArgs e)
        {
            await go_back.ScaleTo(2.3, 50);
            GoTo_RegisterPage();
        }

        async void ScaleUp_Animation(int i)
        {
            await buttons[i].ScaleTo(1.1, 50);
        }

        async void ScaleDown_Animation(int i)
        {
            await buttons[i].ScaleTo(1, 50);
        }

    }
}
