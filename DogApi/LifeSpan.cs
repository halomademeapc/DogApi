using System.Linq;
using UnitsNet;

namespace DogApi
{
    public class LifeSpan
    {
        private readonly string range;
        private readonly static char Delimiter = '-';

        public LifeSpan(string range)
        {
            this.range = range;
        }

        public Duration? Minimum
        {
            get
            {
                var num = range.Split(Delimiter).FirstOrDefault()?.Trim();
                if (decimal.TryParse(num, out var dec))
                    return Duration.FromYears365(dec);
                return default;
            }
        }

        public Duration? Maximum
        {
            get
            {
                var num = range.Split(Delimiter).LastOrDefault()?.Trim();
                if (decimal.TryParse(num, out var dec))
                    return Duration.FromYears365(dec);
                return default;
            }
        }
    }
}
