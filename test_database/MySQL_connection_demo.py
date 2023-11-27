#library: https://pypi.org/project/mysql-connector-python/
import mysql.connector

db = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="gip-WJ",
    database="crowd_management"
    )

mycursor = db.cursor(dictionary=True)

mycursor.execute("SELECT * FROM zones WHERE id=1")

for row in mycursor:
    for key, value in row.items():
        print(key + ": " + str(value))
    print("--------------")
