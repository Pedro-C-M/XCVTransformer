using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using XCVTransformer.AuxClasses;
using XCVTransformer.Helpers;
using XCVTransformer.Pages;
using XCVTransformer.Transformers;
using XCVTransformer.Transformers.Codificators;


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
        ///CODIFICATIONS PAGE
        private string selectedCodification = "";



        ///OTROS PAGE
        private bool isDetectorOn = false;
        private string detectedLanguageText = "";


        //-------------------------CLASE Y METODOS-------------------------

        private readonly Frame _contentFrame;

        public MainViewModel(Frame contentFrame)
        {
            this.clipboardTaker = new ClipboardTaker();
            this.clipboardTaker.loader.OnDetectionCompleted += (detectedLanguage) =>
            {
                DetectedLanguageText = detectedLanguage;
            };
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
                    CheckNoTraductor();
                    string newLangCode = "es";
                    if(AppConstants.LanguageCodes.TryGetValue(value, out newLangCode))
                    {
                        selectedOriginLanguage = value;
                        //Debug.WriteLine(selectedOriginLanguage);

                        //Se comprueba que sea traductor y se cambia
                        if (this.clipboardTaker.GetTransformer() is ITraductor traductor)
                        {
                            traductor.ChangeOriginCode(newLangCode);
                        }
                        this.clipboardTaker.loader.ReestartLastTransformedWord();
                        OnPropertyChanged(nameof(SelectedOriginLanguage));
                        desactivarCodifVisuals();
                        desactivarLanguageDetectorSwitch();
                        desactivarCodifVisuals();
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
                    CheckNoTraductor();
                    string newLangCode = "en";
                    if (AppConstants.LanguageCodes.TryGetValue(value, out newLangCode))
                    {
                        selectedEndLanguage = value;
                        //Debug.WriteLine(selectedEndLanguage);

                        //Se comprueba que sea traductor y se cambia
                        if (this.clipboardTaker.GetTransformer() is ITraductor traductor)
                        {
                            traductor.ChangeEndCode(newLangCode);
                        }

                        this.clipboardTaker.loader.ReestartLastTransformedWord();
                        OnPropertyChanged(nameof(selectedEndLanguage));
                        desactivarCodifVisuals();
                        desactivarLanguageDetectorSwitch();
                        desactivarCodifVisuals();
                    }
                    else
                    {
                        Debug.WriteLine("Error buscando código de lenguaje destino");
                    }
                }
            }
        }

        /**
         * Si venimos de no tener traductor es necesario poner uno antes de configurarlo
         * 
         */
        private void CheckNoTraductor()
        {
            if (this.clipboardTaker.GetTransformer() is not ITraductor)
            {
                this.clipboardTaker.loader._transformer = new Traductor();
            }
        }
        //--------------------------CODIFICATION PAGE-----------------------
        private List<string> codifications = AppConstants.CodificationList;
        public List<string> Codifications
        {
            get => codifications;
            set
            {
                if (codifications != value)
                {
                    codifications = value;
                    OnPropertyChanged(nameof(Codifications));
                }
            }
        }
        public string SelectedCodification
        {
            get => selectedCodification;
            set
            {
                if (selectedCodification != value)
                {
                    selectedCodification = value;
                    OnPropertyChanged(nameof(SelectedCodification));
                    try
                    {
                        this.clipboardTaker.loader._transformer = CodificatorFactory.Create(value);
                        Debug.WriteLine("Codificador creado para: " + value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error creando codificador: " + ex.Message);
                    }
                    desactivarLanguageDetectorSwitch();
                    desactivarTraductorVisuals();
                }
            }
        }

        //-------------------------OTROS PAGE-------------------------
        public bool IsDetectorOn
        {
            get => isDetectorOn;
            set
            {
                if (isDetectorOn != value)
                {
                    isDetectorOn = value;
                    OnPropertyChanged(nameof(isDetectorOn));
                    this.clipboardTaker.loader.ChangeToDetectorMode(isDetectorOn);
                    if (value)
                    {               
                        this.desactivarTraductorVisuals();
                        this.desactivarCodifVisuals();
                    }
                    else
                    {
                        this.DetectedLanguageText = "";
                    }
                }
            }
        }

        

        public string DetectedLanguageText
        {
            get => detectedLanguageText;
            set
            {
                if (detectedLanguageText != value)
                {
                    detectedLanguageText = value;
                    OnPropertyChanged(nameof(DetectedLanguageText));
                }
            }
        }
        //-------------------------CAMBIAR VISUALES-------------------------
        private void desactivarTraductorVisuals()
        {
            this.selectedOriginLanguage = "";
            this.selectedEndLanguage = "";
        }
        
        private void desactivarCodifVisuals()
        {
            this.selectedCodification = "";
        }

        private void desactivarLanguageDetectorSwitch()
        {
            if (this.isDetectorOn){
                this.isDetectorOn = false;
                this.clipboardTaker.loader.ChangeToDetectorMode(false);
                this.detectedLanguageText = "";
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
                "OtrosPage" => typeof(OtrosPage),
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
