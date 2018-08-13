using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Linq;

namespace FiiiCoin.Wallet.Win.CustomControls
{
    public class ImageButton : ToggleButton
    {
        public ImageButton()
        {
            var resourceLoactor = new Uri("pack://application:,,,/FiiiCoin.Wallet.Win;component/CustomControls/ImageButton/ImageButtonStyle.xaml");
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = resourceLoactor;
            if (!Application.Current.Resources.MergedDictionaries.Any(x => x.Source == resourceLoactor))
                Application.Current.Resources.MergedDictionaries.Add(resource);
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(default(ImageSource)));
        
    }
}
