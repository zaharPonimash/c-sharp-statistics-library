using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsLibrary
{
    public class Analyzer
    {
        private double[] m_Values;

        public Analyzer(double[] theSeed)
        {
            if (theSeed.Count() == 0)
                throw new Exception("You must provide a valid dataset with at least 1 item.");

            m_Values = theSeed;
            Array.Sort(theSeed);
            Count = m_Values.Count();
        }

        #region Basic Functions
        /// <summary>
        /// This method returns the average of the array.
        /// </summary>
        /// <returns>Returns the average of the array.</returns>
        public double Mean()
        {
            return m_Values.Average();
        }

        /// <summary>
        /// This method returns the median value in the array.
        /// 
        /// If there is an even number of values, this returns the average of the 
        /// two centermost values.
        /// </summary>
        /// <returns>Returns the median of the array.</returns>
        public double Median()
        {
            int med = m_Values.Length / 2;
            if (m_Values.Length % 2 != 0)
            {
                return (Convert.ToDouble(m_Values[med]) + Convert.ToDouble(m_Values[med + 1])) / 2;
            }

            return m_Values[med];
        }

        /// <summary>
        /// This method returns the mode of the array.
        /// 
        /// The mode is the most common value in the array.
        /// </summary>
        /// <returns>Returns the mode of the array.</returns>
        public double Mode()
        {
            return m_Values.GroupBy(i => i)
                .OrderBy(i => i.Count())
                .Select(g => g.Key)
                .First();
        }

        /// <summary>
        /// Calculates the Standard Z Score of a given value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double StandardizedScore(double value)
        {
            return (value - Mean()) / PopulationStandardDeviation();
        }
        #endregion

        #region Range Functions
        /// <summary>
        /// Returns the largest value in the dataset
        /// </summary>
        /// <returns></returns>
        public double Max()
        {
            return m_Values.Max();
        }

        /// <summary>
        /// Returns the smallest value in the dataset
        /// </summary>
        /// <returns></returns>
        public double Min()
        {
            return m_Values.Min();
        }

        /// <summary>
        /// Returns the effective range of the dataset
        /// </summary>
        /// <returns></returns>
        public double Range()
        {
            dynamic a = m_Values.Max();
            dynamic b = m_Values.Min();

            return a - b;
        }

        /// <summary>
        /// Returns the first quartile, or median of the lower half of the dataset
        /// </summary>
        /// <returns></returns>
        public double FirstQuartile()
        {
            var median = Median();
            var filtered = m_Values.Where(x => Convert.ToDouble(x) <= median);

            return MedianWith(filtered);
        }

        /// <summary>
        /// Returns the third quartile, or median of the upper half of the dataset
        /// </summary>
        /// <returns></returns>
        public double ThirdQuartile()
        {
            var median = Median();
            var filtered = m_Values.Where(x => Convert.ToDouble(x) >= median);

            return MedianWith(filtered);
        }

        /// <summary>
        /// Returns the interquartile range, the distance between the first and third quartiles
        /// </summary>
        /// <returns></returns>
        public double InterquartileRange()
        {
            dynamic a = FirstQuartile();
            dynamic b = ThirdQuartile();
            return b - a;
        }
        #endregion

        #region Population Functions
        /// <summary>
        /// Calculates the variance of the population
        /// </summary>
        /// <returns></returns>
        public double PopulationVariance()
        {
            var mean = Mean();
            var sum = SumOf(x => x - mean, x => Math.Pow(x, 2));

            return sum / Count;
        }

        /// <summary>
        /// Calculates the standard deviation, sigma, of the population
        /// </summary>
        /// <returns></returns>
        public double PopulationStandardDeviation()
        {
            return Math.Sqrt(PopulationVariance());
        }

        /// <summary>
        /// Calculates the skewness of the population.
        /// Skewness is a relationship between the standard deviation and the third moment of the mean
        /// </summary>
        /// <returns></returns>
        public double PopulationSkewness()
        {
            var mean = Mean();
            var sum = SumOf(x => x - mean, x => Math.Pow(x, 3));

            return (1d / Count) * (sum / PopulationStandardDeviation());
        }

        /// <summary>
        /// Calculates the kurtosis of the population.
        /// Kurtosis is the relationship between the second and fourth moments of the mean
        /// </summary>
        /// <returns></returns>
        public double PopulationKurtosis()
        {
            var mean = Mean();

            var fourth = SumOf(x => x - mean, x => Math.Pow(x, 4));
            var second = SumOf(x => x - mean, x => Math.Pow(x, 2));

            if (second == 0)
                throw new Exception("Cannot calculate the population kurtosis with this dataset.");

            return Count * (fourth / Math.Pow(second, 2));
        }
        #endregion

        #region Sample Functions
        /// <summary>
        /// Calculates the variance of the sample
        /// </summary>
        /// <returns></returns>
        public double SampleVariance()
        {
            if (Count <= 1)
                throw new Exception("You must provide more than one value.");

            var mean = Mean();
            var sum = SumOf(x => x - mean, x => Math.Pow(x, 2));

            return sum / (Count - 1);
        }

        /// <summary>
        /// Calculates the standard deviation of the sample
        /// </summary>
        /// <returns></returns>
        public double SampleStandardDeviation()
        {
            return Math.Sqrt(SampleVariance());
        }

        /// <summary>
        /// Calculates the skewness of the sample. See PopulationSkewness for more information
        /// </summary>
        /// <returns></returns>
        public double SampleSkewness()
        {
            if (Count < 3)
                throw new Exception("In order to calculate sample skewness, you must provide at least three values.");

            var mean = Mean();
            var sum = SumOf(x => x - mean, x => Math.Pow(x, 3));

            
            return (Count / ((Count - 1d) * (Count - 2))) * (sum / SampleStandardDeviation());
        }

        /// <summary>
        /// Calculates the kurtosis of the sample. See PopulationKurtosis for more information
        /// </summary>
        /// <returns></returns>
        public double SampleKurtosis()
        {
            if (Count < 4)
                throw new Exception("You must provide at least four values in order to calculate the sample kurtosis.");

            var interim = PopulationKurtosis() / Count;

            return ((Count * (Count + 1) * (Count - 1)) / ((Count - 2) * (Count - 3))) * interim;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Helper method that calculates the sum of values based on passed in callback functions.
        /// For instance, passing in the difference from the mean and then raising the value to a power
        /// </summary>
        /// <param name="fns"></param>
        /// <returns></returns>
        private double SumOf(params Func<double, double>[] fns)
        {
            var values = m_Values.Select(x => Convert.ToDouble(x));
            foreach (var fn in fns)
            {
                values = values.Select(fn);
            }

            return values.Aggregate(0d, (a, x) => a + x);
        }
    
        /// <summary>
        /// This method helps calculate the median value for a subset of the main dataset
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private double MedianWith(IEnumerable<double> items)
        {
            int med = items.Count() / 2;
            if (items.Count() % 2 != 0)
            {
                dynamic a = items.ElementAt(med);
                dynamic b = items.ElementAt(med + 1);
                return (a + b) / 2;
            }

            return items.ElementAt(med);
        }
        #endregion

        #region Noise


        /// <summary>
        /// Генерация некоррелированного шума с равномерным распределением в заданном диапазоне
        /// </summary>
        /// <param name="random">Датчик СЧ</param>
        /// <param name="n">Длинна последовательности</param>
        /// <param name="min">Минимум</param>
        /// <param name="max">Максимум</param>
        public static double[] UniformDistribution(Random random, int n, double min = 0, double max = 1)
        {
            double magn = max - min;
            double[] result = new double[n];

            for (int i = 0; i < n; i++)
            {
                result[i] = magn * random.NextDouble() + min;
            }

            return result;
        }

        /// <summary>
        /// Генерация некоррелированного шума с нормальным распределением в заданном диапазоне
        /// </summary>
        /// <param name="random">Датчик СЧ</param>
        /// <param name="n">Длинна последовательности</param>
        /// <param name="mean">Среднее</param>
        /// <param name="std">Мат. ожидание</param>
        public static double[] NormalDistribution(Random random, int n, double mean = 0, double std = 1)
        {
            int m = 45;
            double[] uDs;
            double[] result = new double[n];
            

            for (int i = 0; i < m; i++)
            {
                uDs = UniformDistribution(random, n, -1, 1);

                for (int j = 0; j < n; j++)
                {
                    result[j] += uDs[j];
                }
            }

            for (int i = 0; i < n; i++)
            {
               result[i] = std * result[i] + mean;
            }



            return result;
        }

        #endregion

        public int Count { get; set; }
    }
}
