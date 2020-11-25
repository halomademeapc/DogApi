using System.Linq;
using UnitsNet;

namespace DogApi
{
    public class Weight
    {
        private static readonly char Delimiter = '-';

        // Ranges provided in "x - y" format
        public string Imperial { get; set; }
        public string Metric { get; set; }

        public Mass? Minimum
        {
            get
            {
                var num = Metric.Split(Delimiter).FirstOrDefault()?.Trim();
                if (decimal.TryParse(num, out var dec))
                    return Mass.FromKilograms(dec);
                return default;
            }
        }

        public Mass? Maximum
        {
            get
            {
                var num = Metric.Split(Delimiter).LastOrDefault()?.Trim();
                if (decimal.TryParse(num, out var dec))
                    return Mass.FromKilograms(dec);
                return default;
            }
        }
    }

}
