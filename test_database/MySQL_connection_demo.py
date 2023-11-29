#library: https://pypi.org/project/mysql-connector-python/
import mysql.connector

db = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="gip-WJ",
    database="crowd_management"
    )


mycursor = db.cursor(dictionary=True)
mycursor.execute("SELECT * FROM tickets")
for row in mycursor:
    for key, value in row.items():
        print(key + ": " + str(value))
    print("--------------")
# db.close()
input("press enter")

db.reconnect()

# mycursor = db.cursor(dictionary=True)
mycursor.execute("SELECT * FROM tickets")

for row in mycursor:
    for key, value in row.items():
        print(key + ": " + str(value))
    print("--------------")