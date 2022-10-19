using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Syncfusion.SfCalendar.XForms.iOS;
using UIKit;

namespace AIO.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            SfCalendarRenderer.Init();

            ////init tesseract
            //var container = TinyIoCContainer.Current;
            //container.Register<IDevice>(AppleDevice.CurrentDevice);
            //container.Register<ITesseractApi>((cont, parameters) =>
            //{
            //    return new TesseractApi();
            //});
            //Resolver.SetResolver(new TinyResolver(container));
            //Tesseract 사용 중지 - 자세한 사항은 Notion에서

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
