using System;
using Xamarin.Forms;

namespace AIO
{
    public class NoticeBoard
    {
        public AbsoluteLayout _layout;
        public Frame _mainFrame;
        public Label _titleLabel;
        public Label _bodyLabel;
        public void Bind()
        {
            _layout.Children.Add(_mainFrame);
            _layout.Children.Add(_titleLabel);
            _layout.Children.Add(_bodyLabel);
        }
    }
}
