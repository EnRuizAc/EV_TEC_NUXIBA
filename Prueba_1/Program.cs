using System;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using ClosedXML.Excel;

//////////// In this first part, the database and its tables are created
//The first connection is made, to create the new database
string connectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True";
SqlConnection connection = new SqlConnection(connectionString);

connection.Open();

//The database is created
string createDBQuery = "CREATE DATABASE People";
SqlCommand creationCommand = new SqlCommand(createDBQuery, connection);
creationCommand.ExecuteNonQuery();

//The connection is closed, in order to create a new connection to the created database
connection.Close();

connectionString = "Data Source=localhost;Initial Catalog=People;Integrated Security=True";
connection = new SqlConnection(connectionString);

connection.Open();

//The two tables that need to be on the database are created
string createUsersQuery = "CREATE TABLE usuarios (userId INT PRIMARY KEY IDENTITY(1,1), Login VARCHAR(100), Nombre VARCHAR(100), Paterno VARCHAR(100), Materno VARCHAR(100))";
SqlCommand usersTableCommand = new SqlCommand(createUsersQuery, connection);
usersTableCommand.ExecuteNonQuery();

string createEmploysQuery = "CREATE TABLE empleados (userId INT, Sueldo FLOAT, FechaIngreso DATE, FOREIGN KEY (userId) REFERENCES usuarios(userID))";
SqlCommand EmploysTableCommand = new SqlCommand(createEmploysQuery, connection);
EmploysTableCommand.ExecuteNonQuery();

/////////// In this second part, the file is read, so the data can be processed and inserted into the created tables
string filePath = Directory.GetCurrentDirectory();
filePath += "\\DatosPracticaSQL.xlsx";

using (var workbook = new XLWorkbook(filePath))
{
    //First, the users worksheet is proccesed and analized
    var usersSheet = workbook.Worksheet(1); // Sheet 1
    var usersRange = usersSheet.RangeUsed();

    // This lists are created in order to store the data read from the file, in case is later needed to be used in another method
    List<string> userId = new List<string>();
    List<string> Nombre = new List<string>();
    List<string> Paterno = new List<string>();
    List<string> Materno = new List<string>();
    int rowNum1 = 0; // 
    //This string will contain the whole query, in order to only make one insert on the database, in order to reduce execution time
    string insertUsersQuery = "INSERT INTO usuarios (Login, Nombre, Paterno, Materno) VALUES ";

    // The rows are analized one by one
    foreach (var row in usersRange.RowsUsed().Skip(1)) // Se asume que la primera fila son encabezados
    {
        // The values on each column are read by row
        var columns = row.Cells().Select(c => c.Value.ToString()).ToList();

        //The values are stored on its corresponding list
        userId.Add(columns[0]);
        Nombre.Add(columns[1]);
        Paterno.Add(columns[2]);
        Materno.Add(columns[3]);

        insertUsersQuery += $"('{userId[rowNum1]}', '{Nombre[rowNum1]}', '{Paterno[rowNum1]}', '{Materno[rowNum1]}'), ";
        rowNum1++;
    }
    //This eliminates the last comma on the string
    insertUsersQuery = insertUsersQuery.Substring(0, insertUsersQuery.Length - 2);

    //The query is executed, and the table on the dabase is filled
    SqlCommand usersCommand = new SqlCommand(insertUsersQuery, connection);
    usersCommand.ExecuteNonQuery();



    //Now that the users are registered, the employs worksheet is proccesed and analized
    //This data requires some consult and manipulation, being a little more complex process
    var employsSheet = workbook.Worksheet(2); // Sheet 2
    var employsRange = employsSheet.RangeUsed();

    // This lists are created in order to store the data read from the file, in case is later needed to be used in another method
    List<string> userIdE = new List<string>();
    List<string> Salary = new List<string>();
    List<string> Date = new List<string>();
    List<DateOnly> DateFormatted = new List<DateOnly>();
    int rowNum2 = 0; // 

    //This string will contain the whole query, in order to only make one insert on the database, in order to reduce execution time
    string insertEmploysQuery = "INSERT INTO empleados (userId, Sueldo, FechaIngreso) VALUES ";
    // This string will be for make the query to obtain the right userId's to insert in the table
    string findKeyQuery = "SELECT userId FROM usuarios WHERE Login IN (";

    // The rows are analized one by one
    foreach (var row in employsRange.RowsUsed().Skip(1)) // Se asume que la primera fila son encabezados
    {
        // The values on each column are read by row
        var columns = row.Cells().Select(c => c.Value.ToString()).ToList();

        //The values are stored on its corresponding list
        userIdE.Add(columns[0]);
        Salary.Add(columns[1]);

        //The date readed is transformed into the correct format
        Date.Add(columns[2].Substring(0, columns[2].Length - 15));
        DateFormatted.Add(DateOnly.Parse(Date[rowNum2]));
        Date[rowNum2] = DateFormatted[rowNum2].ToString("yyyy/MM/dd");

        //The query is filled in order to request the Id's
        findKeyQuery += $"'{userIdE[rowNum2]}', ";
        rowNum2++;
    }

    //This eliminates the last comma on the string and adds the last parenthesis
    findKeyQuery = findKeyQuery.Substring(0, findKeyQuery.Length - 2) + ")";
    SqlCommand findIdCommand = new SqlCommand(findKeyQuery, connection);
    SqlDataReader reader = findIdCommand.ExecuteReader();

    //The query is executed, and the Ids are asigned. This is needed to obtain the right data to insert in the query
    int i = 0;
    while (reader.Read())
    {
        // The correct Ids are retrieved from the query
        string value = reader["userId"].ToString(); // Accessed by name
        insertEmploysQuery += $"({value}, '{Salary[i]}', '{Date[i]}'), ";
        i++;
    }
    reader.Close();

    //This eliminates the last comma on the string
    insertEmploysQuery = insertEmploysQuery.Substring(0, insertEmploysQuery.Length - 2);

    //The query is executed, and the table on the dabase is filled
    SqlCommand employsCommand = new SqlCommand(insertEmploysQuery, connection);
    employsCommand.ExecuteNonQuery();

    connection.Close();
}