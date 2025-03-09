using CefSharp;
using CefSharp.DevTools.CacheStorage;
using CefSharp.DevTools.CSS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Model;

namespace WpfApp1.View_Model
{
    public class Browser : ReactiveObject
    {
        //to know suggestion get from button tab
        private bool isSuggestion;

        //to know user input
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                try
                {
                    this.RaiseAndSetIfChanged(ref _searchText, value);
                    Items.CurrentText = _searchText;
                    AddressListName = new ObservableCollection<AddressItem>(Items.FilterName);
                    ShowListSuggestion = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        //to show suggestion user select from tab
        private string _searchTextWithSuggestion;
        public string SearchTextWithSuggestion
        {
            get => _searchTextWithSuggestion;
            set
            {
                try
                {
                    this.RaiseAndSetIfChanged(ref _searchTextWithSuggestion, value);
                    Items.CurrentTextWithSuggestion = _searchTextWithSuggestion;

                    //update SearchText if it is user typing
                    if (!isSuggestion) SearchText = _searchTextWithSuggestion;
                    else isSuggestion = false;

                    AddressListName = new ObservableCollection<AddressItem>(Items.FilterName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        //to get suggestion that user select
        private object _selectedSuggestion;
        public object SelectedSuggestion
        {
            get => _selectedSuggestion;
            set => this.RaiseAndSetIfChanged(ref _selectedSuggestion, value);
        }

        //to get history that user select
        private object _selectedHistory;
        public object SelectedHistory
        {
            get => _selectedHistory;
            set => this.RaiseAndSetIfChanged(ref _selectedHistory, value);
        }

        //to set browser address
        private string _webViewAddress;
        public string WebViewAddress
        {
            get => _webViewAddress;
            set
            {
                try
                {
                    this.RaiseAndSetIfChanged(ref _webViewAddress, value);
                    SearchTextWithSuggestion = _webViewAddress;
                    Items.AddAddress(SearchText);
                    Items.AddHistory();
                    HistoryListName = new ObservableCollection<string>(Items.ListHistoryMain.AsEnumerable().Reverse());

                    ShowButtonBackActive = (Items.IndexHistory > 1);
                    ShowButtonFowardActive = (Items.IndexHistory < Items.ListHistory.Count);

                    CurrentNameAddress = Items.CurrentNameAddress;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        //to keep track current name address for header
        private string _currentNameAddress;
        public string CurrentNameAddress
        {
            get => _currentNameAddress;
            set
            {
                try
                {
                    if (Items.ListHistory.Count > 0)
                        this.RaiseAndSetIfChanged(ref _currentNameAddress, value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        //to keep track list suggestion show or hide
        private bool _showListSuggestion;
        public bool ShowListSuggestion
        {
            get => _showListSuggestion;
            set => this.RaiseAndSetIfChanged(ref _showListSuggestion, value);
        }

        private bool _showButtonBackActive;
        public bool ShowButtonBackActive
        {
            get => _showButtonBackActive;
            set => this.RaiseAndSetIfChanged(ref _showButtonBackActive, value);
        }

        private bool _showButtonFowardActive;
        public bool ShowButtonFowardActive
        {
            get => _showButtonFowardActive;
            set => this.RaiseAndSetIfChanged(ref _showButtonFowardActive, value);
        }

        //main object
        private Address _items = new Address();
        public Address Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        //to list suggestion item
        private ObservableCollection<AddressItem> _addressListName = new();
        public ObservableCollection<AddressItem> AddressListName
        {
            get => _addressListName;
            set => this.RaiseAndSetIfChanged(ref _addressListName, value);
        }

        //to list history item
        private ObservableCollection<string> _historyListName = new();
        public ObservableCollection<string> HistoryListName
        {
            get => _historyListName;
            set => this.RaiseAndSetIfChanged(ref _historyListName, value);
        }

        //when user press 'enter'
        public ReactiveCommand<Unit, Unit> SubmitAddress
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    Items.AddAddress(SearchTextWithSuggestion);
                    WebViewAddress = Items.CurrentActiveAddress;
                    ShowListSuggestion = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user press 'tab' or 'down'
        public ReactiveCommand<Unit, Unit> TakeSuggestionAddress
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    if (ShowListSuggestion)
                    {
                        AddressItem result = Items.TakeSuggestion();
                        if (result != null)
                        {
                            isSuggestion = true;
                            SearchTextWithSuggestion = result.URL;
                            SelectedSuggestion = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                //else
                //{
                //    // Simulate the default Tab behavior
                //    var currentFocus = Keyboard.FocusedElement as UIElement;
                //    if (currentFocus != null)
                //    {
                //        currentFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                //    }
                //}
            });
        }

        //when user click 1 of suggestion
        public ReactiveCommand<Unit, Unit> ClickSuggestionAddress
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    AddressItem tempSuggest = SelectedSuggestion as AddressItem;
                    if (tempSuggest != null)
                    {
                        SearchTextWithSuggestion = tempSuggest.URL;
                        SubmitAddress.Execute().Subscribe();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user click 1 of history
        public ReactiveCommand<Unit, Unit> ClickHistoryAddress
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    string tempHistory = SelectedHistory as string;
                    if (tempHistory != null)
                    {
                        SearchTextWithSuggestion = tempHistory;
                        SubmitAddress.Execute().Subscribe();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user click back
        public ReactiveCommand<Unit, Unit> ButtonBack
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    SearchTextWithSuggestion = Items.TakeHistory(true);
                    SubmitAddress.Execute().Subscribe();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user click foward
        public ReactiveCommand<Unit, Unit> ButtonFoward
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    SearchTextWithSuggestion = Items.TakeHistory(false);
                    SubmitAddress.Execute().Subscribe();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user click refresh
        public ReactiveCommand<Unit, Unit> ButtonRefresh
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    SubmitAddress.Execute().Subscribe();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        public Browser(List<string> listHistory, List<AddressItem> listSuggestion, string header)
        {
            try
            {
                Items.ListHistoryMain = listHistory;
                Items.List = listSuggestion;
                Initialize();
                _currentNameAddress = header;
                CurrentNameAddress = header;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public Browser()
        {
            try
            {
                Items.ListHistoryMain = new List<string>();
                Items.List = new List<AddressItem>();
                Initialize();
                CurrentNameAddress = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Initialize()
        {
            try
            {
                WebViewAddress = Items.CurrentActiveAddress;
                ShowListSuggestion = false;
                isSuggestion = false;
                Items.CurrentText = SearchText;
                AddressListName = new ObservableCollection<AddressItem>(Items.FilterName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
