using MovieRec.Data;
using MovieRec.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MovieRec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IModel _model;
        List<Movie> _allMovies;
        ObservableCollection<Movie> _moviesAutouComplete;
        ObservableCollection<MovieRecommendation> _moviesRecommendations;

        Movie _selectedMovie = null;
        bool _autoComplete = false;
        public MainWindow()
        {
            InitializeComponent();
            _model = new MovieRecModel();
            _allMovies = _model.GetAllMoviesExist();
            autoCompleteListBx.Visibility = Visibility.Hidden;
            resultsTitleLbl.Visibility = Visibility.Hidden;
            recommendationsDatagrid.Visibility = Visibility.Hidden;

        }

        private void UpdateAutoComplete(object sender, TextChangedEventArgs e)
        {
            if(_autoComplete)
            {
                _autoComplete = false;
                return;
            }
                 
            string input = searchTxtBx.Text.ToLower();
            if (String.IsNullOrWhiteSpace(input) || input.Length<2)
            {
                autoCompleteListBx.Visibility = Visibility.Hidden;
                return;
            }
            _moviesAutouComplete = new ObservableCollection<Movie>();
            foreach (Movie movie in _allMovies)
            {
                if (movie.Title.ToLower().IndexOf(input) >= 0)
                    _moviesAutouComplete.Add(movie);
            }
            if(_moviesAutouComplete.Count == 0)
            {
                autoCompleteListBx.Visibility = Visibility.Hidden;
                _selectedMovie = null;

            }
            else if (_moviesAutouComplete.Count == 1)
            {
                autoCompleteListBx.ItemsSource = _moviesAutouComplete;
                autoCompleteListBx.Visibility = Visibility.Visible;
                _selectedMovie = _moviesAutouComplete.ElementAt(0);
            }
            else
            {
                autoCompleteListBx.ItemsSource = _moviesAutouComplete;
                autoCompleteListBx.Visibility = Visibility.Visible;
                _selectedMovie = null;

            }


        }

        private void AutoCompleteSelection(object sender, SelectionChangedEventArgs e)
        {
            if (autoCompleteListBx.SelectedItem == null)
                return;
            _autoComplete = true;
            _selectedMovie = autoCompleteListBx.SelectedItem as Movie;
            searchTxtBx.Text = (autoCompleteListBx.SelectedItem as Movie).Title;
            autoCompleteListBx.Visibility = Visibility.Hidden;

        }

        private void GetRecommendationsClick(object sender, RoutedEventArgs e)
        {
            if(_selectedMovie==null)
            {
                MessageBox.Show("Movie title was not recognized, please choose a moive title from the suggestion box.");
                return;
            }
            else
            {
                autoCompleteListBx.Visibility = Visibility.Hidden;
                _moviesRecommendations = new ObservableCollection<MovieRecommendation>(_model.GetRecommendationsForMovie(_selectedMovie));
                resultsTitleLbl.Content = String.Format("People who liked '{0}' also liked:", _selectedMovie.Title);
                recommendationsDatagrid.ItemsSource = _moviesRecommendations;
                resultsTitleLbl.Visibility = Visibility.Visible;
                recommendationsDatagrid.Visibility = Visibility.Visible;
            }

        }

        private void AutoCompleteSelection(object sender, MouseButtonEventArgs e)
        {
            _autoComplete = true;
            _selectedMovie = autoCompleteListBx.SelectedItem as Movie;
            searchTxtBx.Text = (autoCompleteListBx.SelectedItem as Movie).Title;
            autoCompleteListBx.Visibility = Visibility.Hidden;
        }
    }
}
