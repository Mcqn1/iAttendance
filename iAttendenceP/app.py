import os
from flask import Flask, request, jsonify
from FaceMatch import FaceRecognitionAPI
from RegisterFaces import RegisterFaceAPI 
from Db import log_attendance, get_last_status

app = Flask(__name__)

# Define the path for the database file
database_path = "C:/Websites/iAttendance/FaceRec_100K.dat"

# Check if the .dat file exists, if not, create it
if not os.path.exists(database_path):
    with open(database_path, 'wb') as f:
        pass  # Just create an empty file

@app.route('/RegisterFace', methods=['POST'])
def register_face():
    data = request.get_json()
    strFileName = ""
    emp_name = ""
    emp_no = ""

    if data is not None:
        strFileName = data.get("filename")
        emp_no = data.get('employeeNo')
        emp_name = data.get('empName')

    if emp_no is None:
        return jsonify({'error': 'Employee No is required.'}), 400

    # Debugging - print received filename and employee details
    print(f"Received File: {strFileName}, Employee No: {emp_no}, Employee Name: {emp_name}")

    # Save the uploaded image temporarily
    image_path = os.path.join('C:/Websites/iAttendance/Registration', strFileName)

    # Check if image exists before face registration
    if not os.path.exists(image_path):
        return jsonify({"error": "Image file not found"}), 404

    try:
        # Create an instance of RegisterFaceAPI with the database path
        register_face_api = RegisterFaceAPI(database_path=database_path)
        
        # Pass employee name as well
        objReturn = register_face_api.register_face(image_path, emp_no, emp_name)

        # Log the response from registration
        print(f"Registration API response: {objReturn}")

    except Exception as e:
        print(f"Error during registration: {str(e)}")
        return jsonify({"error": f"Registration failed: {str(e)}"}), 500

    return jsonify(objReturn)  # Return registration response
    

@app.route('/FaceMatch', methods=['POST'])
def face_match():
    try:
        data = request.json
        image_name = data.get('imgFileName')
        device_id = data.get('deviceId')  # Extract device ID from the request payload

        if not image_name:
            return jsonify({"success": False, "message": "Image file name is required."}), 400

        # Debugging: Print received Device ID
        print(f"Device ID received: {device_id}")

        # Remove any existing file extension if necessary
        if image_name.endswith('.png'):
            image_name = image_name[:-4]  # Remove the last 4 characters

        attendance_path = "C:/Websites/iAttendance/CapturedImage"
        full_image_path = os.path.join(attendance_path, f"{image_name}.png")

        # Check if the file exists
        if not os.path.isfile(full_image_path):
            return jsonify({"success": False, "message": "File not found."}), 404

        # Load the face recognition API
        face_recognition_api = FaceRecognitionAPI(database_path=database_path)
        verification_results = face_recognition_api.VerifyFace(full_image_path, device_id)  # Pass device_id

        if not verification_results:
            return jsonify({"success": False, "message": "No matching face found."}), 404
        else:
            return verification_results

    except Exception as e:
        print(f"Error during face matching: {str(e)}")
        return jsonify({"success": False, "message": f"Face matching failed: {str(e)}"}), 500

if __name__ == '__main__':
    # Create necessary directories if they don't exist
    if not os.path.exists('temp_images'):
        os.makedirs('temp_images')

    app.run(debug=True)
