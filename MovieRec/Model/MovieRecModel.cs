using MovieRec.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace MovieRec
{
    public class MovieRecModel : IModel , INotifyPropertyChanged
    {
        int totalRecords = 27279 + 1048576;
        int currentRecord=0;
        Dictionary<int, Dictionary<int, double>> _ranksOfMovivesByUsers;
        Dictionary<int, Movie> _allMovies;
        public event PropertyChangedEventHandler PropertyChanged;
        string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }
        public MovieRecModel(PropertyChangedEventHandler PropertyChanged)
        {
            this.PropertyChanged = PropertyChanged;
        }

        internal void LoadDataSet()
        {
            readMoviesFile();
            readRatingsFiles();
        }

        private void readRatingsFiles()
        {
            using (var rd = new StreamReader("ratings.csv"))
            {
                   _ranksOfMovivesByUsers = new Dictionary<int, Dictionary<int, double>>();
                var splits = rd.ReadLine().Split(',');
                while (!rd.EndOfStream)
                {
                    splits = rd.ReadLine().Split(',');
                    int userId = Convert.ToInt32(splits[0]);
                    int movieId = Convert.ToInt32(splits[1]);
                    double rank = Convert.ToDouble(splits[2]);
                    AddRating(_ranksOfMovivesByUsers, movieId, userId, rank);
                    Status = String.Format("Loading dataset ({0:0.0}%)", 5 * (double)currentRecord/totalRecords);
                    currentRecord++;
                }
            }
        }


        private static void AddRating(Dictionary<int, Dictionary<int, double>> ranks , int key, int subKey, double rating)
        {
            if (!ranks.ContainsKey(key))
                ranks[key] = new Dictionary<int, double>();
            ranks[key][subKey] = rating;
        }

        private void readMoviesFile()
        {
            _allMovies = new Dictionary<int, Movie>();
            int movieId;
            string movieName;
            using (var rd = new StreamReader("movies.csv"))
            {
                var splits = rd.ReadLine().Split(',');
                while (!rd.EndOfStream)
                {
                    splits = rd.ReadLine().Split(',');
                    movieId = Convert.ToInt32(splits[0]);
                    movieName = splits[1];
                    for (int i = 2; i < splits.Length - 1; i++)
                        movieName += "," + splits[i];
                    if (movieName[0] == '\"' && movieName[movieName.Length - 1] == '\"')
                        movieName = movieName.Substring(1, movieName.Length - 2);
                     _allMovies[movieId] =  new Movie(movieId, movieName, splits[splits.Length-1]);
                    Status = String.Format("Loading dataset ({0:0.0}%)", 5*(double)currentRecord / totalRecords);
                    currentRecord++;
                }
            }
        }

        public List<MovieRecommendation> GetRecommendationsForMovie(Movie _selectedMovie)
        {
            int movieId = _selectedMovie.ID;
            Dictionary<int, double> similarityResults = new Dictionary<int, double>();
            int length = _allMovies.Count;
            for (int i=0;i< _allMovies.Count; i++  )
            {
                int otherMovieId = _allMovies.Keys.ElementAt(i);
                if (movieId != otherMovieId)
                    similarityResults[otherMovieId] = CalculteSimilarity(movieId, otherMovieId);
                Status = String.Format("Searching similar movies ({0:0.0}%)", (double)100*i / length);


            }
            int amountOfResults = Math.Min(1000, similarityResults.Count);

            var topResults = similarityResults.OrderByDescending(pair => pair.Value).Take(amountOfResults)
                           .ToDictionary(pair => pair.Key, pair => pair.Value);
            List<MovieRecommendation> result = new List<MovieRecommendation>();
            int recommendedMovieId;
            double score;
            Movie otherMovie;
            for (int i = 0; i < amountOfResults; i++)
            {
                recommendedMovieId = topResults.ElementAt(i).Key;
                score = topResults.ElementAt(i).Value;
                otherMovie = _allMovies[recommendedMovieId];
                result.Add(new MovieRecommendation(otherMovie, i + 1, score));
            }
            return result;



        }

        private double CalculteSimilarity(int movieId, int otherMovieId)
        {
            if (!_ranksOfMovivesByUsers.ContainsKey(movieId) || !_ranksOfMovivesByUsers.ContainsKey(otherMovieId))
                return 0;
            Dictionary<int, double> movieRatings = _ranksOfMovivesByUsers[movieId];
            Dictionary<int, double> otherMovieRatings = _ranksOfMovivesByUsers[otherMovieId];
            var userIdsRatedMovie = movieRatings.Keys;
            List<double> movieRatingVector = new List<double>() ;
            List<double> otherMovieRatingVector = new List<double>();
            double movieRatingByUser;
            double otherMovieRatingByUser;
            foreach (int userId in userIdsRatedMovie)
            {
                if (!otherMovieRatings.ContainsKey(userId))
                    continue;
                movieRatingByUser = movieRatings[userId];
                movieRatingVector.Add(movieRatingByUser);
                otherMovieRatingByUser = otherMovieRatings[userId];
                otherMovieRatingVector.Add(otherMovieRatingByUser);
            }
            if (movieRatingVector.Count == 0)
                return 0;
            return  Cosine(movieRatingVector, otherMovieRatingVector) * Math.Min(movieRatingVector.Count / Distance(movieRatingVector, otherMovieRatingVector), movieRatingVector.Count) ;

        }

        private double Cosine(List<double> vectorA, List<double> vectorB)
        {
            double ab = 0;
            double aLength = CalculateVectorLength(vectorA);
            double bLength = CalculateVectorLength(vectorB);
            for (int i = 0; i < vectorA.Count; i++)
                ab += vectorA[i] * vectorB[i];
            return ab / (aLength * bLength);

        }

        private double Distance(List<double> vectorA, List<double> vectorB)
        {
            double distance = 0;
            for (int i = 0; i < vectorA.Count; i++)
                distance += Math.Pow(vectorA[i] - vectorB[i], 2);
            return Math.Sqrt(distance);

        }

        private double CalculateVectorLength(List<double> vectorA)
        {
            double length = 0;
            foreach (double x in vectorA)
                length += Math.Pow(x,2);
            return Math.Sqrt(length);

        }

        public List<Movie> GetAllMoviesExist()
        {
            return new List <Movie>(_allMovies.Values);
        }


        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}