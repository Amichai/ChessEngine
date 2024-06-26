﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessEngine;

namespace BestMoveTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            //InitializeComponent();


            new OpeningBookScraper();

            //BoardViewModel = new BoardViewModel(promotionDialog);

        }


        private BoardViewModel boardViewModel;

        public BoardViewModel BoardViewModel
        {
            get { return boardViewModel; }
            set
            {
                if (boardViewModel != value)
                {
                    boardViewModel = value;
                    OnPropertyChanged("BoardViewModel");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            var eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
