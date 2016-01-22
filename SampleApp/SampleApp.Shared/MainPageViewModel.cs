using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SampleApp
{
    public class MainPageViewModel:INotifyPropertyChanged
    {
        private ObservableCollection<SampleModel> sampleList = new ObservableCollection<SampleModel>();
        public ObservableCollection<SampleModel> SampleList
        {
            get
            {
                return sampleList;
            }
            set
            {
                if (value == sampleList)
                    return;

                sampleList = value;
                RaisePropertyChanged("SampleList");
            }
        }

        private string content = "ViewMore";
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value == content)
                    return;

                content = value;
                RaisePropertyChanged("Content");
            }
        }

        public ButtonCommand btnCmd_viewMore { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string value)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(value));
        }

        public MainPageViewModel()
        {
            btnCmd_viewMore = new ButtonCommand(ViewMoreButtonClicked);

            for (int i = 0; i < 10; i++)
                SampleList.Add(getSampleModelItem());
        }

        private void ViewMoreButtonClicked()
        {
            if (content == "ViewMore")
            {
                for (int i = 0; i < 10; i++)
                    SampleList.Add(getSampleModelItem());

                Content = "ViewLess";
            }
            else
            {
                for (int i = sampleList.Count-1; i > 9; i--)
                    SampleList.RemoveAt(i);

                Content = "ViewMore";
            }
        }

        private SampleModel getSampleModelItem()
        {
            SampleModel smplMdl = new SampleModel();
            smplMdl.str1 = "pooja";
            smplMdl.str2 = "soni";
            return smplMdl;
        }
    }
}
