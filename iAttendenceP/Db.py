import mysql.connector  # Use mysql-connector-python
from mysql.connector import Error
from datetime import datetime

# Connection settings for MySQL
connection_config = {
    'host': 'localhost',  
    'database': 'iattendance',  
    'user': 'root',  
    'password': '1234',  
}

# Function to fetch the last attendance status (IN/OUT)
def get_last_status(emp_no):
    conn = None
    try:
        # Establish the database connection
        conn = mysql.connector.connect(**connection_config)
        if conn.is_connected():
            cursor = conn.cursor()
            # Execute the query to get the last attendance status
            cursor.execute(
                "SELECT IN_OUT_STATUS FROM Attendance WHERE EMP_NO = %s ORDER BY IN_TIME DESC LIMIT 1", 
                (emp_no,)
            )
            result = cursor.fetchone()
            return result[0] if result else None  # Returning IN_OUT_STATUS (1 for IN, 0 for OUT)
    except Error as e:
        print(f"Error fetching last status: {str(e)}")
        return None
    finally:
        if conn and conn.is_connected():
            conn.close()

# Function to log the employee's attendance
def log_attendance(emp_no, emp_name, last_status, device_id):
    conn = None
    try:
        # Establish the database connection
        conn = mysql.connector.connect(**connection_config)
        if conn.is_connected:
            cursor = conn.cursor()

            # Determine new status based on last status
            new_status = 0 if last_status == 1 else 1  # 1 for "IN", 0 for "OUT"

            # Get the current timestamp
            current_time = datetime.now()

            if new_status == 1:  # Clocking IN
                cursor.execute(
                    """
                    INSERT INTO Attendance 
                    (EMP_NO, EMP_NAME, IN_TIME, IN_OUT_STATUS, OUT_TIME, DEVICE_ID) 
                    VALUES (%s, %s, %s, %s, %s, %s)
                    """,
                    (emp_no, emp_name, current_time, new_status, None, device_id)  # OUT_TIME is set to NULL
                )
                print(f"Clocked IN for Employee No: {emp_no}, Name: {emp_name}, Device ID: {device_id}")
            else:  # Clocking OUT
                cursor.execute(
                    """
                    UPDATE Attendance 
                    SET OUT_TIME = %s, IN_OUT_STATUS = %s, DEVICE_ID = %s 
                    WHERE EMP_NO = %s AND IN_OUT_STATUS = 1
                    """,
                    (current_time, new_status, device_id, emp_no)
                )
                print(f"Clocked OUT for Employee No: {emp_no}, Name: {emp_name}, Device ID: {device_id}")

            # Save the transaction
            conn.commit()
            return new_status
    except Error as e:
        print(f"Error logging attendance: {str(e)}")
        return None
    finally:
        if conn and conn.is_connected():
            conn.close()
