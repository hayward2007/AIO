using System;
using System.Collections.Generic;
using FireSharp.Config;
using FireSharp.Interfaces;
using Xamarin.Forms;

namespace AIO
{
    public partial class NewMessage : ContentPage
    {
        public Frame[] frames = new Frame[4];
        public Entry[] entries = new Entry[2];
        public StackLayout page = new StackLayout();
        public StackLayout searchstacklayout = new StackLayout();
        public ScrollView scroll = new ScrollView();

        public static List<string> NewMessageUserID = new List<string>();

        public NewMessage()
        {
            InitializeComponent();
            scroll = FindByName("ScrollView") as ScrollView;
            searchstacklayout = FindByName("SearchStackLayout") as StackLayout;
            page = FindByName("newmessagepage") as StackLayout;
            frames[0] = FindByName("home") as Frame;
            frames[1] = FindByName("back") as Frame;
            frames[2] = FindByName("up") as Frame;
            frames[3] = FindByName("refresh") as Frame;
            entries[0] = FindByName("NameEntry") as Entry;
            entries[1] = FindByName("SearchEntry") as Entry;
            //if (App.user_login_data[0].button_solid_color != "")
            //{
            //    foreach (Frame frame in frames)
            //    {
            //        frame.BackgroundColor = Color.FromHex(App.user_login_data[0].button_solid_color);
            //    }
            //}
            //if (App.user_login_data[0].background_color_mode == "solid")
            //{
            //    page.BackgroundColor = Color.FromHex(App.user_login_data[0].background_solid_color);
            //}
            //else if (App.user_login_data[0].background_color_mode == "gradient")
            //{
            //    if (App.user_login_data[0].background_solid_color != "" && App.user_login_data[0].background_gradient_color != "")
            //    {
            //        page.Background = SettingPage.linearGradientBrush;
            //    }
            //}
            page.FadeTo(1, 100);
        }

        FirebaseConfig server = new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        async void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var imagebutton = (ImageButton)sender;
            var frame = imagebutton.Parent as Frame;
            await frame.ScaleTo(1.1, 50);
        }

        async void ImageButton_Realeased(System.Object sender, System.EventArgs e)
        {
            var imagebutton = (ImageButton)sender;
            var classId = imagebutton.ClassId;
            var frame = imagebutton.Parent as Frame;
            await frame.ScaleTo(1.1, 50);
            if (classId == "homebutton")
            {
                MainMenuPage();
            }
            else if (classId == "backbutton")
            {
                Back();
            }
            else if (classId == "upbutton")
            {
                Up();
            }
            else if (classId == "refreshbutton")
            {
                Refresh();
            }
        }

        async void SearchButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var imageButton = (ImageButton)sender;
            await imageButton.ScaleTo(1.2, 50);
        }

        async void SearchButton_Released(System.Object sender, System.EventArgs e)
        {
            var imageButton = (ImageButton)sender;
            await imageButton.ScaleTo(1, 50);
            searchstacklayout.Children.Clear();
            string title_words = entries[1].Text.Replace(" ", "");
            foreach (var data in App.users_data)
            {
                string current_ID = data.user_id.Replace(" ", "");
                string current_name = data.user_name.Replace(" ", "");
                if (current_ID.Contains(title_words) || current_name.Contains(title_words))
                {
                    if (current_ID == App.user_login_data[0].user_id)
                    {
                        continue;
                    }

                    var name_layout = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.CenterAndExpand
                    };

                    name_layout.Children.Add(new Label()
                    {
                        Text = data.user_name
                    });

                    name_layout.Children.Add(new Label()
                    {
                        Text = data.user_study_info
                    });

                    var layout = new StackLayout();

                    layout.Children.Add(name_layout);
                    layout.Children.Add(new Label()
                    {
                        Text = "ID : " + data.user_id,
                        HorizontalOptions = LayoutOptions.Center
                    });

                    var frame = new Frame()
                    {
                        ClassId = data.user_id + "/" + data.user_name,
                        HeightRequest = 45,
                        CornerRadius = 20,
                        HasShadow = false
                    };

                    frame.Content = layout;

                    var tg = new TapGestureRecognizer();
                    tg.Tapped += FrameTapped;
                    frame.GestureRecognizers.Add(tg);

                    searchstacklayout.Children.Add(frame);
                }

            }
        }

        async void NewMessageButton_Pressed(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var frame = button.Parent as Frame;
            await frame.ScaleTo(0.9, 50);
        }

        async void NewMessageButton_Released(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var frame = button.Parent as Frame;
            await frame.ScaleTo(1, 50);

            Network_Error();

            if (entries[0].Text == "" || entries[0].Text == null)
            {
                await DisplayAlert("알림", "채팅방 이름을 적어주세요", "확인");
                return;
            }

            if (NewMessageUserID.Count == 0)
            {
                await DisplayAlert("알림", "채팅 상대를 추가해주세요", "확인");
                return;
            }

            NewMessageUserID.Add(App.user_login_data[0].user_id + "/" + App.user_login_data[0].user_name);

            var msg_lst = new List<string>();
            msg_lst.Add("메세지를 전송하여 소통하세요!");

            var room_info = new RoomClass()
            {
                room_name = entries[0].Text,
                user = NewMessageUserID,
                messages = msg_lst
            };

            client = new FireSharp.FirebaseClient(server);
            await client.SetAsync("message/" + room_info.room_name, room_info);


            await DisplayAlert("알림", "채팅방이 생성되었습니다", "확인");

            Back();
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

        async void FrameTapped(object sender, EventArgs e)
        {
            Frame frame = (Frame)sender;
            await frame.ScaleTo(0.9, 50);
            if (frame.BackgroundColor == Color.Beige)
            {
                NewMessageUserID.Remove(frame.ClassId);
                frame.BackgroundColor = Color.White;
            }
            else
            {
                frame.BackgroundColor = Color.Beige;
                NewMessageUserID.Add(frame.ClassId);
            }
            await frame.ScaleTo(1, 50);
        }

        private async void MainMenuPage()
        {
            NavigationPage SettingPage = new NavigationPage(new SettingPage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await SettingPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }
        private async void Back()
        {
            NavigationPage NewMessage = new NavigationPage(new NewMessage());
            NavigationPage ContactPage = new NavigationPage(new ContactPage());
            await NewMessage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = ContactPage;
        }

        private async void Up()
        {
            await scroll.ScrollToAsync(0, 0, true);
        }

        private async void Refresh()
        {
            NavigationPage NewMessage = new NavigationPage(new NewMessage());
            NavigationPage NewNewMessage = new NavigationPage(new NewMessage());
            await NewMessage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = NewNewMessage;
        }
    }
}

