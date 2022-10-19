using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AIO
{
    public partial class ContactPage : ContentPage
    {
        public Frame[] frames = new Frame[4];
        public StackLayout page = new StackLayout();
        public StackLayout labels = new StackLayout();
        public AbsoluteLayout search_frame = new AbsoluteLayout();
        public ActivityIndicator loading = new ActivityIndicator();
        public static int Current_Room_Index;
        public static List<MessageClass> message_data = new List<MessageClass>();
        public static List<User> user_message_data = new List<User>();

        public ContactPage()
        {
            InitializeComponent();
            search_frame = FindByName("Search_frame") as AbsoluteLayout;
            labels = FindByName("StackLayout") as StackLayout;
            loading = FindByName("loading_indicator") as ActivityIndicator;
            frames[0] = FindByName("home") as Frame;
            frames[1] = FindByName("search") as Frame;
            frames[2] = FindByName("new") as Frame;
            frames[3] = FindByName("refresh") as Frame;
            page = FindByName("contactpage") as StackLayout;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Firebase.Database.FirebaseClient realtime_db = new Firebase.Database.FirebaseClient("https://sunae-3cf06-default-rtdb.firebaseio.com/",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () =>
                            Task.FromResult("3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z")
            });
            client = new FireSharp.FirebaseClient(server);


            Device.BeginInvokeOnMainThread(async () =>
            {
                var room = (await realtime_db.Child("message/")
                .OnceAsync<DownloadRoomClass>())
                .Select(name => new DownloadRoomClass()
                {
                    room_name = name.Object.room_name
                });

                var room_list = room.ToList();

                for (int i = 0; i < room_list.Count; i++)
                {
                    var json = await client.GetAsync("message/" + room_list[i].room_name + "/user");
                    var list = JsonConvert.DeserializeObject<List<string>>(json.Body);

                    var add_list = new List<User>();
                    foreach (var data in list)
                    {
                        var user = data.Split('/');
                        if (user[0] == App.user_login_data[0].user_id && user[1] == App.user_login_data[0].user_name)
                        {
                            var room_layout = new AbsoluteLayout();
                            room_layout.Children.Add(new Label()
                            {
                                Text = room_list[i].room_name,
                                FontSize = 25,
                                FontAttributes = FontAttributes.Bold,
                                TranslationX = 90,
                                TranslationY = 3
                            });
                            room_layout.Children.Add(new Image()
                            {
                                TranslationX = 12,
                                TranslationY = 7,
                                Scale = 1.5,
                                Source = ImageSource.FromResource("AIO.Images.messages.png")
                            });
                            var room_frame = new Frame()
                            {
                                WidthRequest = 260,
                                HeightRequest = 60,
                                CornerRadius = 25,
                                HasShadow = false,
                                Content = room_layout,
                                Opacity = 0,
                                ClassId = i.ToString(),
                                //BackgroundColor = Color.Beige
                            };

                            var tg = new TapGestureRecognizer();
                            tg.Tapped += FrameTapped;
                            room_frame.GestureRecognizers.Add(tg);
                            labels.Children.Add(room_frame);

                            if (loading.IsRunning == true)
                            {
                                await loading.FadeTo(0, 100);
                                loading.IsRunning = false;
                            }

                            
                            await room_frame.FadeTo(1, 100);
                        }
                        var add = new User()
                        {
                            user_id = user[0],
                            user_name = user[1],
                        };
                        add_list.Add(add);
                    }
                    message_data.Add(new MessageClass()
                    {
                        room_name = room_list[i].room_name,
                        user = add_list

                    });
                }
            });
        }


        void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var classId = Imagebutton.ClassId;
            if (classId == "homebutton")
            {
                ScaleUp_Animation(0);
            }
            else if (classId == "searchbutton")
            {
                ScaleUp_Animation(1);
            }
            else if (classId == "newbutton")
            {
                ScaleUp_Animation(2);
            }
            else if (classId == "refreshbutton")
            {
                ScaleUp_Animation(3);
            }
        }

        void ImageButton_Realeased(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var classId = Imagebutton.ClassId;

            if (classId == "homebutton")
            {
                ScaleDown_Animation(0);
                MainMenuPage();
            }
            else if (classId == "searchbutton")
            {
                ScaleDown_Animation(1);
                Search();
            }
            else if (classId == "newbutton")
            {
                ScaleDown_Animation(2);
                New();
            }
            else if (classId == "refreshbutton")
            {
                ScaleDown_Animation(3);
                Refresh();
            }
            
        }

        async void ScaleUp_Animation(int i)
        {
            await frames[i].ScaleTo(1.1, 50);
        }

        async void ScaleDown_Animation(int i)
        {
            await frames[i].ScaleTo(1, 50);
        }

        private async void MainMenuPage()
        {
            NavigationPage ContactPage = new NavigationPage(new ContactPage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await ContactPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }
        private async void MessagePage()
        {
            NavigationPage ContactPage = new NavigationPage(new ContactPage());
            NavigationPage MessagePage = new NavigationPage(new MessagePage());
            await ContactPage.PushAsync(MessagePage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MessagePage;
        }

        async void FrameTapped(object sender, EventArgs e)
        {
            Frame frame = (Frame)sender;
            await frame.ScaleTo(0.9, 50);
            await frame.ScaleTo(1, 50);
            Current_Room_Index = int.Parse(frame.ClassId);
            MessagePage();
        }

        private async void Search()
        {
            if (search_frame.TranslationY == -110)
            {
                await search_frame.TranslateTo(0, 0, 100);
            }
            else
            {
                await search_frame.TranslateTo(0, -110, 100);
            }
        }

        private async void New()
        {
            NavigationPage ContactPage = new NavigationPage(new ContactPage());
            NavigationPage NewMessage = new NavigationPage(new NewMessage());
            await ContactPage.PushAsync(NewMessage);
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = NewMessage;
        }

        private async void Refresh()
        {
            NavigationPage ContactPage = new NavigationPage(new ContactPage());
            NavigationPage NewContactPage = new NavigationPage(new ContactPage());
            await ContactPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = NewContactPage;
        }
    }
}