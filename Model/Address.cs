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
using static System.Net.WebRequestMethods;

namespace WpfApp1.Model
{
    public class Address : ReactiveObject
    {
        private static string _defaultSearch = "https://duckduckgo.com/?q=";
        private static string _defaultHome = "https://duckduckgo.com/";

        //active address in webview
        private string _currentActiveAddress = _defaultHome;
        public string CurrentActiveAddress
        {
            get => _currentActiveAddress;
            set => _currentActiveAddress = value;
        }

        //name header tab
        private string _currentNameAddress = "";
        public string CurrentNameAddress
        {
            get => _currentNameAddress;
            set => _currentNameAddress = value;
        }

        //to filter suggestion list
        private string _currentText = "";
        public string CurrentText
        {
            get => _currentText;
            set
            {
                _currentText = value;
                IndexSuggestion = 0;
            }
        }

        //to show current suggestion or user input
        private string _currentTextWithSuggestion = "";
        public string CurrentTextWithSuggestion
        {
            get => _currentTextWithSuggestion;
            set => _currentTextWithSuggestion = value;
        }

        //to track current suggestion from list
        private int _indexSuggestion;
        public int IndexSuggestion
        {
            get => _indexSuggestion;
            set
            {
                _indexSuggestion = value;

                if (_filterName != null)
                    if (_indexSuggestion >= _filterName.Count)
                        _indexSuggestion = 0;
            }
        }

        //to track current history for back and foward
        private int _indexHistory;
        public int IndexHistory
        {
            get => _indexHistory;
            set => _indexHistory = value;
        }

        //list object address contain all info
        private List<AddressItem> _list = new();
        public List<AddressItem> List
        {
            get => _list;
            set => _list = value;
        }

        //list address name and url for list suggestion
        private List<AddressItem> _filterName = new();
        public List<AddressItem> FilterName
        {
            get
            {
                try
                {
                    var tempList = List;
                    if (!string.IsNullOrEmpty(CurrentText))
                    {
                        tempList = List.Where(x => x.Name.ToLower().Contains(CurrentText.ToLower()) ||
                            x.URL.ToLower().Contains(CurrentText.ToLower())).ToList();
                    }

                    _filterName = tempList.OrderByDescending(x => x.Visit)
                        //.Select(x => x.Name + " - " + x.URL)
                        .Distinct()
                        .Take(7)
                        .ToList();
                    return _filterName;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        //list url address for back and foward history
        private List<string> _listHistory = new();
        public List<string> ListHistory
        {
            get => _listHistory;
            set => _listHistory = value;
        }

        //list url address for all history 
        private List<string> _listHistoryMain = new();
        public List<string> ListHistoryMain
        {
            get => _listHistoryMain;
            set => this.RaiseAndSetIfChanged(ref _listHistoryMain, value);
        }

        public Address()
        {
            IndexSuggestion = 0;
            IndexHistory = 0;
        }

        //to add address into suggestion and history
        public void AddAddress(string address)
        {
            try
            {
                if (string.IsNullOrEmpty(address))
                    return;

                string tempName;
                string tempAddress;

                if (IsValidURL(address))
                {
                    tempAddress = address;
                    tempName = GetDomainName(address);
                }
                else
                {
                    tempName = address;
                    tempAddress = _defaultSearch + address.Replace(' ', '+');
                }

                if (tempAddress.IndexOf(_defaultSearch) >= 0)
                {
                    tempAddress = tempAddress.Split('&')[0];
                }

                var existingAddress = List.FirstOrDefault(x => x.URL == tempAddress);
                if (existingAddress != null)
                {
                    existingAddress.Visit += 1;
                }
                else
                {
                    List.Add(new AddressItem(tempName, tempAddress, 1));
                }

                CurrentActiveAddress = SetCompleteURL(tempAddress);
                CurrentNameAddress = tempName.Replace('+',' ');
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //return first or next suggestion
        public AddressItem TakeSuggestion()
        {
            try
            {
                AddressItem result = null;
                if (_filterName.Count > 0 && _filterName.Count > IndexSuggestion)
                {
                    result = _filterName[IndexSuggestion];
                    IndexSuggestion++;
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        //add history
        public void AddHistory()
        {
            try
            {
                CurrentActiveAddress = CurrentActiveAddress.Replace("t=h_&", "").Replace("&ia=web", "");

                //ignore homepage
                if (ListHistory.Count == 0 && CurrentActiveAddress == _defaultHome) return;

                //if it try insert same history with current index, ignore it, it mean from back or foward or refresh
                if (ListHistory.Count == 0 || ListHistory[IndexHistory - 1] != CurrentActiveAddress)
                {
                    if (ListHistory.Count > IndexHistory)
                        ListHistory = ListHistory.Take(IndexHistory).ToList();

                    ListHistory.Add(CurrentActiveAddress);
                    ListHistoryMain.Add(CurrentActiveAddress);
                    IndexHistory++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //take history
        public string TakeHistory(bool isBack)
        {
            try
            {
                string result = "";

                if (isBack && IndexHistory > 1)
                {
                    IndexHistory--;
                    result = ListHistory[IndexHistory - 1];
                }
                else if (!isBack && ListHistory.Count > IndexHistory)
                {
                    IndexHistory++;
                    result = ListHistory[IndexHistory - 1];
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        //check string is acceptable URL
        public bool IsValidURL(string URL)
        {
            try
            {
                string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
                Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return Rgx.IsMatch(URL);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        //get domain name
        public string GetDomainName(string URL)
        {
            try
            {
                string result = "";
                var tempResult = URL.Replace("http://", "").Replace("https://", "").Replace("www.", "").Split('.');
                URL = URL.Replace("t=h_&", "");

                if (URL.IndexOf(_defaultSearch) >= 0)
                {
                    //for search on web get the word instead
                    result = URL.Replace(_defaultSearch, "").Split('&')[0];
                }
                else if (!tempResult[0].All(char.IsDigit))
                {
                    result = tempResult[0];
                }
                else
                {
                    //for ip
                    result = string.Join('.', tempResult).Split('/')[0];
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        //make sure it complete url
        public string SetCompleteURL(string URL)
        {
            try
            {
                string result = URL;

                if (result.IndexOf("http://") < 0 && result.IndexOf("https://") < 0)
                {
                    result = "https://" + result;
                }

                //if (result.IndexOf("http://www.") < 0 && result.IndexOf("https://www.") < 0)
                //{
                //    result = result.Replace("http://", "http://www.");
                //    result = result.Replace("https://", "https://www.");
                //}

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
    }

    public class AddressItem
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private string _url = "";
        public string URL
        {
            get => _url;
            set => _url = value;
        }

        private int _visit;
        public int Visit
        {
            get => _visit;
            set => _visit = value;
        }

        public AddressItem(string name, string url, int visit)
        {
            Name = name;
            URL = url;
            Visit = visit;
        }
    }
}
