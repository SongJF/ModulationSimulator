﻿using System;
using System.ComponentModel;

namespace ModulationSimulator.Domain
{
    class MenuList: INotifyPropertyChanged
    {
        private string _name;
        private object _content;

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        public object Content
        {
            get { return _content; }
            set { this.MutateVerbose(ref _content, value, RaisePropertyChanged()); }
        }

        public MenuList(string name, object content)
        {
            _name = name;
            Content = content;
        }

        public string Name
        {
            get { return _name; }
            set { this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }
        }
    }

}
