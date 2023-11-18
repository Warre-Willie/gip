import mysql.connector

db = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="",
    database="demo school"
    )

mycursor = db.cursor(dictionary=True)

mycursor.execute("SELECT * FROM persoongegevens")

for row in mycursor:
    for key, value in row.items():
        print(key + ": " + str(value))
    print("--------------")