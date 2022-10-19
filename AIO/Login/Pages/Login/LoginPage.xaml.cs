using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AIO
{
    public partial class LoginPage : ContentPage
    {
        Button[] buttons = new Button[3];
        Entry id = new Entry();
        Entry password = new Entry();
        Label error = new Label();
        public StackLayout page = new StackLayout();

        public LoginPage()
        {
            InitializeComponent();
            buttons[0] = FindByName("login") as Button;
            buttons[1] = FindByName("register") as Button;
            buttons[2] = FindByName("find") as Button;
            id = FindByName("id_entry") as Entry;
            password = FindByName("password_entry") as Entry;
            error = FindByName("login_error") as Label;
            page = FindByName("login_page") as StackLayout;
            page.FadeTo(1, 100);
        }

        //파이어베이스 클라이언트 접속
        public IFirebaseClient client = new FireSharp.FirebaseClient(new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        });

        async void Button_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1.1, 50);
        }

        async void Button_Released(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1, 50);

            var button_id = button.ClassId;

            if (button_id == "loginbutton")
            {
                LoginButton_Pressed();
            }
            else if (button_id == "registerbutton")
            {
                RegisterButton_Pressed();
            }
            else if (button_id == "findbutton")
            {

            }
        }

        //로그인
        public async void LoginButton_Pressed()
        {
            Network_Error();

            if (id.Text == "" || password.Text == "" || id.Text == null || password.Text == null)
            {
                error.Text = "아이디 또는 비밀번호가 공백입니다";
            }
            else
            {
                //개선 전 로그인 장치

                //--------------------------------------------------------------------------------------------
                //foreach (var data in App.users_data)
                //{
                //    if (id.Text == data.user_id)
                //    {
                //        if (password.Text == data.user_password)
                //        {

                //            App.user_login_data[0].Is_Logined = true;
                //            App.user_login_data[0].user_id = data.user_id;
                //            App.user_login_data[0].user_password = data.user_password;
                //            App.user_login_data[0].user_name = data.user_name;
                //            App.user_login_data[0].user_birth = data.user_birth;
                //            App.user_login_data[0].user_study_info = data.user_study_info;
                //            App.user_login_data[0].user_phone_number = data.user_phone_number;

                //            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "User_Info.json"));
                //            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "User_Info.json"), Newtonsoft.Json.JsonConvert.SerializeObject(App.user_login_data[0]));


                //            Login_Success();
                //            GoTo_MainMenuPage();
                //            return;
                //        }
                //    }
                //}
                //Login_Error();
                //--------------------------------------------------------------------------------------------


                //개선 후 로그인 장치

                //--------------------------------------------------------------------------------------------
                try
                {
                    App.users_data.Clear();
                    var data = await client.GetAsync("users/" + App.Static_Encrypt_Key(id.Text));
                    App.users_data.Add(JsonConvert.DeserializeObject<Upload>(data.Body));

                    //비밀번호 비교하기
                    if (password.Text == App.Dynamic_Decipher_Key(App.users_data[0].user_password))
                    {

                        App.Logined = true;

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


                        Login_Success();

                        GoTo_MainMenuPage();
                    }
                    //틀릴시 저장 정보 초기하기
                    else
                    {
                        Login_Error();
                    }
                }
                catch (Exception e)
                {
                    await DisplayAlert("Error", e.ToString(), "Okay!");
                }
            }
        }


        //네트워크 연결 확인
        async void Network_Error()
        {
            try
            {
                client = new FireSharp.FirebaseClient(new FirebaseConfig()
                {
                    AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
                    BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
                });
            }
            catch
            {
                await DisplayAlert("오류가 발생했습니다", "네트워크 연결을 확인해주세요.", "확인");
            }
        }

        //로그인 에러 메세지 출력
        void Login_Error()
        {
            error.Text = "아이디 또는 비밀번호가 틀렸습니다";
            buttons[2].IsVisible = true;
            buttons[2].IsEnabled = true;
        }

        //로그인 성공 알림 출력
        async void Login_Success()
        {
            await DisplayAlert("알림", "로그인하였습니다!", "확인");
        }

        //MainMenuPage로 이동
        async void GoTo_MainMenuPage()
        {
            NavigationPage LoginPage = new NavigationPage(new LoginPage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await LoginPage.PushAsync(MainMenuPage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }

        //RegisterPage로 이동
        async void RegisterButton_Pressed()
        {
            NavigationPage LoginPage = new NavigationPage(new LoginPage());
            NavigationPage RegisterPage = new NavigationPage(new CameraPage());
            //NavigationPage RegisterPage = new NavigationPage(new RegisterPage());
            await LoginPage.PushAsync(RegisterPage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = RegisterPage;
        }
    }
}
