using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Model;
using WpfApp1.View_Model;

using Microsoft.Web.WebView2.Core;
using System.Diagnostics;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for BrowserControl.xaml
    /// </summary>
    public partial class BrowserControl : UserControl
    {
        public static readonly DependencyProperty BrowserClassProperty =
            DependencyProperty.Register("BrowserClass", typeof(Browser), typeof(BrowserControl), new PropertyMetadata(new Browser()));

        public Browser BrowserClass
        {
            get => (Browser)GetValue(BrowserClassProperty);
            set => SetValue(BrowserClassProperty, value);
        }

        public static readonly DependencyProperty AddTabProperty =
            DependencyProperty.Register("AddTab", typeof(ICommand), typeof(BrowserControl), new UIPropertyMetadata(null));

        public ICommand AddTab
        {
            get => (ICommand)GetValue(AddTabProperty);
            set => SetValue(AddTabProperty, value);
        }

        public BrowserControl()
        {
            InitializeComponent();
        }
    }
}
