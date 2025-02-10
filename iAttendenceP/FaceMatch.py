import struct
import numpy as np
import face_recognition
import json
from Db import log_attendance, get_last_status

class FaceRecognitionAPI:
    def __init__(self, database_path):
        self.known_face_encodings = []
        self.known_face_names = []
        self.employee_nos = []  # To store employee numbers
        self.load_known_faces(database_path)

    def load_known_faces(self, database_path):
        try:
            with open(database_path, 'rb') as f:
                while True:
                    # Read the length of the JSON data first
                    length_bytes = f.read(4)
                    if not length_bytes:
                        break  # End of file
                    length = struct.unpack('I', length_bytes)[0]  # Get the length
                    
                    json_bytes = f.read(length)  # Read the JSON data
                    json_data = json_bytes.decode('utf-8')
                    employee_data = json.loads(json_data)  # Deserialize JSON to dictionary

                    # Extract employee information
                    self.known_face_encodings.append(np.array(employee_data['face_encoding']))
                    self.known_face_names.append(employee_data['employee_name'])
                    self.employee_nos.append(employee_data['employee_no'])  # Store employee number

            self.known_face_encodings = np.array(self.known_face_encodings)

        except Exception as e:
            print(f"Error loading face encodings from {database_path}: {str(e)}")

    def VerifyFace(self, image_path, device_id):
        try:
            unknown_image = face_recognition.load_image_file(image_path)
            unknown_encodings = face_recognition.face_encodings(unknown_image)

            if len(unknown_encodings) == 0:
                print("No face found in the image.")
                return None

            if len(self.known_face_encodings) == 0:
                print("No known face encodings to compare with.")
                return None

            unknown_encoding = unknown_encodings[0]

            distances = face_recognition.face_distance(self.known_face_encodings, unknown_encoding)
            matches = face_recognition.compare_faces(self.known_face_encodings, unknown_encoding, tolerance=0.4)

            for i, match in enumerate(matches):
                if match:
                    last_status = get_last_status(self.employee_nos[i])
                    new_status = log_attendance(self.employee_nos[i], self.known_face_names[i], last_status, device_id)
                    return [{'name': self.known_face_names[i], "status": new_status, 'employeeNo': self.employee_nos[i], 'distance': distances[i]}]

            return None
        except Exception as e:
            print(f"Error during face verification: {str(e)}")
            return None
