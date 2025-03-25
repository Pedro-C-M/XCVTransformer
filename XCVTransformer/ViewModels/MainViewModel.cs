using System;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;


namespace XCVTransformer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //-------------------------ATRIBUTOS DEL MVVM-------------------------

        private ClipboardTaker clipboardTaker;
        private bool isTransformerOn;
        private string lastSelectedNavItem;


        //-------------------------CLASE Y METODOS-------------------------

        private readonly Frame _contentFrame;

        public MainViewModel(Frame contentFrame)
        {
            this.clipboardTaker = new ClipboardTaker();
            this._contentFrame = contentFrame;

            NavigateCommand = new RelayCommand<string>(Navigate);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //-------------------------PROPERTIES DEL MVVM-------------------------

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
     
        public string LastSelectedNavItem
        {
            get => lastSelectedNavItem;
            set
            {
                if(lastSelectedNavItem != value)
                {
                    lastSelectedNavItem = value;
                    OnPropertyChanged(nameof(LastSelectedNavItem));
                }
            }
        }

        //-------------------------NAVEGACIÓN-------------------------

        public ICommand NavigateCommand { get; }

        public void Navigate(string tag)
        {
            // Evita re-seleccionar el mismo elemento
            if (tag == null || tag == LastSelectedNavItem) return; 

            Type targetPage = tag switch
            {
                "TranslatorPage" => typeof(TranslatorPage),
                "OptionsPage" => typeof(OptionsPage),
                _ => null
            };

            if (targetPage != null)
            {
                _contentFrame.Navigate(targetPage);
                LastSelectedNavItem = tag;
            }
        }

    }
}
