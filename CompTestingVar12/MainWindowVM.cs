using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    for(int i = 0; i < MatrixRank; i++)
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

        #endregion

        public MainWindowVM()
        {
            MatrixRank = 2;
            Matrix = new int?[2, 2] { { 1, 3 }, { 2, 4 } };
            LowLimit = -1;
            HighLimit = 1;
        }
    }
}
