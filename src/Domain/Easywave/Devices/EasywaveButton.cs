﻿using System;

namespace Domain
{
    public class EasywaveButton : IEasywaveButton
    {
        private string _name;

        public EasywaveButton(uint address) : this(address, $"Unkown {address}")
        {
        }

        public EasywaveButton(uint address, string name)
        {
            Address = address;
            _name = name;
        }

        public uint Address { get; }
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                NameChanged?.Invoke(this, _name);
            }
        }

        public event EventHandler<string> NameChanged;

        void IDisposable.Dispose()
        {
            //Nothing to dispose
        }

        public void Start()
        {
            //Nothing to do
        }

        public void Stop()
        {
            //Nothing to do
        }
    }
}