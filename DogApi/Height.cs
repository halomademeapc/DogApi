using System.Linq;
using UnitsNet;

namespace DogApi
{
    public class Height
    {
        private static readonly char Delimiter = '-';

        public string Imperial { get; set; }
        public string Metric { get; set; }

        public Length? Minimum
        {
            get
            {
                var num = Metric.Split(Delimiter).FirstOrDefault()?.Trim();
                if (decimal.TryParse(num, out var dec))
                    return Length.FromCentimeters(dec);
                return default;
            }
        }

        public Length? Maximum
        {
            get
            {
                var num = Metric.Split(Delimiter).LastOrDefault()?.Trim();
                if (decimal.TryParse(num, out var dec))
                    return Length.FromCentimeters(dec);
                return default;
            }
        }
    }

}
