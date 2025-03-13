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
using System.Windows.Threading;
using WpfApp1.Model;
using WpfApp1.View;

namespace WpfApp1.View_Model
{
    public class Main : ReactiveObject
    {
        //to get selected tab
        private int _selectedTab;
        public int SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }

        //to change size tab control so it trigger cache cleaning when tab got close
        private bool _isTabRefresh = false;
        public bool IsTabRefresh
        {
            get => _isTabRefresh;
            set => this.RaiseAndSetIfChanged(ref _isTabRefresh, value);
        }

        //main object
        private BrowserTab _items = new BrowserTab();
        public BrowserTab Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        //main binding object
        private ObservableCollection<BrowserTabItem> _listTab = new();
        public ObservableCollection<BrowserTabItem> ListTab
        {
            get
            {
                return _listTab;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _listTab, value);
                Items.List = new List<BrowserTabItem>(_listTab);
            }
        }

        //main binding history
        private ObservableCollection<string> _listWindowHistory = new();
        public ObservableCollection<string> ListWindowHistory
        {
            get => _listWindowHistory;
            set => this.RaiseAndSetIfChanged(ref _listWindowHistory, value);
        }

        //when user click close
        public ReactiveCommand<string, Unit> ButtonClose
        {
            get => ReactiveCommand.Create<string>((name) => {
                try
                {
                    Items.RemoveTab(name);
                    ListTab = new ObservableCollection<BrowserTabItem>(Items.List);

                    if (SelectedTab < 0) SelectedTab = Items.IndexAvailableTab;
                    else IsTabRefresh = true; //for clean cache because selection not change

                    if (ListTab.Count <= 1) Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user press shortcut close
        public ReactiveCommand<Unit, Unit> ShortcutClose
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    Items.RemoveTab(SelectedTab);
                    ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
                    if (SelectedTab < 0) SelectedTab = Items.IndexAvailableTab;

                    if (ListTab.Count <= 1) Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user change tab
        public ReactiveCommand<Unit, Unit> ChangeTab
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    if (SelectedTab != 0 && SelectedTab == Items.List.Count - 1)
                    {
                        AddTab.Execute().Subscribe();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //for add tab
        public ReactiveCommand<Unit, Unit> AddTab
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    Items.AddTab("New Tab");
                    ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
                    SelectedTab = ListTab.Count - 2;

                    if (SelectedTab <= 0) SelectedTab = 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //when user press shortcut reopen tab
        public ReactiveCommand<Unit, Unit> ShortcutReopenTab
        {
            get => ReactiveCommand.Create(() => {
                try
                {
                    Items.ReopenTab();
                    ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
                    if (SelectedTab < 0) SelectedTab = Items.IndexAvailableTab;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //for helper drag and drop call, because it cannot update list in there
        public void DragDropHelperUpdateList(List<BrowserTabItem> list)
        {
            Items.List = list.ToList();
            ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
        }

        //for helper drag and drop call, because it cannot update list in there
        public void DragDropHelperUpdateList(int removeIndex, int insertIndex)
        {
            Items.ReorderTab(removeIndex, insertIndex);
            ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
        }

        public Main()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                Items.AddTab("+");
                Items.AddTab("New Window");
                ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
