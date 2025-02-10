import struct
import numpy as np
import face_recognition
import json

class RegisterFaceAPI:
    def __init__(self, database_path):
        self.database_path = database_path

    def register_face(self, image_path, emp_no, emp_name):
        try:
            # Load the image
            image = face_recognition.load_image_file(image_path)
            encoding = face_recognition.face_encodings(image)

            if len(encoding) == 0:
                print("No face found in the image.")
                return None

            encoding = encoding[0]  # Get the first face encoding

            # Create a dictionary to store employee data
            employee_data = {
                'employee_no': emp_no,
                'employee_name': emp_name,
                'face_encoding': encoding.tolist()  # Convert numpy array to list
            }

            # Serialize the dictionary to JSON
            json_data = json.dumps(employee_data).encode('utf-8')
            length = struct.pack('I', len(json_data))  # Pack length of the JSON data

            # Save the encoding to the .dat file
            with open(self.database_path, 'ab') as f:
                f.write(length)  # Write the length of the JSON
                f.write(json_data)  # Write the JSON data

            print(f"Face encodings saved successfully.")
            return {'message': f"Successfully registered face for employee {emp_name} ({emp_no})."}
        except Exception as e:
            print(f"Error during registration: {str(e)}")
            return None
