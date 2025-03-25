using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using WinRT.Interop;
using XCVTransformer.AuxClasses;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;
using XCVTransformer.ViewModels;

namespace XCVTransformer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ClipboardTaker clipboardTaker;
        private bool isTransformerOn;


        public MainViewModel()
        {
            this.clipboardTaker = new ClipboardTaker();
        }


        //-------------------------PROPERTIES-------------------------

        public ClipboardTaker ClipboardTaker
        {
            get => clipboardTaker;
        }

        public bool IsTransformerOn
        {
            get => isTransformerOn;
            set
            {
                if(isTransformerOn != value)
                {
                    isTransformerOn = value;
                    OnPropertyChanged(nameof(IsTransformerOn));

                    clipboardTaker.ChangeTransformerState(value);
                }   
            }
        }

        public string ClipboardText
        { 
            get; 
            set; 
        } = "Esperando texto...";

        //-------------------------MAS-------------------------

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
