using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace AIO
{
    public static class Global
    {
        public static int _totalnum = 0;
        public static int _indexnum = 0;
        public static double _FrameHeight = 0;
        public static double _layoutY = 0;
        public static bool _functionbool = false;
        public static bool _finishbool = false;
        public static void Init()
        {
            _totalnum = 0;
            _functionbool = false;
            _finishbool = false;
        }
    }

    public partial class FeedPage : ContentPage
    {
        public List<NoticeBoard> _noticeBoards = new List<NoticeBoard>();
        public List<Notice> _notice = new List<Notice>();

        public Frame[] frames = new Frame[4];

        public AbsoluteLayout page = new AbsoluteLayout();

        public AbsoluteLayout Search = new AbsoluteLayout();

        public ScrollView Scroll = new ScrollView();

        public Entry Search_Entry = new Entry();

        public StackLayout feed = new StackLayout();

        public ActivityIndicator indicator = new ActivityIndicator();

        public int DataNum = 1;
        public bool Search_Boolean = false;
        public bool DoubleTap = false;
        public bool on_search = false;

        public FeedPage()
        {
            InitializeComponent();
            Global.Init();
            page = FindByName("feedpage") as AbsoluteLayout;
            feed = FindByName("feedlayout") as StackLayout;
            frames[0] = FindByName("home") as Frame;
            frames[1] = FindByName("search") as Frame;
            frames[2] = FindByName("new") as Frame;
            frames[3] = FindByName("refresh") as Frame;
            Scroll = FindByName("ScrollView") as ScrollView;
            Search = FindByName("Search_frame") as AbsoluteLayout;
            Search_Entry = FindByName("writeSearch") as Entry;
            indicator = FindByName("loading_indicator") as ActivityIndicator;
            indicator.IsRunning = true;

            if (App.user_login_data[0].button_solid_color != "00000000")
            {
                foreach (Frame frame in frames)
                {
                    frame.BackgroundColor = Color.FromHex(App.user_login_data[0].button_solid_color);
                }
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_notice.Count == 0)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string data;
                    using (WebClient wc = new WebClient())
                    {
                        data = await wc.DownloadStringTaskAsync("https://firebasestorage.googleapis.com/v0/b/sunae-3cf06.appspot.com/o/Feed_Json%2FData.json?alt=media&token=36825494-9a62-44cb-9729-bb077189e2d0");
                    }
                    _notice.AddRange(JsonConvert.DeserializeObject<List<Notice>>(data));
                    DataSync();
                    indicator.IsRunning = false;
                });
            }
        }

        public async Task Run()
        {
            int _nullnum = Global._totalnum;
            for (int i = _nullnum; i < _nullnum + 3; i++)
            {
                string title;
                string bodyline;
                double height = 250;
                int ch_num = 0;
                if (_notice[i].title.Length < 20)
                {
                    title = _notice[i].title;
                }
                else
                {
                    title = _notice[i].title.Substring(0, 18) + "...";
                }
                foreach (char ch in _notice[i].body)
                {
                    if (ch == '\n')
                    {
                        ch_num += 15;
                    }
                    else
                    {
                        ch_num++;
                    }
                }
                if (_notice[i].body == "." || _notice[i].body == "")
                {
                    bodyline = "아무 내용 없음";
                    height = 30;
                }
                else if (ch_num <= 300)
                {
                    bodyline = _notice[i].body;
                    height = 30 + ch_num / 25 * 10;
                }
                else
                {
                    bodyline = _notice[i].body;
                }
                NoticeBoard board = new NoticeBoard();

                board._layout = new AbsoluteLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = height + 50,
                    ClassId = Global._totalnum.ToString(),
                    Opacity = 0
                };
                board._mainFrame = new Frame()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = height + 50,
                    WidthRequest = 300,
                    CornerRadius = 30,
                    BackgroundColor = Color.Beige,
                    Opacity = 0.7,
                    //BackgroundColor = Color.FromHex("48466D"),
                    HasShadow = false
                };
                board._titleLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 340,
                    TranslationY = 25,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = title,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    //TextColor = Color.FromHex("D4E2D4")
                };
                board._bodyLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 300,
                    HeightRequest = height,
                    TranslationX = 20,
                    TranslationY = 70,
                    Text = " " + bodyline,
                    //TextColor = Color.FromHex("D4E2D4"),
                };
                board.Bind();
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
                tapGestureRecognizer.NumberOfTapsRequired = 2;
                board._layout.GestureRecognizers.Add(tapGestureRecognizer);
                _noticeBoards.Add(board);
                feed.Children.Add(board._layout);
                if (indicator.IsRunning == true)
                {
                    await indicator.FadeTo(0, 100);
                    indicator.IsRunning = false;
                }
                await board._layout.FadeTo(1, 500);
                Global._totalnum++;

            }
            Global._functionbool = false;
            indicator.IsRunning = false;
        }

        async void ScrollView_Scrolled(System.Object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            if (Global._totalnum == 0)
            {
                return;
            }

            var scrollView = sender as ScrollView;
            var scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;


            if (scrollingSpace < Scroll.ScrollY & Global._functionbool == false & on_search == false)
            {
                Global._functionbool = true;
                await Run();
            }
        }

        async void OnTapGestureRecognizerTapped(System.Object sender, System.EventArgs e)
        {
            if (DoubleTap == false)
            {
                AbsoluteLayout layout = sender as AbsoluteLayout;
                int num = int.Parse(layout.ClassId);
                Global._layoutY = layout.Y;
                Global._indexnum = num;
                Global._FrameHeight = layout.Height;
                await Scroll.ScrollToAsync(0, layout.Y - 50, true);
                var animate_layout = new Animation(d => layout.HeightRequest = d, 300, 845);
                var animate_frame = new Animation(d => _noticeBoards[num]._mainFrame.HeightRequest = d, 300, 610);
                animate_layout.Commit(layout, "BarGraph", 16, 200);
                animate_frame.Commit(_noticeBoards[num]._mainFrame, "BarGraph", 16, 200);
                await Task.Delay(250);
                AbsoluteLayout feedlayout = new AbsoluteLayout()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HeightRequest = 845,
                    WidthRequest = 390,
                };
                feedlayout.Children.Add(new Frame()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    TranslationX = 25,
                    HeightRequest = 610,
                    WidthRequest = 300,
                    CornerRadius = 30,
                    BackgroundColor = Color.Beige,
                    Opacity = 0.7,
                    //BackgroundColor = Color.FromHex("48466D"),
                    HasShadow = false,
                });
                feedlayout.Children.Add(new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 300,
                    TranslationX = 45,
                    TranslationY = 20,
                    HorizontalTextAlignment = TextAlignment.Center,
                    //TextColor = Color.FromHex("D4E2D4"),
                    Text = _notice[Global._indexnum].title,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold
                });
                feedlayout.Children.Add(new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    //TextColor = Color.FromHex("D4E2D4"),
                    HeightRequest = 750,
                    WidthRequest = 300,
                    TranslationX = 45,
                    TranslationY = 90,
                    Text = " " + _notice[Global._indexnum].body
                });
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
                tapGestureRecognizer.NumberOfTapsRequired = 2;
                feedlayout.GestureRecognizers.Add(tapGestureRecognizer);
                Scroll.Content = feedlayout;
                Scroll.HeightRequest = 845;
                await Scroll.ScrollToAsync(0, -50, false);
                DoubleTap = true;
            }
            else
            {
                await Scroll.ScrollToAsync(0, -50, true);
                Scroll.Content = feed;
                await Scroll.ScrollToAsync(0, Global._layoutY - 50, false);
                await Task.Delay(250);
                var animate_layout = new Animation(d => _noticeBoards[Global._indexnum]._layout.HeightRequest = d, 900, Global._FrameHeight);
                var animate_frame = new Animation(d => _noticeBoards[Global._indexnum]._mainFrame.HeightRequest = d, 900, Global._FrameHeight);
                animate_layout.Commit(_noticeBoards[Global._indexnum]._layout, "BarGraph", 16, 200);
                animate_frame.Commit(_noticeBoards[Global._indexnum]._mainFrame, "BarGraph", 16, 200);

                DoubleTap = false;
            }
        }

        private async void DataSync()
        {
            int _nullnum = Global._totalnum;
            for (int i = _nullnum; i < _nullnum + 10; i++)
            {
                string title;
                string bodyline;
                double height = 250;
                int ch_num = 0;
                if (_notice[i].title.Length < 20)
                {
                    title = _notice[i].title;
                }
                else
                {
                    title = _notice[i].title.Substring(0, 18) + "...";
                }
                foreach (char ch in _notice[i].body)
                {
                    if (ch == '\n')
                    {
                        ch_num += 15;
                    }
                    else
                    {
                        ch_num++;
                    }
                }
                if (_notice[i].body == "." || _notice[i].body == "")
                {
                    bodyline = "아무 내용 없음";
                    height = 30;
                }
                else if (ch_num <= 300)
                {
                    bodyline = _notice[i].body;
                    height = 30 + ch_num / 25 * 10;
                }
                else
                {
                    bodyline = _notice[i].body;
                }
                NoticeBoard board = new NoticeBoard();

                board._layout = new AbsoluteLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = height + 50,
                    ClassId = Global._totalnum.ToString(),
                    Opacity = 0
                };
                board._mainFrame = new Frame()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = height + 50,
                    WidthRequest = 300,
                    CornerRadius = 30,
                    BackgroundColor = Color.Beige,
                    Opacity = 0.7,
                    //BackgroundColor = Color.FromHex("48466D"),
                    HasShadow = false
                };
                board._titleLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 340,
                    TranslationY = 25,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = title,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    //TextColor = Color.FromHex("D4E2D4")
                };
                board._bodyLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 300,
                    HeightRequest = height,
                    TranslationX = 20,
                    TranslationY = 70,
                    Text = " " + bodyline,
                    //TextColor = Color.FromHex("D4E2D4"),
                };
                board.Bind();
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
                tapGestureRecognizer.NumberOfTapsRequired = 2;
                board._layout.GestureRecognizers.Add(tapGestureRecognizer);
                _noticeBoards.Add(board);
                feed.Children.Add(board._layout);
                if (indicator.IsRunning == true)
                {
                    await indicator.FadeTo(0, 100);
                    indicator.IsRunning = false;
                }
                await board._layout.FadeTo(1, 100);
                Global._totalnum++;

            }
            Global._functionbool = false;
            indicator.IsRunning = false;

        }
        async void ImageButton_Pressed(System.Object sender, System.EventArgs e)
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
            else if (classId == "gosearchbutton")
            {
                await Imagebutton.ScaleTo(0.8, 50);
            }
        }
        async void ImageButton_Realeased(System.Object sender, System.EventArgs e)
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
                if (Search_Boolean == false)
                {
                    await Search.TranslateTo(0, 0, 100);
                    Search_Boolean = true;
                }
                else
                {
                    await Search.TranslateTo(0, -110, 100);
                    Search_Boolean = false;
                }
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
            else if (classId == "gosearchbutton")
            {
                await Imagebutton.ScaleTo(0.6, 50);
                await Search.TranslateTo(0, -110, 100);
                Search_engine();
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
            NavigationPage SchedulePage = new NavigationPage(new SchedulePage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await SchedulePage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }
        public async void Search_engine()
        {
            indicator.IsRunning = true;
            List<int> index = new List<int>();
            if (Search_Entry.Text == null)
            {
                await DisplayAlert("오류","검색어가 없습니다","확인");
                return;
            }
            string title_words = Search_Entry.Text.Replace(" ", "");
            for (int i = _notice.Count - 1; i >= 0; i--)
            {
                string current_words = _notice[i].title.Replace(" ", "");
                if (current_words.Contains(title_words) == true)
                {
                    index.Add(i);
                }
            }
            feed.Children.Clear();
            on_search = true;
            if (index.Count == 0)
            {
                feed.Children.Add(new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 300,
                    HeightRequest = 300,
                    TranslationX = 70,
                    TranslationY = 70,
                    Text = "검색 결과가 없습니다",
                    TextColor = Color.FromHex("D4E2D4"),
                });
            }
            foreach (int i in index)
            {
                string title;
                string bodyline;
                double height = 250;
                int ch_num = 0;
                if (_notice[i].title.Length < 20)
                {
                    title = _notice[i].title;
                }
                else
                {
                    title = _notice[i].title.Substring(0, 18) + "...";
                }
                foreach (char ch in _notice[i].body)
                {
                    if (ch == '\n')
                    {
                        ch_num += 15;
                    }
                    else
                    {
                        ch_num++;
                    }
                }
                if (_notice[i].body == "." || _notice[i].body == "")
                {
                    bodyline = "아무 내용 없음";
                    height = 30;
                }
                else if (ch_num <= 300)
                {
                    bodyline = _notice[i].body;
                    height = 30 + ch_num / 25 * 10;
                }
                else
                {
                    bodyline = _notice[i].body;
                }
                NoticeBoard board = new NoticeBoard();

                board._layout = new AbsoluteLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = height + 50,
                    ClassId = Global._totalnum.ToString()
                };
                board._mainFrame = new Frame()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = height + 50,
                    WidthRequest = 300,
                    CornerRadius = 30,
                    BackgroundColor = Color.Beige,
                    Opacity = 0.7,
                    //BackgroundColor = Color.FromHex("48466D"),
                    HasShadow = false
                };
                board._titleLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 340,
                    TranslationY = 25,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = title,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    //TextColor = Color.FromHex("D4E2D4")
                };
                board._bodyLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    WidthRequest = 300,
                    HeightRequest = height,
                    TranslationX = 20,
                    TranslationY = 70,
                    Text = " " + bodyline,
                    //TextColor = Color.FromHex("D4E2D4"),
                };
                board.Bind();
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
                tapGestureRecognizer.NumberOfTapsRequired = 2;
                board._layout.GestureRecognizers.Add(tapGestureRecognizer);
                _noticeBoards.Add(board);
                feed.Children.Add(board._layout);
            }
            Frame null_area = new Frame()
            {
                BackgroundColor = Color.Transparent,
                HeightRequest = 150
            };
            feed.Children.Add(null_area);
            indicator.IsRunning = false;
            await feed.FadeTo(100, 200, Easing.SinIn);
        }

        private async void New()
        {
        }

        private async void Refresh()
        {
            Global._totalnum = 0;
            on_search = false;
            feed.Children.Clear();
            DataSync();
            await Scroll.ScrollToAsync(0, -50, false);
        }
    }
}