using CoordinateSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPBot
{
    static class Weather
    {
        private static Random r = new Random(DateTime.Now.Millisecond);
        static Coordinate Coords = new Coordinate(52.135712, 0);
        
        public static double RandomBiasedPow(double min, double max, int tightness, double peak)
        {
            // Calculate skewed normal distribution, skewed by Math.Pow(...), specifiying where in the range the peak is
            // NOTE: This peak will yield unreliable results in the top 20% and bottom 20% of the range.
            //       To peak at extreme ends of the range, consider using a different bias function

            double total = 0.0;
            double scaledPeak = peak / (max - min);
            /*
            if (scaledPeak < 0.2 || scaledPeak > 0.8)
            {
                throw new Exception("Peak cannot be in bottom 20% or top 20% of range.");
            }
            */
            double exp = GetExp(scaledPeak);

            for (int i = 1; i <= tightness; i++)
            {
                // Bias the random number to one side or another, but keep in the range of 0 - 1
                // The exp parameter controls how far to bias the peak from normal distribution
                total += BiasPow(r.NextDouble(), exp);
            }

            return ((total / tightness) * (max - min)) + min;
        }

        public static double GetExp(double peak)
        {
            return -12.7588 * Math.Pow(peak, 3) + 27.3205 * Math.Pow(peak, 2) - 21.2365 * peak + 6.31735;
        }

        public static double BiasPow(double input, double exp)
        {
            return Math.Pow(input, exp);
        }

        public static WeatherObject NextDay (WeatherObject lastDay)
        {
            int averageTemp = 0;
            int season = 0;
            switch (lastDay.Date.Month)
            {
                case 1:
                case 2:
                case 12:
                    averageTemp = (int)RandomBiasedPow(lastDay.High - 7, lastDay.High + 4, 3, lastDay.High);
                    if (averageTemp < -3) averageTemp = -3;
                    if (averageTemp > 8) averageTemp = 8;

                    season = 0;
                    break;
                case 3:
                case 4:
                case 5:
                    averageTemp = (int)RandomBiasedPow(lastDay.High - 7, lastDay.High + 7, 3, lastDay.High);
                    if (averageTemp < 5) averageTemp = 5;
                    if (averageTemp > 12) averageTemp = 12;
                    season = 1;
                    break;
                case 6:
                case 7:
                case 8:
                    averageTemp = (int)RandomBiasedPow(lastDay.High - 4, lastDay.High + 7, 3, lastDay.High);
                    if (averageTemp < 7) averageTemp = 7;
                    if (averageTemp > 18) averageTemp = 18;
                    season = 2;
                    break;
                case 9:
                case 10:
                case 11:
                    averageTemp = (int)RandomBiasedPow(lastDay.High - 7, lastDay.High + 7, 3, lastDay.High);
                    if (averageTemp < 5) averageTemp = 5;
                    if (averageTemp > 12) averageTemp = 12;
                    season = 3;
                    break;
                default:
                    break;
            }
            int maxTemp = averageTemp + r.Next(1, 5);
            int minTemp = averageTemp - r.Next(1, 5);
            return new WeatherObject(lastDay.Date.AddDays(1), maxTemp, minTemp, ChooseWeatherType(maxTemp, minTemp, season, (int)lastDay.Type), (int)RandomBiasedPow(1, 80, 4, 20), ChooseWindDirection(), (int)RandomBiasedPow(10, 100, 5, 70));

        }

        public static string ChooseWindDirection()
        {
            switch (r.Next(0, 7))
            {
                case 0:
                    return "N";
                case 1:
                    return "NE";
                case 2:
                    return "E";
                case 3:
                    return "SE";
                case 4:
                    return "S";
                case 5:
                    return "SW";
                case 6:
                    return "W";
                case 7:
                    return "NW";
            }
            return "error";
        }

        public static WeatherType ChooseWeatherType(int max, int min, int season, int lastWeather) 
        {
            int peak = 0;
            if (season == 0) peak = 5;
            if (season == 1 || season == 3) peak = 4;
            if (season == 2) peak = 1;

            int weatherType = (int)RandomBiasedPow(0, 11, 3, lastWeather);


            return (WeatherType)weatherType;
        }

        public static string GenerateSevenDays(List<WeatherObject> week, string output)
        {
            string border = "Weather/background.png";
           
            var fontCollection = new FontCollection();
            var parentFont = fontCollection.Install("Weather/main.ttf").CreateFont(70, FontStyle.Bold);
            var childFont = new Font(parentFont, 90);
            var subFont = new Font(parentFont, 35);

            using (Image<Rgba32> backgroundImage = Image.Load(border))
            {
                int imageX = 105;
                foreach (var day in week)
                {
                    string maxTemp = day.High + "°C";
                    string minTemp = day.Low + "°C";

                    var maxSize = TextMeasurer.Measure(maxTemp, new RendererOptions(parentFont));

                    var maxOffset = imageX + 70 - maxSize.Width / 2;

                    var minSize = TextMeasurer.Measure(minTemp, new RendererOptions(parentFont));

                    var minOffset = imageX + 300 - minSize.Width / 2;

                    string dayString = "";
                    if (day.Date == DateTime.Today)
                    {
                        dayString = "Today";
                    }
                    else if (day.Date == DateTime.Today.AddDays(1))
                    {
                        dayString = "Tomorrow";
                    }
                    else
                    {
                        dayString = day.Date.ToString($"ddd dd MMM");
                    }

                    var dateSize = TextMeasurer.Measure(dayString, new RendererOptions(parentFont));

                    var dateOffset = imageX + 200 - dateSize.Width / 2;

                    var image1 = Image.Load($"Weather/{day.Type.ToString()}.png");
                    backgroundImage.Mutate(x => x.DrawImage(image1, 1, new Point(imageX, 775)).DrawText(dayString, parentFont, Rgba32.Black, new PointF(dateOffset, 370)).DrawText(maxTemp, childFont, Rgba32.Black, new PointF(maxOffset, 445)).DrawText(minTemp, childFont, Rgba32.FromHex("#4E4E4E"), new PointF(minOffset, 445)));
                    string windDescriptor = "";
                    if (day.WindSpeed < 15) windDescriptor = "Light";
                    if (day.WindSpeed >= 15 && day.WindSpeed < 30) windDescriptor = "Moderate";
                    if (day.WindSpeed >= 30 && day.WindSpeed < 60) windDescriptor = "Strong";
                    if (day.WindSpeed >= 60) windDescriptor = "Gale Force";

                    string wind = $"{windDescriptor}, {day.WindSpeed} km/h {day.WindDirection}";

                    var windSize = TextMeasurer.Measure(wind, new RendererOptions(subFont));

                    var windOffset = 307 + (imageX - 105) - windSize.Width / 2;

                    Coords.GeoDate = day.Date;
                    string sunrise = "Sunrise: " + Coords.CelestialInfo.SunRise.GetValueOrDefault().ToString("hh:mm tt");
                    string sunset = "Sunset: " + Coords.CelestialInfo.SunSet.GetValueOrDefault().ToString("hh:mm tt");

                    var sunriseSize = TextMeasurer.Measure(sunrise, new RendererOptions(subFont));

                    var sunriseOffset = 307 + (imageX-105) - sunriseSize.Width / 2;

                    var sunsetSize = TextMeasurer.Measure(sunset, new RendererOptions(subFont));

                    var sunsetOffset = 307 + (imageX - 105) - sunsetSize.Width / 2;

                    string humidity = "Humidity: " + day.Humidity + "%";
                    var humiditySize = TextMeasurer.Measure(humidity, new RendererOptions(subFont));

                    var humidityOffset = 307 + (imageX - 105) - humiditySize.Width / 2;

                    backgroundImage.Mutate(x => x.DrawText(wind, subFont, Rgba32.Black, new PointF(windOffset, 570)).DrawText(sunrise, subFont, Rgba32.Black, new PointF(sunriseOffset, 620)).DrawText(sunset, subFont, Rgba32.Black, new PointF(sunsetOffset, 670)).DrawText(humidity, subFont, Rgba32.Black, new PointF(humidityOffset, 720)));
                    

                    imageX += 599;
                }
                backgroundImage.Save($"Weather/{output}.png");
                return $"Weather/{output}.png";
            }
        }

        public static string GenerateOneDay(WeatherObject day, string output)
        {
            string border = "Weather/one.png";


            var fontCollection = new FontCollection();
            var parentFont = fontCollection.Install("Weather/main.ttf").CreateFont(70, FontStyle.Bold);
            var childFont = new Font(parentFont, 90);
            var subFont = new Font(parentFont, 35);

            using (Image<Rgba32> backgroundImage = Image.Load(border))
            {
                int imageX = 105;
                string maxTemp = day.High + "°C";
                string minTemp = day.Low + "°C";

                var maxSize = TextMeasurer.Measure(maxTemp, new RendererOptions(parentFont));

                var maxOffset = 180 - maxSize.Width / 2;

                var minSize = TextMeasurer.Measure(minTemp, new RendererOptions(parentFont));

                var minOffset = 405 - minSize.Width / 2;

                string dayString = DateTime.Today.ToString($"ddd, dd MMM");

                var dateSize = TextMeasurer.Measure(dayString, new RendererOptions(parentFont));

                var dateOffset = 307 - dateSize.Width / 2;

                string windDescriptor = "";
                if (day.WindSpeed < 15) windDescriptor = "Light";
                if (day.WindSpeed >= 15 && day.WindSpeed < 30) windDescriptor = "Moderate";
                if (day.WindSpeed >=30 && day.WindSpeed < 60) windDescriptor = "Strong";
                if (day.WindSpeed >= 60) windDescriptor = "Gale Force";

                string wind = $"{windDescriptor}, {day.WindSpeed} km/h {day.WindDirection}";

                var windSize = TextMeasurer.Measure(wind, new RendererOptions(subFont));

                var windOffset = 307 - windSize.Width / 2;

                string sunrise = "Sunrise: " + Coords.CelestialInfo.SunRise.GetValueOrDefault().ToString("hh:mm tt");
                string sunset = "Sunset: " + Coords.CelestialInfo.SunSet.GetValueOrDefault().ToString("hh:mm tt");

                var sunriseSize = TextMeasurer.Measure(sunrise, new RendererOptions(subFont));

                var sunriseOffset = 307 - sunriseSize.Width / 2;

                var sunsetSize = TextMeasurer.Measure(sunset, new RendererOptions(subFont));

                var sunsetOffset = 307 - sunsetSize.Width / 2;

                string humidity = "Humidity: " + day.Humidity + "%";
                var humiditySize = TextMeasurer.Measure(humidity, new RendererOptions(subFont));

                var humidityOffset = 307 - humiditySize.Width / 2;

                var image1 = Image.Load($"Weather/{day.Type.ToString()}.png");
                backgroundImage.Mutate(x => x.DrawImage(image1, 1, new Point(imageX, 825)).DrawText(dayString, parentFont, Rgba32.Black, new PointF(dateOffset, 370)).DrawText(maxTemp, childFont, Rgba32.Black, new PointF(maxOffset, 445)).DrawText(minTemp, childFont, Rgba32.FromHex("#4E4E4E"), new PointF(minOffset, 445)).DrawText(wind, subFont, Rgba32.Black, new PointF(windOffset, 570)).DrawText(sunrise, subFont, Rgba32.Black, new PointF(sunriseOffset, 620)).DrawText(sunset, subFont, Rgba32.Black, new PointF(sunsetOffset, 670)).DrawText(humidity, subFont, Rgba32.Black, new PointF(humidityOffset, 720)));

                backgroundImage.Save($"Weather/{output}.png");
                return $"Weather/{output}.png";
            }
        }
    }

    class WeatherList
    {
        public List<WeatherObject> WeatherObjects;
        public DateTime DatePosted;

        public WeatherList()
        {
            WeatherObjects = new List<WeatherObject>();
        }
    }
    class WeatherObject
    {
        public DateTime Date;
        public int High;
        public int Low;
        public int WindSpeed;
        public string WindDirection;
        public int Humidity;
        public WeatherType Type;

        public WeatherObject(DateTime date, int high, int low, WeatherType type, int windSpeed, string windDirection, int humidity)
        {
            Date = date;
            High = high;
            Low = low;
            Type = type;
            WindSpeed = windSpeed;
            WindDirection = windDirection;
            Humidity = humidity;
        }
    }

    enum WeatherType
    {
        Sunny,
        PartlyCloudy,
        MostlyCloudy,
        Cloudy,
        Showers,
        Rain,
        HeavyRain,
        RainAndSnow,
        Snow,
        Hail,
        HeavySnow,
        Thunderstorm
    }
}
 