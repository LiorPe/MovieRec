using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRec.Data
{
    [Serializable]
    public class DataCookie
    {
       public Dictionary<int, Dictionary<int, double>> RanksOfMovivesByUsers { get; set; }
       public Dictionary<int, Movie> AllMovies { get; set; }

    }
}
