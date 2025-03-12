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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;

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

        //list close object tab
        private List<BrowserTabItem> _listCloseTab = new();
        public List<BrowserTabItem> ListCloseTab
        {
            get => _listCloseTab;
            set => _listCloseTab = value;
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
                        BrowserTabItem lastTab = List[List.Count - 2];
                        List.RemoveAt(List.Count - 2);
                        List.Add(lastTab);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //when user close tab using button
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
                    ListCloseTab.Add(tempBrowserTabItem);
                    IndexAvailableTab--;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //when user close tab using shortcut
        public void RemoveTab(int index)
        {
            try
            {
                if (index <= List.Count - 1)
                {
                    BrowserTabItem tempBrowserTabItem = List[index];
                    IndexAvailableTab = index;
                    if (tempBrowserTabItem != null)
                    {
                        List.Remove(tempBrowserTabItem);
                        ListCloseTab.Add(tempBrowserTabItem);
                        IndexAvailableTab--;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //for reopen tab
        public void ReopenTab()
        {
            try
            {
                _totalTabCreated++;

                if (List != null)
                {
                    BrowserTabItem reopenTab = ListCloseTab[ListCloseTab.Count - 1];
                    ListCloseTab.RemoveAt(ListCloseTab.Count - 1);

                    BrowserTabItem reopenTab2 = new BrowserTabItem(reopenTab);
                    List.Add(reopenTab2);

                    BrowserTabItem lastTab = List[List.Count - 2];
                    List.RemoveAt(List.Count - 2);
                    List.Add(lastTab);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //for reorder tab
        public void ReorderTab(int removeIndex, int insertIndex)
        {
            try
            {
                //if it try put in lastest tab, ignore it
                if (insertIndex == List.Count - 1)
                    return;

                //if user try move the latest tab, ignore it
                if (removeIndex == List.Count - 1)
                    return;

                BrowserTabItem tabItem = List[removeIndex];
                List.RemoveAt(removeIndex);
                List.Insert(insertIndex, tabItem);
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

        public BrowserTabItem(BrowserTabItem reopenTab)
        {
            try
            {
                Name = reopenTab.Name;
                Header = reopenTab.Header;
                VisibilityBtn = reopenTab.VisibilityBtn;
                ReverseVisibilityBtn = reopenTab.ReverseVisibilityBtn;
                BrowserDataContext = reopenTab.BrowserDataContext;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
