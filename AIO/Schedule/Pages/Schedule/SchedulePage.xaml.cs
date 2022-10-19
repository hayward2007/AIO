using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Syncfusion.SfCalendar.XForms;
using Xamarin.Forms;


namespace AIO
{
    public partial class SchedulePage : ContentPage
    {
        public ActivityIndicator indicator = new ActivityIndicator();
        public Frame[] frames = new Frame[4];
        public SfCalendar calendar = new SfCalendar();
        public StackLayout page = new StackLayout();
        public CalendarEventCollection col = new CalendarEventCollection();
        public AbsoluteLayout search_frame = new AbsoluteLayout();
        public Entry search_entry = new Entry();

        public bool is_searching = false;
        public List<int> read_pages = new List<int>();
        public static List<Schedule> Schedules = new List<Schedule>();

        public SchedulePage()
        {
            InitializeComponent();

            page = FindByName("schedulepage") as StackLayout;

            search_frame = FindByName("Search_frame") as AbsoluteLayout;
            search_entry = FindByName("SearchEntry") as Entry;

            calendar = FindByName("sfcalendar") as SfCalendar;
            calendar.MinDate = new DateTime(DateTime.Now.Year, 1, 1);
            calendar.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);
            read_pages.Add(DateTime.Now.Month);

            indicator = FindByName("loading_indicator") as ActivityIndicator;

            frames[0] = FindByName("home") as Frame;
            frames[1] = FindByName("search") as Frame;
            frames[2] = FindByName("new") as Frame;
            frames[3] = FindByName("refresh") as Frame;

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
        
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (Schedules.Count == 0)
                {
                    string data;
                    string path = "https://firebasestorage.googleapis.com/v0/b/sunae-3cf06.appspot.com/o/Schedule_Json%2FData.json?alt=media&token=264af6d5-5806-4ab8-b6ee-48373abfd526";
                    using (WebClient wc = new WebClient())
                    {
                        data = await wc.DownloadStringTaskAsync(path);
                    }
                    Schedules.AddRange(JsonConvert.DeserializeObject<List<Schedule>>(data));
                }
                DataSync();
            });
        }

        public async void DataSync()
        {
            calendar.DataSource.Clear();

            foreach (var data in Schedules)
            {
                if (data.Month == DateTime.Now.Month)
                {
                    foreach (var _event in data.Event)
                    {
                        var schedule_event = new CalendarInlineEvent();
                        schedule_event.Subject = _event.Event_Name;
                        schedule_event.StartTime = DateTime.Parse(data.Month.ToString() + "/" + _event.Start_Date.ToString());
                        schedule_event.EndTime = DateTime.Parse(data.Month.ToString() + "/" + _event.End_Date.ToString());
                        calendar.DataSource.Add(schedule_event);
                    }
                }
            }
            await indicator.FadeTo(0, 100, Easing.SinIn);
            indicator.IsRunning = false;
        }

        async void sfcalendar_MonthChanged(System.Object sender, Syncfusion.SfCalendar.XForms.MonthChangedEventArgs e)
        {
            if(is_searching)
            {
                return;
            }
            var current_page = int.Parse(e.CurrentValue.ToString().Substring(5, 2));
            if (read_pages.Contains(current_page))
            {
                return;
            }

            read_pages.Add(current_page);

            indicator.IsRunning = true;
            await indicator.FadeTo(1, 100);
            foreach (var data in Schedules)
            {
                if (data.Month == current_page)
                {
                    foreach (var _event in data.Event)
                    {
                        var schedule_event = new CalendarInlineEvent();
                        schedule_event.Subject = _event.Event_Name;
                        schedule_event.StartTime = DateTime.Parse(data.Month.ToString() + "/" + _event.Start_Date.ToString());
                        schedule_event.EndTime = schedule_event.StartTime = DateTime.Parse(data.Month.ToString() + "/" + _event.End_Date.ToString());
                        calendar.DataSource.Add(schedule_event);

                    }
                }
            }
            await indicator.FadeTo(0, 100, Easing.SinIn);
            indicator.IsRunning = false;
        }

        async void ImageButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var Frame = Imagebutton.Parent as Frame;
            await Frame.ScaleTo(1.1, 50);
        }

        async void ImageButton_Realeased(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            var classId = Imagebutton.ClassId;
            var Frame = Imagebutton.Parent as Frame;
            await Frame.ScaleTo(1, 50);
            if (classId == "homebutton")
            {
                MainMenuPage();
            }
            else if (classId == "searchbutton")
            {
                Search();
            }
            else if (classId == "newbutton")
            {
                New();
            }
            else if (classId == "refreshbutton")
            {
                Refresh();
            }
        }

        async void SearchButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            await Imagebutton.ScaleTo(0.8, 50);
        }

        async void SearchButton_Released(System.Object sender, System.EventArgs e)
        {
            var Imagebutton = (ImageButton)sender;
            await Imagebutton.ScaleTo(0.6, 50);
            indicator.IsRunning = true;
            await indicator.FadeTo(1, 100);
            await search_frame.TranslateTo(0, -125, 100);
            calendar.DataSource.Clear();
            is_searching = true;

            var search_string = search_entry.Text;

            foreach (var data in Schedules)
            {
                foreach (var _event in data.Event)
                {
                    if (_event.Event_Name.Contains(search_string))
                    {
                        var current_event = new CalendarInlineEvent()
                        {
                            Subject = _event.Event_Name,
                            StartTime = DateTime.Parse(data.Month.ToString() + "/" + _event.Start_Date.ToString()),
                            EndTime = DateTime.Parse(data.Month.ToString() + "/" + _event.End_Date.ToString())
                        };

                        if (!col.Contains(current_event))
                        {
                            current_event.Color = Color.Red;
                            col.Add(current_event);
                        }
                        calendar.DataSource = col;
                    }
                }
            }
            await indicator.FadeTo(0, 100);
            indicator.IsRunning = false;
        }


        private async void MainMenuPage()
        {
            NavigationPage SchedulePage = new NavigationPage(new SchedulePage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await SchedulePage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }

        private async void Search()
        {
            if (search_frame.TranslationY == -125)
            {
                await search_frame.TranslateTo(0, -30, 100);
            }
            else
            {
                await search_frame.TranslateTo(0, -125, 100);
            }
        }

        private async void New()
        {

        }

        private async void Refresh()
        {
            NavigationPage SchedulePage = new NavigationPage(new SchedulePage());
            NavigationPage NewSchedulePage = new NavigationPage(new SchedulePage());
            await SchedulePage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = NewSchedulePage;
        }


    }
}