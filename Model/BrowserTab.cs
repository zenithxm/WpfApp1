using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows;
using DynamicData;
using WpfApp1.View_Model;

namespace WpfApp1.Model
{
    public class BrowserTab : ReactiveObject
    {
        private int _totalTabCreated;

        //to info available tab to select
        private int _indexAvailableTab;
        public int IndexAvailableTab
        {
            get => _indexAvailableTab;
            set 
            { 
                _indexAvailableTab = value;
                if (_indexAvailableTab < 0) _indexAvailableTab = 0;
            }
        }

        //list object tab
        private List<BrowserTabItem> _list = new();
        public List<BrowserTabItem> List
        { 
            get => _list; 
            set => _list = value;
        }

        //list history for all tab
        private List<string> _listHistory = new();
        public List<string> ListHistory
        {
            get => _listHistory;
            set => this.RaiseAndSetIfChanged(ref _listHistory, value);
        }

        //list suggestion for all tab
        private List<AddressItem> _listSuggestion = new();
        public List<AddressItem> ListSuggestion
        {
            get => _listSuggestion;
            set => this.RaiseAndSetIfChanged(ref _listSuggestion, value);
        }

        public BrowserTab ()
        {
            try
            {
                _totalTabCreated = 0;
                IndexAvailableTab = 0;
                List = new List<BrowserTabItem>();

                ListSuggestion.Add(new AddressItem("Apple", "https://duckduckgo.com/?q=Apple", 1));
                ListSuggestion.Add(new AddressItem("Banana", "https://duckduckgo.com/?q=Banana", 1));
                ListSuggestion.Add(new AddressItem("Orange", "https://duckduckgo.com/?q=Orange", 1));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //when useer add tab
        public void AddTab(string header)
        {
            try
            {
                _totalTabCreated++;

                if (List != null)
                {
                    string tempName = "bTab" + _totalTabCreated.ToString();
                    List.Add(new BrowserTabItem(tempName, header, ListHistory, ListSuggestion));

                    if (header != "+")
                    {
                        BrowserTabItem lastTab = List[List.Count() - 2];
                        List.RemoveAt(List.Count() - 2);
                        List.Add(lastTab);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //when user close tab
        public void RemoveTab(string name)
        {
            try
            {
                BrowserTabItem? tempBrowserTabItem = List.Where(x => x.Name == name).FirstOrDefault();
                IndexAvailableTab = 0;
                if (tempBrowserTabItem != null)
                {
                    IndexAvailableTab = List.IndexOf(tempBrowserTabItem);
                    List.Remove(tempBrowserTabItem);
                    IndexAvailableTab--;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public class BrowserTabItem : ReactiveObject
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private string _header = "";
        public string Header
        {
            get => _header;
            set => _header = value;
        }

        private Visibility _visibilityBtn;
        public Visibility VisibilityBtn
        {
            get => _visibilityBtn;
            set => _visibilityBtn = value;
        }

        private Visibility _reverseVisibilityBtn;
        public Visibility ReverseVisibilityBtn
        {
            get => _reverseVisibilityBtn;
            set => _reverseVisibilityBtn = value;
        }

        //for datacontext
        private Browser _browserDataContext = new();
        public Browser BrowserDataContext
        {
            get => _browserDataContext;
            set => _browserDataContext = value;
        }

        public BrowserTabItem(string name, string header, List<string> listHistory, List<AddressItem> listSuggestion)
        {
            try
            {
                Name = name;
                Header = header;

                if (Header != "+")
                {
                    VisibilityBtn = Visibility.Visible;
                    ReverseVisibilityBtn = Visibility.Collapsed;
                    BrowserDataContext = new Browser(listHistory, listSuggestion, Header);
                }
                else
                {
                    Name = "+";
                    VisibilityBtn = Visibility.Collapsed;
                    ReverseVisibilityBtn = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
