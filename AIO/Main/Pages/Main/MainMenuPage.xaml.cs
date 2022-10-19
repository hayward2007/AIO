using System;
using System.Drawing.Drawing2D;
using Xamarin.Forms;

namespace AIO
{
    public partial class MainMenuPage : ContentPage
    {
        Label[] labels = new Label[4];
        AbsoluteLayout[] layouts = new AbsoluteLayout[4];
        StackLayout page = new StackLayout();

        public MainMenuPage()
        {
            InitializeComponent();
            labels[0] = FindByName("school_name") as Label;
            labels[1] = FindByName("time") as Label;
            labels[2] = FindByName("date") as Label;
            labels[3] = FindByName("surrent_class") as Label;
            layouts[0] = FindByName("Feed") as AbsoluteLayout;
            layouts[1] = FindByName("Calendar") as AbsoluteLayout;
            layouts[2] = FindByName("Message") as AbsoluteLayout;
            layouts[3] = FindByName("Setting") as AbsoluteLayout;
            page = FindByName("mainmenu_page") as StackLayout;

            if (App.user_login_data[0].button_solid_color != "00000000")
            {
                Feed_Frame.BackgroundColor = Color.FromHex(App.user_login_data[0].button_solid_color);
                Message_Frame.BackgroundColor = Color.FromHex(App.user_login_data[0].button_solid_color);
                Calendar_Frame.BackgroundColor = Color.FromHex(App.user_login_data[0].button_solid_color);
                Setting_Frame.BackgroundColor = Color.FromHex(App.user_login_data[0].button_solid_color);
            }
            if (App.user_login_data[0].background_solid_color != "00000000")
            {
                if (App.user_login_data[0].background_color_mode == "solid")
                {
                    page.BackgroundColor = Color.FromHex(App.user_login_data[0].background_solid_color);
                }
                else if (App.user_login_data[0].background_color_mode == "gradient")
                {
                    page.Background = SettingPage.linearGradientBrush;
                }
            }

            page.FadeTo(1, 100);
        }



        void Button_Pressed(System.Object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            var classId = Button.ClassId;
            if (classId == "feedbutton")
            {
                ScaleUp_Animation(0);
            }
            else if (classId == "calendarbutton")
            {
                ScaleUp_Animation(1);
            }
        }

        void Button_Released(System.Object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            var classId = Button.ClassId;
            if (classId == "feedbutton")
            {
                ScaleDown_Animation(0);
                GoTo_FeedPage();
            }
            else if (classId == "calendarbutton")
            {
                ScaleDown_Animation(1);
                GoTo_SchedulePage();
            }
        }

        void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var classId = Imagebutton.ClassId;
            if (classId == "messagebutton")
            {
                Layout_ScaleUp_Animation(2);
            }
            else if (classId == "settingbutton")
            {
                Layout_ScaleUp_Animation(3);
            }
        }

        void ImageButton_Released(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var classId = Imagebutton.ClassId;
            if (classId == "messagebutton")
            {
                Layout_ScaleDown_Animation(2);
                GoTo_MessagePage();
            }
            else if (classId == "settingbutton")
            {
                Layout_ScaleDown_Animation(3);
                GoTo_SettingPage();
            }
        }
        //MainMenuPage로 이동
        async void GoTo_FeedPage()
        {
            try
            {
                NavigationPage FeedPage = new NavigationPage(new FeedPage());
                NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
                await MainMenuPage.PushAsync(FeedPage);
                await page.FadeTo(0.5, 100);
                Application.Current.MainPage = FeedPage;
            }
            catch(Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "Okay!");
            }

        }

        //MainMenuPage로 이동
        async void GoTo_SchedulePage()
        {
            NavigationPage SchedulePage = new NavigationPage(new SchedulePage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await MainMenuPage.PushAsync(SchedulePage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = SchedulePage;
        }
        //MainMenuPage로 이동
        async void GoTo_MessagePage()
        {
            NavigationPage MessagePage = new NavigationPage(new ContactPage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await MainMenuPage.PushAsync(MessagePage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MessagePage;
        }
        //MainMenuPage로 이동
        async void GoTo_SettingPage()
        {
            NavigationPage SettingPage = new NavigationPage(new SettingPage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await MainMenuPage.PushAsync(SettingPage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = SettingPage;
        }

        async void ScaleUp_Animation(int i)
        {
            await layouts[i].ScaleTo(1.06, 50);
        }

        async void ScaleDown_Animation(int i)
        {
            await layouts[i].ScaleTo(1, 50);
        }

        async void Layout_ScaleUp_Animation(int i)
        {
            await layouts[i].ScaleTo(1.1, 50);
        }

        async void Layout_ScaleDown_Animation(int i)
        {
            await layouts[i].ScaleTo(1, 50);
        }
    }
}
