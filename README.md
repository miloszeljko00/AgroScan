# AgroScan

AgroScan is a mobile application that allows users to take an image of a plant so that it can analyze it and detect if there is any disease present on the plant in the image. If it detects the disease, it will provide the users with a list of recommended agrochemicals they can use to treat that disease.

The project consists of three parts. The first one is the mobile application written in React Native. Its purpose is to provide users with an elegant interface to a system of disease detection and treatment recommendations. The second one is the web API written in .NET Core. Its purpose is to handle users' data about previous scans and recommendations and also to provide new recommendations for disease treatment using ontology. Recommendations are made by connecting active ingredients, from which agrochemicals are made, to diseases they cure. The last part is the web API written in Python Flask. Its role in this system is to provide access to CNN models for predicting diseases to a .NET Core web API. It takes an image as input and returns a predicted disease and plant to the .NET Core web API.

# Setup
1. Set up a PostgreSQL database (either locally or inside a Docker container).
2. Inside the .NET Core application update the "DefaultConnection" connection string in AgroScan.WebAPI/appsettings.json to point to your newly created database.
3. Inside the .NET Core application update the IP address in AgroScan.WebAPI/Properties/LaunchSettings.json to match your PC's IP address.
4. Inside the React Native application create .env file in the root folder and define the following variable: "EXPO_PUBLIC_AGRO_SCAN_API_BASE_URL" with value of .NET Core's application base URL(http://IP_ADDRESS:PORT, Do not use localhost! As it won't be accessable on your phone.)
5. Download the Expo Go application on your mobile phone.
6. Make sure your PC and phone are on the same network.
7. Install required packages for Python Flask web API (Flask, keras, Numpy and base64)
8. Run the Python Flask web API by starting the main.py script.
9. Run the .NET Core application in Visual Studio.
10. Run npm install in the React Native application
11. Run npm start in the React Native application

# Supported plants and diseases

## Tomato

Currently, the system can recognize 9 disease types on tomato plants. More about tomato disease detection can be read on paper in docs/papers/Detection of tomato leaf diseases.pdf

1. Bacterial spot
2. Early blight
3. Late blight
4. Target spot
5. Septoria leaf mold
6. Leaf mold
7. Spider mites
8. Tomato yellow leaf curl virus
9. Tomato mosaic virus


# Ontology

## Structure

* AgroChemical: A chemical product used in agriculture.
    * agroChemicalName: Name of the product.
    * agroChemicalManufacturer: The company manufacturing the product.
    * agroChemicalRepresentative: The company representing the product.
    * contains: The active material(s) within the product.
    * belongsToAgroChemicalType: The type of agrochemical (e.g., insecticide, fungicide).
* AgroChemicalType: Type/category of AgroChemical.
    * prevents: The disease type(s) it prevents.
    * AgroChemicalActiveMaterial: The active ingredient within an AgroChemical.
    * isActiveMaterial: The core active material.
* ActiveMaterial: The base chemical compound.
    * cures: The disease(s) it can treat.
* Disease: A disease affecting crops.
    * belongsToDiseaseType: Type of the disease.

## AgroChemical Recommendation Query
```SPARQL Query
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX agro: <http://agroscan.com/ontology/>

SELECT ?agroChemical ?agroChemicalName ?manufacturerName ?representativeName
WHERE {{
    ?agroChemical rdf:type agro:AgroChemical ;
        agro:contains ?agroChemicalActiveMaterial ;
        agro:belongsToAgroChemicalType ?agroChemicalType .

    ?agroChemicalType rdf:type agro:AgroChemicalType ;
        agro:prevents ?diseaseType .

    ?agroChemical agro:agroChemicalName ?agroChemicalName ;
        agro:agroChemicalManufacturer ?manufacturerName ;
        agro:agroChemicalRepresentative ?representativeName .

    ?agroChemicalActiveMaterial rdf:type agro:AgroChemicalActiveMaterial ;
        agro:isActiveMaterial ?activeMaterial .

    ?activeMaterial rdf:type agro:ActiveMaterial ;
        agro:cures <{diseaseUri}> .

    <{diseaseUri}> rdf:type agro:Disease ;
        agro:belongsToDiseaseType ?diseaseType .
```

This SPARQL query retrieves information about agrochemicals that can cure a particular disease. It searches for agrochemicals that contain active materials that target the disease specified by {diseaseUri}, and provides details like the agrochemical name, manufacturer, and representative for those effective agrochemicals.
