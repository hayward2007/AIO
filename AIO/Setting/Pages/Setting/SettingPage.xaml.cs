using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace AIO
{
    public partial class SettingPage : ContentPage
    {
        public Frame[] frames = new Frame[4];
        public AbsoluteLayout page = new AbsoluteLayout();
        public StackLayout ColorChange = new StackLayout();

        public static LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
        public static GradientStop gradientStop1 = new GradientStop()
        {
            Offset = 0
        };
        public static GradientStop gradientStop2 = new GradientStop()
        {
            Offset = 1
        };
        public bool ColorChangeAble = false;

        public SettingPage()
        {
            InitializeComponent();
            page = FindByName("settingpage") as AbsoluteLayout;
            frames[0] = FindByName("home") as Frame;
            frames[1] = FindByName("search") as Frame;
            frames[2] = FindByName("new") as Frame;
            frames[3] = FindByName("refresh") as Frame;
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
                    page.Background = linearGradientBrush;
                }
            }

            page.FadeTo(1, 100);
        }
        public event EventHandler<Color> PickedColorChanged;

        public static readonly BindableProperty PickedColorProperty
            = BindableProperty.Create(
                nameof(PickedColor),
                typeof(Color),
                typeof(SettingPage));

        public Color PickedColor
        {
            get { return (Color)GetValue(PickedColorProperty); }
            set { SetValue(PickedColorProperty, value); }
        }

        private SKPoint _lastTouchPoint = new SKPoint();

        private void SkCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var skImageInfo = e.Info;
            var skSurface = e.Surface;
            var skCanvas = skSurface.Canvas;

            var skCanvasWidth = skImageInfo.Width;
            var skCanvasHeight = skImageInfo.Height;

            skCanvas.Clear(SKColors.White);

            // Draw gradient rainbow Color spectrum
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                // Initiate the primary Color list
                // picked up from Google Web Color Picker
                var colors = new SKColor[]
                {
                    new SKColor(255, 0, 0), // Red
					new SKColor(255, 255, 0), // Yellow
					new SKColor(0, 255, 0), // Green (Lime)
					new SKColor(0, 255, 255), // Aqua
					new SKColor(0, 0, 255), // Blue
					new SKColor(255, 0, 255), // Fuchsia
					new SKColor(255, 0, 0), // Red
				};

                // create the gradient shader between Colors
                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(skCanvasWidth, 0),
                    colors,
                    null,
                    SKShaderTileMode.Clamp))
                {
                    paint.Shader = shader;
                    skCanvas.DrawPaint(paint);
                }
            }

            // Draw darker gradient spectrum
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                // Initiate the darkened primary color list
                var colors = new SKColor[]
                {
                    SKColors.Transparent,
                    SKColors.Black
                };

                // create the gradient shader 
                using (var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(0, skCanvasHeight),
                    colors,
                    null,
                    SKShaderTileMode.Clamp))
                {
                    paint.Shader = shader;
                    skCanvas.DrawPaint(paint);
                }
            }

            // Picking the Pixel Color values on the Touch Point

            // Represent the color of the current Touch point
            SKColor touchPointColor;

            using (SKBitmap bitmap = new SKBitmap(skImageInfo))
            {
                // get the pixel buffer for the bitmap
                IntPtr dstpixels = bitmap.GetPixels();

                // read the surface into the bitmap
                skSurface.ReadPixels(skImageInfo,
                    dstpixels,
                    skImageInfo.RowBytes,
                    (int)_lastTouchPoint.X, (int)_lastTouchPoint.Y);

                // access the color
                touchPointColor = bitmap.GetPixel(0, 0);
            }

            // Painting the Touch point
            using (SKPaint paintTouchPoint = new SKPaint())
            {
                paintTouchPoint.Style = SKPaintStyle.Fill;
                paintTouchPoint.Color = SKColors.White;
                paintTouchPoint.IsAntialias = true;

                // Outer circle (Ring)
                var outerRingRadius =
                    ((float)skCanvasWidth / (float)skCanvasHeight) * (float)18;
                skCanvas.DrawCircle(
                    _lastTouchPoint.X,
                    _lastTouchPoint.Y,
                    outerRingRadius, paintTouchPoint);

                // Draw another circle with picked color
                paintTouchPoint.Color = touchPointColor;

                // Outer circle (Ring)
                var innerRingRadius =
                    ((float)skCanvasWidth / (float)skCanvasHeight) * (float)12;
                skCanvas.DrawCircle(
                    _lastTouchPoint.X,
                    _lastTouchPoint.Y,
                    innerRingRadius, paintTouchPoint);
            }

            // Set selected color
            PickedColor = touchPointColor.ToFormsColor();
            PickedColorChanged?.Invoke(this, PickedColor);
            if (ColorChangeAble == true)
            {
                var SkCanvasView = (SKCanvasView)sender;
                if (SkCanvasView.ClassId == "1")
                {
                    foreach (Frame frame in frames)
                    {
                        frame.BackgroundColor = PickedColor;
                    }
                }
                else if (SkCanvasView.ClassId == "2")
                {

                }
                else if (SkCanvasView.ClassId == "3")
                {
                    if (App.user_login_data[0].background_color_mode == "solid")
                    {
                        page.BackgroundColor = PickedColor;
                    }
                    else if (App.user_login_data[0].background_color_mode == "gradient")
                    {
                        gradientStop1.Color = PickedColor;
                        page.Background = linearGradientBrush;
                    }
                }
                else if (SkCanvasView.ClassId == "4")
                {
                    gradientStop2.Color = PickedColor;
                    page.Background = linearGradientBrush;
                }
            }
        }

        private void SkCanvasView_OnTouch(object sender, SKTouchEventArgs e)
        {
            _lastTouchPoint = e.Location;

            var SkCanvasView = (SKCanvasView)sender;

            var canvasSize = SkCanvasView.CanvasSize;

            if ((e.Location.X > 0 && e.Location.X < canvasSize.Width) &&
                (e.Location.Y > 0 && e.Location.Y < canvasSize.Height))
            {
                e.Handled = true;
                SkCanvasView.InvalidateSurface();
            }
        }

        async void Button_Pressed(System.Object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            await Button.ScaleTo(1.1, 50);
        }

        async void Button_Released(System.Object sender, System.EventArgs e)
        {
            var Button = (Button)sender;
            _ = Button.ScaleTo(1, 50);
            ColorChange = FindByName("ColorChangeLayout" + Button.ClassId) as StackLayout;
            //색상 모드 변경 버튼
            if(Button.Text == "단색")
            {
                Button.Text = "그라데이션";
                if(Button == Button_Color_Mode_Button)
                {
                    App.user_login_data[0].button_color_mode = "gradient";
                }
                else if(Button == Backgroud_Color_Mode_Button)
                {
                    App.user_login_data[0].background_color_mode = "gradient";
                }
            }
            else if (Button.Text == "그라데이션")
            {
                Button.Text = "단색";
                if (Button == Button_Color_Mode_Button)
                {
                    App.user_login_data[0].button_color_mode = "solid";
                }
                else if (Button == Backgroud_Color_Mode_Button)
                {
                    App.user_login_data[0].background_color_mode = "solid";
                }
            }

            //색상 변경 버튼
            if (Button.Text == "색상 변경")
            {
                if (Button.ClassId == "4" && App.user_login_data[0].background_color_mode != "gradient")
                {
                    await DisplayAlert("오류", "단색 모드입니다!", "확인");
                    return;
                }
                if (Button.ClassId == "2" && App.user_login_data[0].button_color_mode != "gradient")
                {
                    await DisplayAlert("오류", "단색 모드입니다!", "확인");
                    return;
                }
                Button.WidthRequest = 115;
                Button.Text = "완료";
                Button button = new Button()
                {
                    Text = "취소",
                    WidthRequest = 115,
                    BorderColor = Color.Black,
                    BorderWidth = 1,
                    CornerRadius = 10,
                    ClassId = Button.ClassId
                };
                button.Pressed += Button_Pressed;
                button.Released += Button_Released;
                ColorChange.Children.Add(button);
                var SkCanvasView = FindByName("SkCanvasView" + Button.ClassId) as SKCanvasView;
                SkCanvasView.IsEnabled = true;
                ColorChangeAble = true;
            }
            else if (Button.Text == "완료" || Button.Text == "취소")
            {
                string text = Button.Text;
                int index = int.Parse(Button.ClassId);
                ColorChange.Children.Clear();
                Button button = new Button()
                {
                    Text = "색상 변경",
                    WidthRequest = 240,
                    BorderColor = Color.Black,
                    BorderWidth = 1,
                    CornerRadius = 10,
                    ClassId = index.ToString()
                };
                button.Pressed += Button_Pressed;
                button.Released += Button_Released;
                ColorChange.Children.Add(button);
                var SkCanvasView = FindByName("SkCanvasView" + index) as SKCanvasView;
                SkCanvasView.IsEnabled = false;
                if (text == "완료")
                {
                    if (index == 1)
                    {
                        App.user_login_data[0].button_solid_color = frames[0].BackgroundColor.ToHex();
                    }
                    else if (index == 2)
                    {

                    }
                    else if (index == 3)
                    {
                        App.user_login_data[0].background_solid_color = gradientStop1.Color.ToHex();
                    }
                    else if (index == 4)
                    {
                        App.user_login_data[0].background_gradient_color = gradientStop2.Color.ToHex();
                    }
                }
                else if (text == "취소")
                {
                    page.BackgroundColor = Color.FromHex(App.user_login_data[0].background_solid_color);
                }
                ColorChangeAble = false;
            }
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
            NavigationPage SettingPage = new NavigationPage(new SettingPage());
            NavigationPage MainMenuPage = new NavigationPage(new MainMenuPage());
            await SettingPage.PopAsync();
            await page.FadeTo(0.5, 100);
            Application.Current.MainPage = MainMenuPage;
        }
        private async void Search()
        {
        }
        private async void New()
        {
        }
        private async void Refresh()
        {
        }

        async void LogOut_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1.1, 50);
        }

        async void LogOut_Released(System.Object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1, 50);

            var task = await DisplayAlert("알림", "로그아웃 하시겠습니까?", "로그아웃", "취소");

            if(task == true)
            {
                File.Delete(App.json_path);

                NavigationPage SettingPage = new NavigationPage(new SettingPage());
                NavigationPage LoginPage = new NavigationPage(new LoginPage());
                await page.FadeTo(0, 100);
                await SettingPage.PopToRootAsync();
                Application.Current.MainPage = LoginPage;
            }
        }
    }
}