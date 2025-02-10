import requests

def get_location_from_ip(ip_address):
    try:
        # Make a GET request to the IP info API with the provided IP address
        url = f"https://ipinfo.io/{ip_address}/json"
        response = requests.get(url)
        
        if response.status_code == 200:
            data = response.json()
            
            # Extract location (latitude, longitude) from the response
            location = data.get("loc", None)
            if location:
                lat, lon = location.split(",")
                
                # Print latitude and longitude for debugging
                print(f"Latitude: {lat}, Longitude: {lon}")
                
                return {"latitude": lat, "longitude": lon}  # return as a dictionary
            else:
                return {"error": "Location not found for this IP address."}
        else:
            return {"error": f"Error retrieving location: {response.status_code}"}
    
    except Exception as e:
        return {"error": f"An error occurred: {str(e)}"}

# # Example usage
# ip_address = "115.98.3.86"
# location_data = get_location_from_ip(ip_address)
# print(location_data)  # Print the result (including latitude and longitude)
