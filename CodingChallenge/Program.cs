using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

class SensorDataCorrelation
{
    // Sensor reading class for CSV data with lowercase property names
    private class CsvSensorReading
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    // Sensor reading class for JSON data
    private class JsonSensorReading
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    // Custom CSV mapping class to handle header matching
    private sealed class CsvSensorReadingMap : ClassMap<CsvSensorReading>
    {
        public CsvSensorReadingMap()
        {
            Map(m => m.id).Name("id");
            Map(m => m.latitude).Name("latitude");
            Map(m => m.longitude).Name("longitude");
        }
    }

    // Haversine formula to calculate distance between two geographic points
    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusMeters = 6371000; // Earth's radius in meters
        
        // Convert latitude and longitude to radians
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        // Haversine formula
        double dlat = lat2Rad - lat1Rad;
        double dlon = lon2Rad - lon1Rad;

        double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dlon / 2) * Math.Sin(dlon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusMeters * c;
    }

    static void Main()
    {
        try
        {
            // Get the directory of the current project
            string projectDirectory = Directory.GetCurrentDirectory();
            if (Directory.GetParent(projectDirectory)?.FullName is string parentDir)
            {
                projectDirectory = parentDir;
            }

            // Construct full paths to input files
            string csvFilePath = Path.Combine(projectDirectory, "SensorData1.csv");
            string jsonFilePath = Path.Combine(projectDirectory, "SensorData2.json");

            // Read CSV sensor data
            List<CsvSensorReading> csvReadings = new List<CsvSensorReading>();
            if (File.Exists(csvFilePath))
            {
                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Disable header validation
                    HeaderValidated = null,
                    // Ignore missing fields
                    MissingFieldFound = null
                }))
                {
                    // Register the custom mapping
                    csv.Context.RegisterClassMap<CsvSensorReadingMap>();

                    csvReadings = csv.GetRecords<CsvSensorReading>()?.ToList() ?? new List<CsvSensorReading>();
                }
            }
            else
            {
                Console.WriteLine($"CSV file not found: {csvFilePath}");
                return;
            }

            // Read JSON sensor data
            List<JsonSensorReading> jsonReadings = new List<JsonSensorReading>();
            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                jsonReadings = JsonConvert.DeserializeObject<List<JsonSensorReading>>(jsonContent) 
                    ?? new List<JsonSensorReading>();
            }
            else
            {
                Console.WriteLine($"JSON file not found: {jsonFilePath}");
                return;
            }

            // Dictionary to store correlated sensor readings
            var correlatedSensors = new Dictionary<int, int>();

            // Correlation logic
            foreach (var csvReading in csvReadings)
            {
                // Skip invalid coordinates
                if (IsInvalidCoordinate(csvReading.latitude, csvReading.longitude))
                    continue;

                foreach (var jsonReading in jsonReadings)
                {
                    // Skip invalid coordinates
                    if (IsInvalidCoordinate(jsonReading.Latitude, jsonReading.Longitude))
                        continue;

                    // Calculate distance between readings
                    double distance = CalculateDistance(
                        csvReading.latitude, csvReading.longitude, 
                        jsonReading.Latitude, jsonReading.Longitude
                    );

                    // If distance is within 100 meters, consider it a match
                    if (distance <= 100)
                    {
                        correlatedSensors[csvReading.id] = jsonReading.Id;
                        break; // Break after first match to avoid multiple correlations
                    }
                }
            }

            // Construct output file path
            string outputFilePath = Path.Combine(projectDirectory, "CorrelatedSensors.json");

            // Output results
            string outputJson = JsonConvert.SerializeObject(correlatedSensors, Formatting.Indented);
            File.WriteAllText(outputFilePath, outputJson);

            Console.WriteLine($"Sensor correlation completed. Results saved to {outputFilePath}");
            Console.WriteLine("Correlated Sensors:");
            foreach (var pair in correlatedSensors)
            {
                Console.WriteLine($"CSV Sensor ID: {pair.Key}, JSON Sensor ID: {pair.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }

    // Helper method to validate coordinates
    private static bool IsInvalidCoordinate(double latitude, double longitude)
    {
        return Math.Abs(latitude) > 90 || Math.Abs(longitude) > 180;
    }
}