using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Firebase.Database;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AIO
{
    public partial class MessagePage : ContentPage
    {
        public Frame[] frames = new Frame[4];
        public Label room_name = new Label();
        public StackLayout page = new StackLayout();
        public Frame message_frame = new Frame();
        public Entry message_entry = new Entry();
        public ScrollView message_scroll = new ScrollView();
        public StackLayout message_layout = new StackLayout();
        public ActivityIndicator loading = new ActivityIndicator();


        public static List<string> Messages = new List<string>();
        public string sender;
        public double current_Y;

        public ObservableCollection<string> DatabaseItems { get; set; } = new ObservableCollection<string>(); 
        Firebase.Database.FirebaseClient realtime_db = new Firebase.Database.FirebaseClient("https://sunae-3cf06-default-rtdb.firebaseio.com/",
        new FirebaseOptions
        {
            AuthTokenAsyncFactory = () =>
                        Task.FromResult("3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z")
        });


        public MessagePage()
        {
            InitializeComponent();
            message_frame = FindByName("Message_Frame") as Frame;
            message_entry = FindByName("Message_Entry") as Entry;
            message_layout = FindByName("Message_StackLayout") as StackLayout;
            message_scroll = FindByName("Message_ScrollView") as ScrollView;
            frames[0] = FindByName("home") as Frame;
            frames[1] = FindByName("search") as Frame;
            frames[2] = FindByName("new") as Frame;
            frames[3] = FindByName("refresh") as Frame;
            loading = FindByName("loading_indicator") as ActivityIndicator;
            room_name = FindByName("Room") as Label;
            room_name.Text = ContactPage.message_data[ContactPage.Current_Room_Index].room_name;
            page = FindByName("messagepage") as StackLayout;
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

        IFirebaseClient client = new FireSharp.FirebaseClient(new FirebaseConfig()
        {
            AuthSecret = "3Nf7LXCML6UpXudo1qTWPdILga5UStNT8TOeFJ4Z",
            BasePath = "https://sunae-3cf06-default-rtdb.firebaseio.com/"
        });

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            Device.BeginInvokeOnMainThread(async () =>
            {
                
                var list = await client.GetAsync("message/" + ContactPage.message_data[ContactPage.Current_Room_Index].room_name + "/messages/");
                Messages = JsonConvert.DeserializeObject<List<string>>(list.Body);
                foreach (var data in Messages)
                {
                    try
                    {
                        await Add_Message(data);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }


                var collection = realtime_db
                .Child("message/" + room_name.Text + "/messages/")
                .AsObservable<string>()
                .Subscribe(async (dbevent) =>
                {
                    if (dbevent.Object != null)
                    {
                        Console.WriteLine(dbevent.Object);
                        if (!Messages.Contains(dbevent.Object))
                        {
                            Messages.Add(dbevent.Object);
                            await Add_Message(dbevent.Object);
                        }
                    }
                });

                await loading.FadeTo(0, 100);
                loading.IsRunning = false;
            });


        }

        async void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var frame = Imagebutton.Parent as Frame;
            await frame.ScaleTo(1.1, 50);
        }

        async void ImageButton_Realeased(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var frame = Imagebutton.Parent as Frame;
            await frame.ScaleTo(1, 50);

            var classId = Imagebutton.ClassId;
            if (classId == "homebutton")
            {
                MainMenuPage();
            }
            else if (classId == "backbutton")
            {
                GotoContactPage();
            }
            else if (classId == "sendbutton")
            {
                Move_Message_Frame();
            }
            else if (classId == "searchbutton")
            {
                Search();
            }
        }

        async void Move_Message_Frame()
        {
            if (message_frame.TranslationY == 800)
            {
                _ = message_layout.TranslateTo(0, -250, 100, Easing.SinIn);

                _ = message_frame.FadeTo(1, 100);
                await message_frame.TranslateTo(0, 550, 100, Easing.SinIn);
                message_entry.Focus();
            }
            else if (message_frame.TranslationY == 730)
            {
                _ = message_layout.TranslateTo(0, -50, 100, Easing.SinIn);

                _ = message_frame.FadeTo(0, 100);
                await message_frame.TranslateTo(0, 800, 100, Easing.SinIn);
            }
        }

        async void Message_Entry_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            _ = message_layout.TranslateTo(0, -250, 100, Easing.SinIn);

            await message_frame.TranslateTo(0, 550, 100, Easing.SinIn);
        }

        async void Message_Entry_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            _ = message_layout.TranslateTo(0, -120, 100, Easing.SinOut);

            await message_frame.TranslateTo(0, 730, 100, Easing.SinOut);
        }

        async void Send_Button_Pressed(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            await Imagebutton.ScaleTo(0.9, 50);
        }

        async void Send_Button_Released(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            await Imagebutton.ScaleTo(0.7, 50);

            _ = message_frame.FadeTo(0, 100);
            await message_frame.TranslateTo(0, 630, 100, Easing.SinIn);
            message_entry.Unfocus();
            Send_Message();
        }

        async void Send_Message()
        {
            await message_layout.TranslateTo(0, 0, 100, Easing.SinIn);

            _ = message_frame.FadeTo(0, 100);
            await message_frame.TranslateTo(0, 720, 100, Easing.SinIn);

            if (message_entry.Text == null || message_entry.Text == "")
            {
                await DisplayAlert("오류", "메세지가 비어있습니다", "확인");

                return;
            }

            await client.SetAsync("message/" + ContactPage.message_data[ContactPage.Current_Room_Index].room_name + "/messages/" + Messages.Count.ToString(), message_entry.Text + "~" + App.user_login_data[0].user_name + "/" + App.user_login_data[0].user_id + "~" + DateTime.Now.TimeOfDay.ToString().Substring(0, 5));
            message_entry.Text = String.Empty;
        }

        async Task Add_Message(string full_msg)
        {
            await Device.InvokeOnMainThreadAsync( async () =>
            {
                var texts = full_msg.Split('~');


                var message_stack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Opacity = 0
                };

                if (App.user_login_data[0].user_name + "/" + App.user_login_data[0].user_id == texts[1])
                {
                    sender = texts[1];
                    message_stack.HorizontalOptions = LayoutOptions.End;
                    message_stack.Children.Add(new Label()
                    {
                        VerticalOptions = LayoutOptions.End,
                        Text = texts[2]
                    });
                    message_stack.Children.Add(new Frame()
                    {
                        HeightRequest = 17,
                        BackgroundColor = Color.Beige,
                        CornerRadius = 17,
                        HasShadow = false,
                        Padding = 10,
                        Content = new Label()
                        {
                            Text = texts[0]
                        }
                    });
                    message_layout.Children.Add(message_stack);
                    await message_scroll.ScrollToAsync(0, message_stack.Y, true);
                    await message_stack.FadeTo(1, 25);
                }
                else
                {
                    if (texts[1] != sender)
                    {
                        sender = texts[1];
                        var sender_name = new Label()
                        {
                            Text = texts[1],
                            TranslationX = 5,
                            Opacity = 0
                        };
                        message_layout.Children.Add(sender_name);
                        await sender_name.FadeTo(1, 25);
                    }

                    message_stack.Children.Add(new Frame()
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 15,
                        BackgroundColor = Color.White,
                        CornerRadius = 17,
                        HasShadow = false,
                        Padding = 10,
                        Content = new Label()
                        {
                            Text = texts[0]
                        }
                    });
                    message_stack.Children.Add(new Label()
                    {
                        VerticalOptions = LayoutOptions.End,
                        Text = texts[2]
                    });
                    message_layout.Children.Add(message_stack);
                    await message_scroll.ScrollToAsync(0, message_stack.Y, true);
                    await message_stack.FadeTo(1, 25);
                }
            });
            
        }

        private async void MainMenuPage()
        {
            NavigationPage MessagePage = new NavigationPage(new MessagePage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await MessagePage.PopAsync();
            await MessagePage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }

        private async void GotoContactPage()
        {
            NavigationPage MessagePage = new NavigationPage(new MessagePage());
            NavigationPage ContactPage = new NavigationPage(new ContactPage());
            await MessagePage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = ContactPage;
        }

        private async void Search()
        {
        }
    }
}
