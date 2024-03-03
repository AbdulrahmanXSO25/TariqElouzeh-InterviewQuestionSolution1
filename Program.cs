using System.Text;

class Program
{
    static void Main()
    {
        // a code snippet to make console print arabic letters..
        Console.OutputEncoding = Encoding.UTF8;

        var countries = API.GetCountries();

        var firstMatchedCountry = GetFirstMatchedCountry(countries);

        var countryWithRateRequestDto = new CountryWithRateRequestDto()
        {
            Name = firstMatchedCountry.Country,
            Rate = Helpers.GetCountryGrowthRateBetweenTheLastTwoPopulations(firstMatchedCountry.Population)
        };

        API.Post(countryWithRateRequestDto);
    }

    static CountryPopulationDataToReturnDto GetFirstMatchedCountry(List<CountryPopulationDataToReturnDto> countries)
    {
        foreach (var country in countries)
        {
            var countryWithCapital = API.GetCountryByName(country.Country);

            if (!Helpers.CapitalNameIsCountryName(countryWithCapital)) continue;

            if (Helpers.PopulationIsIncreasing(country.Population)) return country;
        }

        return null;
    }
}

public static class Helpers
{
    public static bool CapitalNameIsCountryName(CountryWithCapitalToReturnDto country)
    {
        return country.Capital.Contains(country.Name);
    }

    public static double GetCountryGrowthRateBetweenTheLastTwoPopulations(Dictionary<string, double> populations)
    {
        var populationArray = populations.Select(x => x.Value).ToArray();

        return Math.Round((populationArray[populationArray.Length - 1] / populationArray[populationArray.Length - 2] - 1) * 100, 2);
    }

    public static bool PopulationIsIncreasing(Dictionary<string, double> populations)
    {
        var populationArray = populations.Select(x => x.Value).ToArray();

        for (int i = 0; i < populationArray.Length; i++)
        {
            if (i != populationArray.Length - 1)
            {
                if (populationArray[i] > populationArray[i + 1])
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public static class API
{

    private static List<Country> _countries = new List<Country>();

    static API()
    {
        _countries = new List<Country>()
        {
            new Country("الأردن", "عمان", new Dictionary<string, double>
            {
                ["1990"] = 3.5f,
                ["1995"] = 4.6f,
                ["2000"] = 5.1f,
                ["2005"] = 5.8f,
                ["2010"] = 7.2f
            }),
            new Country("الكويت", "مدينة الكويت", new Dictionary<string, double>
            {
                ["1990"] = 1.67f,
                ["1995"] = 1.65f,
                ["2000"] = 1.93f,
                ["2005"] = 2.23f,
                ["2010"] = 2.94f
            }),
            new Country("الجزائر", "الجزائر العاصمة", new Dictionary<string, double>
            {
                ["1990"] = 25.5f,
                ["1995"] = 28.4f,
                ["2000"] = 30.7f,
                ["2005"] = 32.9f,
                ["2010"] = 35.8f
            })
        };
    }

    public static List<CountryPopulationDataToReturnDto> GetCountries()
    {
        var countries = _countries
            .Select(x => new CountryPopulationDataToReturnDto
            {
                Country = x.CountryName,
                Population = x.Population
            })
            .ToList();

        return countries;
    }

    public static CountryWithCapitalToReturnDto GetCountryByName(string countryName)
    {
        var country = _countries.FirstOrDefault(x => x.CountryName == countryName);
        if (country != null)
        {
            return new CountryWithCapitalToReturnDto() { Name = country.CountryName, Capital = country.CountryCapital };
        }
        return null;
    }

    public static void Post(CountryWithRateRequestDto countryWithRate)
    {
        var reversedCountryName = new string(countryWithRate.Name.Reverse().ToArray());

        Console.Write($"Success sending rate: {countryWithRate.Rate} for \"");

        var originalColor = Console.ForegroundColor;
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(reversedCountryName);
        
        Console.ForegroundColor = originalColor;

        Console.Write("\".");

        Console.WriteLine();
    }
}

public class CountryWithRateRequestDto
{
    public string Name { get; set; }
    public double Rate { get; set; }
}

public class CountryPopulationDataToReturnDto
{
    public string Country { get; set; }
    public Dictionary<string, double> Population { get; set; }
}

public class CountryWithCapitalToReturnDto
{
    public string Name { get; set; }
    public string Capital { get; set; }
}

public class Country
{
    public string CountryName { get; set; }

    public string CountryCapital { get; set; }

    public Dictionary<string, double> Population { get; set; }

    public Country(string countryName, string capital, Dictionary<string, double> population)
    {
        CountryName = countryName;
        CountryCapital = capital;
        Population = population;
    }
}