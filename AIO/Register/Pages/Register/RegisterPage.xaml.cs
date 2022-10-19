using Xamarin.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using Xamarin.CommunityToolkit;
using Xamarin.CommunityToolkit.UI.Views;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System;

namespace AIO
{
    public partial class RegisterPage : ContentPage
    {
        Button[] buttons = new Button[2];
        Entry[] entries = new Entry[3];
        DatePicker birth = new DatePicker();
        Picker study_info = new Picker();
        Label error = new Label();
        StackLayout page = new StackLayout();
        ImageButton go_back = new ImageButton();

        public static Upload upload = new Upload();
        public static School school = new School();
        public static MemoryStream user_image = new MemoryStream();

        public RegisterPage()
        {
            InitializeComponent();
            buttons[0] = FindByName("photo") as Button;
            buttons[1] = FindByName("register") as Button;
            go_back = FindByName("close") as ImageButton;
            entries[0] = FindByName("user_name_entry") as Entry;
            entries[1] = FindByName("user_id_entry") as Entry;
            entries[2] = FindByName("user_password_entry") as Entry;
            birth = FindByName("user_birth") as DatePicker;
            study_info = FindByName("user_study_info") as Picker;
            error = FindByName("register_error") as Label;
            page = FindByName("register_page") as StackLayout;

            if(user_image != null)
            {
                if(upload.user_name != "")
                    entries[0].Text = upload.user_name;
                if (upload.user_id != "")
                    entries[1].Text = upload.user_id;
                if (upload.user_password != "")
                    entries[2].Text = upload.user_password;
                if (upload.user_study_info != "")
                {
                    if (!study_info.Items.Contains(upload.user_study_info))
                        study_info.ItemsSource.Add(upload.user_study_info);
                    study_info.SelectedItem = upload.user_study_info;
                }
            }



            page.FadeTo(1, 100);
        }

        FirebaseConfig server = new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        void Button_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            var button_id = button.ClassId;
            if (button_id == "photobutton")
            {
                ScaleUp_Animation(0);
            }
            else if (button_id == "registerbutton")
            {
                ScaleUp_Animation(1);
            }
            else if (button_id == "closebutton")
            {
                ScaleUp_Animation(1);
            }
        }

        async void Button_Released(System.Object sender, System.EventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var button_id = button.ClassId;
                if (button_id == "registerbutton")
                {
                    ScaleDown_Animation(1);
                    RegisterButton_Pressed();
                }
            }
            catch (Exception k)
            {
                await DisplayAlert("", k.ToString(), "");
            }

        }

        async void RegisterButton_Pressed()
        {
            try
            {
                //네트워크 연결 확인
                Network_Error();

                if (entries[0].Text == null || entries[1].Text == null || entries[2].Text == null || entries[0].Text == "" || entries[1].Text == "" || entries[2].Text == "" || study_info.SelectedItem == null || birth.Date == null)
                {
                    error.Text = "입력 공간에 공백이 있습니다";
                    return;
                }
                else if (entries[2].Text.Length < 8)
                {
                    error.Text = "비밀번호는 영문 또는 기호 8글자 이상이어야 합니다";
                    return;
                }

                //이미 있는 계정인지 확인하기
                if (client.Get("users/" + App.Static_Encrypt_Key(entries[1].Text)) == null)
                {
                    await DisplayAlert("알림", "이미 사용된 ID입니다.", "확인");
                    return;
                }

                //var result = client.Get("users/" + RegisterPage.upload.user_id);
                //Upload upload_id = result.ResultAs<Upload>();

                ////암호화 할 사항
                upload.user_id = App.Static_Encrypt_Key(entries[1].Text);
                upload.user_password = App.Dynamic_Encrypt_Key(entries[2].Text);
                upload.user_birth = App.Static_Encrypt_Key(birth.Date.ToString().Substring(0, 10));
                upload.user_name = entries[0].Text;

                ////식별로 요소로 사용해야 함
                school.user_id = upload.user_id;
                if (user_image != null)
                    upload.user_study_info = upload.user_study_info;
                else
                    upload.user_study_info = study_info.SelectedItem.ToString();

                Console.WriteLine(upload.user_id);
                Console.WriteLine(upload.user_password);
                Console.WriteLine(upload.user_birth);
                Console.WriteLine(upload.user_study_info);
                Console.WriteLine(upload.user_name);

                //암호화 하기에는 양이 너무 많음
                school.user_image = user_image.ToArray();

                //암호화 할 필요 없는 사항
                upload.background_color_mode = "solid";
                upload.background_solid_color = "00000000";
                upload.background_gradient_color = "00000000";
                upload.button_color_mode = "solid";
                upload.button_solid_color = "00000000";
                upload.button_gradient_color = "00000000";

                GoTo_SMSCertificationPage();
            }
            catch(Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "Okay!");
            }
            

            
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

        async void GoTo_SMSCertificationPage()
        {
            var RegisterPage = new NavigationPage();
            var CertificationPage = new NavigationPage(new CertificationPage());
            await RegisterPage.PushAsync(CertificationPage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = CertificationPage;
        }

        async void GoTo_LoginPage()
        {
            NavigationPage LoginPage = new NavigationPage(new LoginPage());
            NavigationPage RegisterPage = new NavigationPage(new RegisterPage());
            await RegisterPage.PopAsync();
            await RegisterPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = LoginPage;
        }

        async void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            await go_back.ScaleTo(2.6, 50);
        }

        async void ImageButton_Released(System.Object sender, System.EventArgs e)
        {
            await go_back.ScaleTo(2.3, 50);
            GoTo_LoginPage();
        }

        async void ScaleUp_Animation(int i)
        {
            await buttons[i].ScaleTo(1.1, 50);
        }

        async void ScaleDown_Animation(int i)
        {
            await buttons[i].ScaleTo(1, 50);
        }

        async void photo_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1.1, 50);
        }

        async void photo_Released(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1, 50);

            NavigationPage CameraPage = new NavigationPage(new CameraPage());
            NavigationPage RegisterPage = new NavigationPage(new RegisterPage());
            await RegisterPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = CameraPage;
        }
    }
}
