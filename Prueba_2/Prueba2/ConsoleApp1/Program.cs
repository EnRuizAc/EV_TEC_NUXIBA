using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        //This class will be used to store the retrieved data from the database
        public class Persona
        {
            public int id { get; set;}
            public string login { get; set;}
            public string nombre { get; set;}
            public string paterno { get; set;}
            public string materno { get; set;}
            public double sueldo { get; set;}
            public DateTime fecha { get; set;}
        }

        static void Main()
        {
            List<Persona> personas = new List<Persona>();
            //The data required in order to obtain a successful connection
            string connectionString = "Data Source=localhost;Initial Catalog=People;Integrated Security=True";

            //The connection is opened and the methods areimplemented
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

               Console.WriteLine("Los primeros 10 usuarios registrados son los siguientes: \n");

               //The query for the first requirement
               string sqlQuery = "SELECT TOP 10 * FROM usuarios U JOIN empleados E ON U.userId = E.userId";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                //The data is read and stores in a List that contains all the data, so the creation of the csv file is easier
                while (reader.Read())
                {
                    int id = (int)reader["userId"];
                    string login = (string)reader["Login"];
                    string nombre = (string)reader["Nombre"];
                    string paterno = (string)reader["Paterno"];
                    string materno = (string)reader["Materno"];
                    double sueldo = (double)reader["Sueldo"];
                    DateTime date = (DateTime)reader["FechaIngreso"];

                    // The data is stored in a list of objects, in order to make easier the generation of the csv file
                    Persona persona = new Persona { id = (int)reader["userId"], login = (string)reader["Login"], nombre = (string)reader["Nombre"], paterno = (string)reader["Paterno"],
                                      materno = (string)reader["Materno"], sueldo = (double)reader["Sueldo"], fecha = (DateTime)reader["FechaIngreso"]};

                    personas.Add(persona);

                    Console.WriteLine($"userId: {id}, Login: {login}, Nombre: {nombre}, Paterno: {paterno}, Materno: {materno}, Sueldo: {sueldo}");
                }

                reader.Close();


                bool cont = true;
                ConsoleKeyInfo pressedKey;

                /////////////////////////////////////////
                while (cont)
                {
                    //The user is asked about the implmementation of the second requirement
                    Console.WriteLine("\n\nA continuación se presentan las opciones disponibles en el programa. Ingrese el número de la opción que desea ejecutar.");
                    Console.WriteLine("1. Generar un archivo CSV con los usuarios mostrados anteriormente (Top 10)");
                    Console.WriteLine("2. Actualizar el sueldo de algún usuario");
                    Console.WriteLine("3. Ingresar un nuevo usuario");
                    Console.WriteLine("0. Finalizar con la ejecución del programa");

                    pressedKey = Console.ReadKey();
                    Console.WriteLine("\n");

                    if ((pressedKey.Key == ConsoleKey.D1) || (pressedKey.Key == ConsoleKey.NumPad1))
                    {
                        Console.WriteLine("¿Desea generar un archivo CSV con los usuarios mostrados (Top 10)? [Y/N]");
                        ConsoleKeyInfo pressedNewKey = Console.ReadKey();

                        //The function is executed only if the user indicates so
                        if (pressedNewKey.Key == ConsoleKey.Y)
                        {
                            //Finding a good path for the file to be created
                            string filePath = Directory.GetCurrentDirectory();
                            //4 is the ammount of directories needed to be stepped out in order for the file to be in a good directory.
                            for (int i = 0; i < 4; i++)
                            {
                                filePath = filePath.Substring(0, filePath.LastIndexOf("\\"));
                            }
                            filePath += "\\UsuariosGenerados.csv";


                            using (StreamWriter writer = new StreamWriter(filePath))
                            {
                                // The header of the file are written
                                writer.WriteLine("Login, Nombre Completo, Sueldo, Fecha Ingreso");

                                // The file is filled with the top 10 users shown at the beginning of the app execution
                                for (int i = 0; i < personas.Count; i++)
                                {
                                    writer.WriteLine($"{personas[i].login}, {personas[i].nombre + " " + personas[i].paterno + " " + personas[i].materno}, {personas[i].sueldo}, {personas[i].fecha}");
                                }

                                // The StreamWriter is Closed
                                writer.Close();
                            }
                            Console.WriteLine("El archivo ha sido generado en la ruta: \n " + filePath + "\n");
                        }
                    }


                    //If the file isn't needed, then the app will continue to ask for the execution of the next requirements
                    else if ((pressedKey.Key == ConsoleKey.D2) || (pressedKey.Key == ConsoleKey.NumPad2))
                    {
                        //The second requirement is asked
                        Console.WriteLine("¿Deseas actualizar el salario de algún usuario? [Y/N]");
                        ConsoleKeyInfo pressedNewKey = Console.ReadKey();

                        if (pressedNewKey.Key == ConsoleKey.Y)
                        {
                            //Since this is the primary key on the database, this will be used to find a user
                            Console.WriteLine("\nPor favor introduce el ID del usuario del cuál te gustaría actualizar su salario: ");
                            string inputNumber = Console.ReadLine();
                            int numero;
                            int.TryParse(inputNumber, out numero);
                            double nuevoSueldo;

                            //This query is needed in order to obtain the data of the user, to later update its salary
                            string salaryQuery = $"SELECT * FROM usuarios U JOIN empleados E ON U.userId = E.userId WHERE U.userId = {numero}";
                            SqlCommand salaryCommand = new SqlCommand(salaryQuery, connection);


                            reader = salaryCommand.ExecuteReader();

                            while (reader.Read())
                            {
                                int id = (int)reader["userId"];
                                string login = (string)reader["Login"];
                                string nombre = (string)reader["Nombre"];
                                string paterno = (string)reader["Paterno"];
                                string materno = (string)reader["Materno"];
                                double sueldo = (double)reader["Sueldo"];
                                DateTime date = (DateTime)reader["FechaIngreso"];

                                Console.WriteLine("\n El ID ingresado, corresponde al siguiente usuario: \n");
                                Console.WriteLine("Nombre: " + nombre + " " + paterno + " " + materno + "\n");
                                Console.WriteLine("Su salario actual es de: $" + sueldo.ToString());
                            }
                            reader.Close();

                            //The new salary is requested and converted in the right data type
                            Console.WriteLine("Ingresa el nuevo sueldo: ");
                            string inputSueldo = Console.ReadLine();
                            double.TryParse(inputSueldo, out nuevoSueldo);
                            Console.WriteLine(nuevoSueldo.ToString());

                            //The salary is updated on the database
                            string salaryUpdateQuery = $"UPDATE empleados SET Sueldo = {nuevoSueldo} WHERE userId = {numero}";
                            SqlCommand salaryUpdateCommand = new SqlCommand(salaryUpdateQuery, connection);
                            salaryUpdateCommand.ExecuteNonQuery();
                        }
                    }

                    else if ((pressedKey.Key == ConsoleKey.D3) || (pressedKey.Key == ConsoleKey.NumPad3))
                    {
                        //The last requirement is asked
                        Console.WriteLine("\n¿Deseas agregar un nuevo usuario? [Y/N]");
                        ConsoleKeyInfo pressedNewKey = Console.ReadKey();

                        if (pressedNewKey.Key == ConsoleKey.Y)
                        {
                            //The data of the new user is requested and stored
                            Persona nuevoUsuario = new Persona();
                            Console.WriteLine("\nA continuación, se presentarán 1 a 1 los datos que deben de ser ingresados. Al finalizar de escribir cada dato, presiona Enter: ");
                            Console.WriteLine("Nombre: ");
                            nuevoUsuario.nombre = Console.ReadLine();
                            Console.WriteLine("Apelido Paterno: ");
                            nuevoUsuario.paterno = Console.ReadLine();
                            Console.WriteLine("Apellido Materno: ");
                            nuevoUsuario.materno = Console.ReadLine();
                            Console.WriteLine("Nombre de Usuario: ");
                            nuevoUsuario.login = Console.ReadLine();

                            Console.WriteLine("Sueldo: ");
                            double nuevoSueldo;
                            string inputSueldo = Console.ReadLine();
                            double.TryParse(inputSueldo, out nuevoSueldo);
                            nuevoUsuario.sueldo = nuevoSueldo;
                            nuevoUsuario.fecha = DateTime.Now.Date;

                            //The query to add the new user on the database
                            string newUserQuery = $"BEGIN TRANSACTION " +
                                $"INSERT INTO usuarios (login, nombre, paterno, materno) VALUES ('{nuevoUsuario.login}', '{nuevoUsuario.nombre}', '{nuevoUsuario.paterno}', '{nuevoUsuario.materno}') " +
                                $"DECLARE @userId INT = SCOPE_IDENTITY() " +
                                $"INSERT INTO empleados (userId, Sueldo, FechaIngreso) VALUES (@userId, {nuevoUsuario.sueldo}, '{nuevoUsuario.fecha.ToString("yyyy/MM/dd")}') COMMIT";

                            SqlCommand newUserCommand = new SqlCommand(newUserQuery, connection);
                            newUserCommand.ExecuteNonQuery();
                        }
                    }

                    else if ((pressedKey.Key == ConsoleKey.D0) || (pressedKey.Key == ConsoleKey.NumPad0))
                    {
                        //The loop is ended
                        cont = false;
                    }

                    else
                    {
                        //The menu is shown again, waiting for the selection of a valid option
                        Console.WriteLine("\n Por favor ingresa una opción válida:");
                    }
                }
                connection.Close();

            }
            //The program ends its execution
            Console.WriteLine("Presiona cualquier tecla para terminar la ejecución del programa...");
            Console.ReadKey();
        }
    }
}
