using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<string, SampleModel> dict_ = new Dictionary<string, SampleModel>();
        public MainPage()
        {
            this.InitializeComponent();
            SampleModel smplMdl = new SampleModel();
            smplMdl.str1 = "pooja";
            smplMdl.str2 = "soni";
            dict_.Add("one", smplMdl);
            dict_.Add("two", smplMdl);
            dict_.Add("three", smplMdl);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await StorageHelper.WriteFileAsync<Dictionary<string, SampleModel>>("myDict", dict_, StorageHelper.StorageLocation.Local);
            MessageDialog msgDlg = new MessageDialog("File Saved Successfully. " + dict_.Count+" items received");
            await msgDlg.ShowAsync();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Dictionary<string, SampleModel> dict_retrieved = await StorageHelper.ReadFileAsync<Dictionary<string, SampleModel>>("myDict", StorageHelper.StorageLocation.Local);
            MessageDialog msgDlg = new MessageDialog("File Retrieved Successfully. " + dict_retrieved.Count+" items received");
            await msgDlg.ShowAsync();
        }
    }

    
}
