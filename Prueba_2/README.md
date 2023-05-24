## Directorio para la prueba 1 ##
I decided to create a program that could create the database and the tables,
read the xlsx file to obtain the data,
and then, insert the data into the created tables.

The queries I used for this, can be found throughout the "Program.cs" file in the "Prueba_1" folder.

- The query to create the database:
  CREATE DATABASE People

- The query to create the table "usuarios":
  CREATE TABLE usuarios (userId INT PRIMARY KEY IDENTITY(1,1), Login VARCHAR(100), Nombre VARCHAR(100), Paterno VARCHAR(100), Materno VARCHAR(100))

- The query to create the table "empleados":
  CREATE TABLE empleados (userId INT, Sueldo FLOAT, FechaIngreso DATE, FOREIGN KEY (userId) REFERENCES usuarios(userID))

- The query to insert the data into the "usuarios" table starts with:
  INSERT INTO usuarios (Login, Nombre, Paterno, Materno) VALUES ...
  And then, just the values are added in the right format

- The query to obtain the foreign keu "userId" from the "usuarios" table, is obtained through the "login" field. It starts with:
  SELECT userId FROM usuarios WHERE Login IN ...
  And then, just the values are inserted into the query to obtain the right data to insert

- The query to insert the data into the "empleados" trable in the right format, starts with:
  INSERT INTO empleados (userId, Sueldo, FechaIngreso) VALUES ...

And those are all the queries used to create and fill the database the way it was requested.

Now, the specific queries requested at the end of "Prueba_1" are these:

- The query to filter the ID's different than 6, 7, 9 and 10 on the "usuarios" table:
  SELECT * FROM usuarios WHERE userId NOT IN (6, 7, 9, 10)

- The query used to update and increase "Sueldo" on a 10% rate on the employees from between years 2000 and 2001:
  UPDATE empleados SET Sueldo = Sueldo * 1.1 WHERE FechaIngreso BETWEEN '2000' and '2001'

- The query used to obtain the username and the "FechaIngreso" of the employees that have a "Sueldo" greater than 10,000
  and whose last name starts with "T":
  SELECT U.Login, E.FechaIngreso FROM usuarios U JOIN empleados E ON U.userId = E.userId WHERE E.Sueldo > 10000 AND LEFT(U.Paterno, 1) = 'T'
  
- And finally, the query meant to filter the employees acording to their "Sueldo", depending if it is equal or greater than 1200;
  or if it is less than that amount, obtaining the count of members in each of the groups:
  SELECT CASE WHEN Sueldo < 1200 THEN 'Menor a 1200' ELSE 'Mayor o igual a 1200' END AS Salario, COUNT(*) AS CantidadMiembros
  FROM empleados GROUP BY CASE WHEN Sueldo < 1200 THEN 'Menor a 1200' ELSE 'Mayor o igual a 1200' END;

  As final notes, the program on "Prueba_1" was developed on Visual Studio Code, while the application for "Prueba_2" was developed on Visual Studio.