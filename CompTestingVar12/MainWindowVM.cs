using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CompTestingVar12
{
    internal class MainWindowVM : NotifyPropertyChanged
    {
        #region Свойства
        private int _matrixRank;
        public int MatrixRank
        {
            get => _matrixRank;
            set
            {
                if(value > 15) _matrixRank = 15;
                else _matrixRank = value;
                OnPropertyChanged();
            }
        }

        private int?[,] _matrix;
        public int?[,] Matrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                OnPropertyChanged();
            }
        }

        private int _lowLimit;
        public int LowLimit
        {
            get => _lowLimit;
            set
            {
                if (value > HighLimit) return;
                else _lowLimit = value;
            }
        }

        private int _highLimit;
        public int HighLimit
        {
            get => _highLimit;
            set
            {
                if (value < LowLimit) return;
                else _highLimit = value;
            }
        }

        private string _loadedFile;
        public string LoadedFile
        {
            get => _loadedFile;
            set
            {
                _loadedFile = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ObservableCollection<int?>> _matrixValueCollection;
        public ObservableCollection<ObservableCollection<int?>> MatrixValueCollection
        {
            get => _matrixValueCollection;
            set
            {
                _matrixValueCollection = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Команды

        private ICommand _resizeMatrixCommand;
        public ICommand ResizeMatrixCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    MatrixValueCollection = new ObservableCollection<ObservableCollection<int?>>();
                    for(int i = 0; i < MatrixRank; i++)
                    {
                        MatrixValueCollection.Add(new ObservableCollection<int?>());
                        for(int j = 0; j < MatrixRank; j++)
                        {
                            MatrixValueCollection[i].Add(0);
                        }
                    }
                    Matrix = new int?[MatrixRank, MatrixRank];
                }, o => true);
            }
            set { }
        }

        private ICommand _fillMatrixRandomCommand;
        public ICommand FillMatrixRandomCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    MatrixValueCollection = new ObservableCollection<ObservableCollection<int?>>();
                    Random rnd = new Random();
                    for (int i = 0; i < MatrixRank; i++)
                    {
                        MatrixValueCollection.Add(new ObservableCollection<int?>());
                        for (int j = 0; j < MatrixRank; j++)
                        {
                            Matrix[i, j] = rnd.Next(LowLimit, HighLimit);
                            MatrixValueCollection[i].Add(Matrix[i, j]);
                        }
                    }
                }, o => true);
            }
            set { }
        }

        private ICommand _openMatrixFileCommand;
        public ICommand OpenMatrixFileCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Текстовый файл (*.txt)|*.txt";
                    ofd.FilterIndex = 1;
                    ofd.RestoreDirectory = true;
                    if (ofd.ShowDialog() == true) LoadedFile = File.ReadAllText(ofd.FileName);
                    Matrix = ParseToArray(LoadedFile);
                    if (Matrix == null) return;
                    MatrixValueCollection = new ObservableCollection<ObservableCollection<int?>>();
                    for (int i = 0; i < MatrixRank; i++)
                    {
                        MatrixValueCollection.Add(new ObservableCollection<int?>());
                        for (int j = 0; j < MatrixRank; j++)
                        {
                            MatrixValueCollection[i].Add(Matrix[i, j]);
                        }
                    }
                }, o => true);
            }
        }

        private ICommand _closeApplicationCommand;
        public ICommand CloseApplicationCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Application.Current.Shutdown();
                }, o => true);
            }
        }

        private ICommand _squareLocalMinsCommand;
        public ICommand SquareLocalMinsCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    int?[,] array = new int?[MatrixRank, MatrixRank];
                    for (int i = 0; i < MatrixRank; i++)
                    {
                        for (int j = 0; j < MatrixRank; j++)
                        {
                            bool flag = true;
                            if (i + 1 < MatrixRank && (Matrix[i, j] > Matrix[i + 1, j])) flag = false;
                            if (i - 1 > MatrixRank && (Matrix[i, j] > Matrix[i - 1, j])) flag = false;
                            if (j + 1 < MatrixRank && (Matrix[i, j] > Matrix[i, j + 1])) flag = false;
                            if (j - 1 > MatrixRank && (Matrix[i, j] > Matrix[i, j - 1])) flag = false;
                            if(flag) array[i, j] = Matrix[i, j] * Matrix[i, j];
                            else array[i, j] = Matrix[i, j];
                        }
                    }
                    MatrixValueCollection = new ObservableCollection<ObservableCollection<int?>>();
                    for (int i = 0; i < MatrixRank; i++)
                    {
                        MatrixValueCollection.Add(new ObservableCollection<int?>());
                        for (int j = 0; j < MatrixRank; j++)
                        {
                            Matrix[i, j] = array[i, j];
                            MatrixValueCollection[i].Add(Matrix[i, j]);
                        }
                    }
                }, o => true);
            }
        }

        #endregion

        private int?[,] ParseToArray(string parsingString)
        {
            string[] separatingString = { "\r\n" };
            var strings = parsingString.Split(separatingString, StringSplitOptions.RemoveEmptyEntries);
            int?[,] array = new int?[strings.Count(), strings.Count()];
            try
            {
                for (int i = 0; i < strings.Count(); i++)
                {
                    var k = strings[i].Split(' ');
                    if (k.Length != strings.Count()) throw new Exception("Преобразование невозможно: матрица не является квадратной");
                    for (int j = 0; j < strings.Count(); j++)
                    {
                        array[i, j] = Convert.ToInt32(k[j]);
                    }
                }
                MatrixRank = strings.Count();
            }
            catch(Exception e) {
                MessageBox.Show("Введены некорректные данные, проверьте правильность вводимых данных и повторите ещё раз!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return array;
        }

        public MainWindowVM()
        {
            LowLimit = -1;
            HighLimit = 1;
        }
    }
}
