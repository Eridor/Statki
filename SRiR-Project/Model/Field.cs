using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SRiR_Project.Model
{
    class Field : INotifyPropertyChanged
    {
        public enum MapType { My = 0, Enemy = 1}

        public enum FieldType
        {
            Unknown = 0,
            Missed = 1,
            Hited = 2,
            Sinked = 3,
            Ship = 4,
            Builded = 5
        }
        private GameCore game = GameCore.Instance;

        public MapType Map;
        public int id;
        public int X;
        public int Y;
        public FieldType Type;

        private string _ContentText;
        public string ContentText
        {
            get
            {
                return _ContentText;
            }
            set
            {
                _ContentText = value;
                OnPropertyChanged();
            }
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public int Size { get; set; }
        public bool Enabled { get; set; }

        public string _Background;
        public string Background
        {
            get
            {
                switch (Type)
                {
                    case FieldType.Unknown:
                        return "LightBlue";
                    case FieldType.Missed:
                        return "DarkGray";
                    case FieldType.Hited:
                        return "Red";
                    case FieldType.Sinked:
                        return "DarkRed";
                    case FieldType.Builded:
                        return "Lime";
                    case FieldType.Ship:
                        return "Green";
                    default:
                        return "LightBlue";
                }
                //return _Background;
            }
            set
            {
                if(value == "LightBlue")
                    Type = FieldType.Unknown;
                else if (value == "DarkGray")
                    Type = FieldType.Missed;
                else if (value == "Red")
                    Type = FieldType.Hited;
                else if (value == "DarkRed")
                    Type = FieldType.Sinked;
                else if (value == "Lime")
                    Type = FieldType.Builded;
                else if (value == "Green")
                    Type = FieldType.Ship;
                else
                    Type = FieldType.Unknown;
                
                _Background = value;
                OnPropertyChanged();
            }
        }

        public Field()
        {
            SelectFieldCommand = new DelegateCommand(SetSelectedField);
        }

        public ICommand SelectFieldCommand { get; set; }
        private void SetSelectedField(object obj)
        {
            GameViewModel.SelectedField = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));

        }

        
    }
}
