using CefSharp.DevTools.CSS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            get => _listTab;
            set => this.RaiseAndSetIfChanged(ref _listTab, value);
        }

        //main binding history
        private ObservableCollection<string> _listWindowHistory = new();
        public ObservableCollection<string> ListWindowHistory
        {
            get => _listWindowHistory;
            set
            {
                if(this.RaiseAndSetIfChanged(ref _listWindowHistory, value) != null)
                {
                    return;
                }
            }
        }

        //when user click close
        public ReactiveCommand<string, Unit> ButtonClose
        {
            get => ReactiveCommand.Create<string>((name) => {
                Items.RemoveTab(name);
                ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
                SelectedTab = Items.IndexAvailableTab;

                if (ListTab.Count() <= 1) Application.Current.Shutdown();
            });
        }

        //when user change tab
        public ReactiveCommand<Unit, Unit> ChangeTab
        {
            get => ReactiveCommand.Create(() => {
                if (SelectedTab != 0 && SelectedTab == Items.List.Count() - 1)
                {
                    AddTab.Execute().Subscribe();
                }
            });
        }

        //for add tab
        public ReactiveCommand<Unit, Unit> AddTab
        {
            get => ReactiveCommand.Create(() => {
                Items.AddTab("New Tab");
                ListTab = new ObservableCollection<BrowserTabItem>(Items.List);
                SelectedTab = ListTab.Count() - 2;

                if (SelectedTab <= 0) SelectedTab = 0;
            });
        }

        public Main()
        {
            Initialize();
        }

        private void Initialize()
        {
            Items.AddTab("+");
            Items.AddTab("New Window");
            ListTab = new ObservableCollection<BrowserTabItem>(Items.List);

        }
    }
}
