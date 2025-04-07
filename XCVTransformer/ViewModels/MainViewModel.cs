using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using XCVTransformer.AuxClasses;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;


namespace XCVTransformer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //-------------------------ATRIBUTOS DEL MVVM-------------------------

        ///MAIN 
        private ClipboardTaker clipboardTaker;
        private bool isTransformerOn = true;
        private string lastSelectedNavItem;

        ///TRANSLATION PAGE
        private string initText = "Hola";
        private string selectedOriginLanguage = "Español";
        private string selectedEndLanguage = "Inglés";

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

        ///-----------------------------------------------------MAIN 

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
        ///-----------------------------------------------------TRANSLATOR PAGE 
        public String InitText
        {
            get => initText;
        }

        private List<string> words = AppConstants.LanguageList;

        public List<string> Words
        {
            get => words;
            set
            {
                if (words != value)
                {
                    words = value;
                    OnPropertyChanged(nameof(Words)); 
                }
            }
        }

        public string SelectedOriginLanguage
        {
            get => selectedOriginLanguage;
            set
            {
                if (selectedOriginLanguage != value)
                {
                    string newLangCode = "es";
                    if(AppConstants.LanguageCodes.TryGetValue(value, out newLangCode))
                    {
                        selectedOriginLanguage = value;
                        Debug.WriteLine(selectedOriginLanguage);
                        this.clipboardTaker.GetTransformer().ChangeOriginCode(newLangCode);
                        this.clipboardTaker.loader.ReestartLastTransformedWord();
                        OnPropertyChanged(nameof(SelectedOriginLanguage));
                    }
                    else
                    {
                        Debug.WriteLine("Error buscando código de lenguaje origen");
                    }
                }
            }
        }

        public string SelectedEndLanguage
        {
            get => selectedEndLanguage;
            set
            {
                if (selectedEndLanguage != value)
                {
                    string newLangCode = "en";
                    if (AppConstants.LanguageCodes.TryGetValue(value, out newLangCode))
                    {
                        selectedEndLanguage = value;
                        Debug.WriteLine(selectedEndLanguage);
                        this.clipboardTaker.GetTransformer().ChangeEndCode(newLangCode);
                        this.clipboardTaker.loader.ReestartLastTransformedWord();
                        OnPropertyChanged(nameof(selectedEndLanguage));
                    }
                    else
                    {
                        Debug.WriteLine("Error buscando código de lenguaje destino");
                    }
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
                "CodificationPage" => typeof(CodificationPage),
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
