# Sensor Data Correlation Coding Challenge

## Overview
My submission to the Canadian UAVs Intern Coding Challenge. This project correlates sensor data from two different sources (CSV and JSON) based on geographic proximity. It helps identify potential anomaly detections by matching sensor readings within a 100-meter accuracy range. Thank you for your consideration!


### Key Features
- Reads sensor data from CSV and JSON files
- Calculates geographic distance using Haversine formula
- Correlates sensor readings within 100 meters
- Handles coordinate validation
- Generates a JSON output of correlated sensor IDs

## Prerequisites
- .NET SDK
- NuGet Packages:
  - Newtonsoft.Json
  - CsvHelper

## Installation

### 1. Clone the Repository
```bash
git clone https://github.com/chrisskjodt/Intern-Coding-Challenge.git
cd Intern-Coding-Challenge\CodingChallenge
```

### 2. Install Dependencies
```bash
dotnet restore
```

## Usage

### Input Files
Place your sensor data files in the project root (provided data files included already):
- `SensorData1.csv`: First sensor's data in CSV format
- `SensorData2.json`: Second sensor's data in JSON format

### CSV/JSON Format
#### CSV Format
```
id,latitude,longitude
8,50.96850558,-114.3583294
23,51.82328031,-114.9146874
```

#### JSON Format
```json
[
  {
    "Id": 29,
    "Latitude": 36.08572847,
    "Longitude": 138.78546118
  }
]
```

### Running the Program
```bash
dotnet run
```

### Output
- Console will display correlated sensor IDs
- Generates `CorrelatedSensors.json` in the project root

## How It Works
1. Reads sensor data from CSV and JSON files
2. Validates coordinate values
3. Calculates geographic distance between readings
4. Identifies readings within 100 meters
5. Outputs correlated sensor IDs

## Distance Calculation
Uses the Haversine formula to calculate geographic distance, accounting for the Earth's curvature.

## Error Handling
- Validates coordinate ranges
- Handles missing or invalid input files
- Provides detailed error messaging

## Performance Considerations
- O(n²) correlation algorithm
- Breaks after first match to optimize performance

## Contact
Timothy Skjodt - timothy.skjodt@ucalgary.ca
