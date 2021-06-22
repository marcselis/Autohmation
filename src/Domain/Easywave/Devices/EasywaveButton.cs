﻿using System;

namespace Domain
{
    public class EasywaveButton : IEasywaveButton
    {
        private string _name;

        public EasywaveButton(uint address) : this(address, $"Unknown {address}")
        {
        }

        public EasywaveButton(uint address, string name)
        {
            Address = address;
            _name = name;
        }

        public uint Address { get; set; }
        public string Name
        {
            get { return _name; }
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

        public event EventHandler<string>? NameChanged;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
