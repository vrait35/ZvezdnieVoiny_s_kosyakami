using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace StarWars
{
    public class StarWarsResponse
    {

        public Planet Planet(int index,List<Person> peoplesInPlanet)
        {           
            try
            {
                WebRequest planets = WebRequest.Create($"https://swapi.co/api/planets/{index}/");
                WebResponse responsePlanet = planets.GetResponse();

                Stream streamPlanet = responsePlanet.GetResponseStream();
                StreamReader readerPlanet = new StreamReader(streamPlanet);
                var resultPlanet = readerPlanet.ReadToEnd();

                //List<People> peoplesInPlanet = new List<People>();

                Planet objectPlanet = JsonConvert.DeserializeObject<Planet>(resultPlanet);

                using (SqlConnection sqlConnection = new SqlConnection())
                {                   
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "select * from Planets";
                    //sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    int isExist = 0;
                    while (reader.Read())
                    {
                        if (int.Parse(reader["id_Planet"].ToString()) == index) isExist++;                        
                    }                    
                    reader.Close();
                    if (isExist == 0)
                    {                     
                        sqlCommand.CommandText = "insert into Planets(id_Planet, climate, diameter, name,url) " +
                            "values(@id_Planet, @climate, @diameter, @name, @iji)";
                        var idParameteer = sqlCommand.CreateParameter();                       
                        sqlCommand.Parameters.Add(new SqlParameter() {
                            ParameterName = "@id_Planet",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = index,
                            IsNullable = false
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@climate",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Climate,
                            IsNullable = false
                        });
                        try
                        {
                            sqlCommand.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@diameter",
                                SqlDbType = System.Data.SqlDbType.Float,
                                Value = float.Parse(objectPlanet.Diameter),
                                IsNullable = false
                            });
                        }
                        catch (Exception ex) {
                            sqlCommand.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@diameter",
                                SqlDbType = System.Data.SqlDbType.Float,
                                Value = float.Parse(objectPlanet.Diameter),
                                IsNullable = false
                            });
                        }
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@name",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Name,
                            IsNullable = false
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@iji",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Url,
                            IsNullable = false
                        });

                        sqlCommand.ExecuteNonQuery();
                    }
                }

                long[] array = new long[objectPlanet.Residents.Count];
                string str="";
                int indexArray = 0;
                if (objectPlanet.Residents.Count > 0)
                {
                    Console.WriteLine("tut");
                    foreach (var item in objectPlanet.Residents)
                    {
                        for(int i=item.Length-2; ;)
                        {
                            Console.WriteLine("item[i]="+item[i]);
                            if (item[i] == '/') break;
                            str += item[i];
                            i--;
                        }
                        Console.WriteLine("str: "+str);
                        if (str.Length > 0)
                        {
                            Array.Reverse(str.ToArray());
                            array[indexArray] = long.Parse(str);
                            indexArray++;
                        }                     
                        WebRequest request = WebRequest.Create(item);
                        WebResponse response = request.GetResponse();

                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        var result = reader.ReadToEnd();

                        Person people = JsonConvert.DeserializeObject<Person>(result);
                        Console.WriteLine("peopleName:"+people.Name);
                        peoplesInPlanet.Add(people);
                    }
                                        
                    
                }
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    //string isTest = ConfigurationManager.AppSettings["isTest"].ToString();
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    sqlCommand.Connection = sqlConnection;
                    SqlDataReader reader1 = null;
                    for (int i = 0; i < indexArray; i++)
                    {
                        sqlCommand.CommandText = "update People set id_Planet=" + index.ToString() + "where id_Person=" + array[i].ToString();
                        reader1 = sqlCommand.ExecuteReader();
                        reader1.Close();
                    }
                    sqlCommand.CommandText = "select*from People";
                    reader1 = sqlCommand.ExecuteReader();
                    List<int> listIdPerson = new List<int>();
                    while (reader1.Read())
                    {
                        listIdPerson.Add(int.Parse(reader1["id_Person"].ToString()));
                    }
                    reader1.Close();
                    foreach (int num in listIdPerson)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (num == array[i]) array[i] = -1;
                        }
                    }
                    SqlCommand sqlCommand1;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > 0)
                        {
                         //   Console.WriteLine("CREATE PERSON");
                            sqlCommand1 = new SqlCommand();
                            sqlCommand1.Connection = sqlConnection;
                            //    sqlCommand1.CommandText = "insert People(id_Person,id_Planet,birth_year,eye_color,gender,hair_color,height," +
                            //        "homeworld,mass,name,skin_color,url) values('" +
                            //    array[i].ToString() + "','" + index.ToString() + "','" + peoplesInPlanet[i].BirthYear + "','" + peoplesInPlanet[i].EyeColor + "','" +
                            //    peoplesInPlanet[i].Gender + "','" + peoplesInPlanet[i].HairColor + "','" + peoplesInPlanet[i].Height + "','" +
                            //    peoplesInPlanet[i].Homeworld + "','" + peoplesInPlanet[i].Mass + "','" + peoplesInPlanet[i].Name + "','" +
                            //    peoplesInPlanet[i].SkinColor + "','"  + peoplesInPlanet[i].Url+ "')";
                            sqlCommand1.CommandText = "insert into People(id_Person,id_Planet,name) values(@id_Person,@id_Planet,@name)";
                            sqlCommand1.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@id_Person",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Value = array[i],
                                IsNullable=false
                            });
                            sqlCommand1.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@id_Planet",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Value = index,
                                IsNullable = false
                            });
                            sqlCommand1.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@name",
                                SqlDbType = System.Data.SqlDbType.VarChar,
                                Value = peoplesInPlanet[i].Name,
                                IsNullable = false
                            });
                            reader1 = sqlCommand1.ExecuteReader();
                            reader1.Close();
                        }
                    }
                    reader1.Close();
                }
                return objectPlanet;
            }
            catch (WebException ex) when (ex.Response != null)
            {
               // Console.WriteLine("cath");
                return null ;
            }
        }


        public Person People(int index)
        {
            try
            {
                WebRequest peopls = WebRequest.Create($"https://swapi.co/api/people/{index}/");
                WebResponse responsePeopls = peopls.GetResponse();

                Stream streamPeople = responsePeopls.GetResponseStream();
                StreamReader readerPeople = new StreamReader(streamPeople);
                var resultPeople = readerPeople.ReadToEnd();

                Person pl = JsonConvert.DeserializeObject<Person>(resultPeople);
                using (SqlConnection sqlConnection = new SqlConnection())
                {                   
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    int isExist = 0;
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "select * from People";
                    //sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if (int.Parse(reader["id_Person"].ToString()) == index) isExist++;                        
                    }
                    reader.Close();
                    if (isExist == 0)
                    {
                        Console.WriteLine("pd.name: "+pl.Name);
                        sqlCommand.CommandText = "insert into People(id_Person,name)" +
                            "values(@id_Person,@name)";

                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@id_Person",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = index,
                            IsNullable = false
                        });

                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@name",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value=pl.Name,
                            IsNullable = true
                        });                      
                        sqlCommand.ExecuteNonQuery();
                    }                  
                }
                return pl;
            }
            catch (WebException ex) when (ex.Response != null)
            {
                return null;
            }

            
        }


      



        public Starship ShipAdd(int index)
        {
            List<Person> peoplesInPlanet = new List<Person>();
            try
            {
                WebRequest planets = WebRequest.Create($"https://swapi.co/api/starships/{index}/");
                WebResponse responsePlanet = planets.GetResponse();

                Stream streamPlanet = responsePlanet.GetResponseStream();
                StreamReader readerPlanet = new StreamReader(streamPlanet);
                var resultPlanet = readerPlanet.ReadToEnd();

                //List<People> peoplesInPlanet = new List<People>();

                Starship objectPlanet = JsonConvert.DeserializeObject<Starship>(resultPlanet);

                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "select * from Ships";
                    //sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    int isExist = 0;
                    while (reader.Read())
                    {
                        if (int.Parse(reader["id_Ship"].ToString()) == index) isExist++;
                    }
                    reader.Close();
                    if (isExist == 0)
                    {                        
                        sqlCommand.CommandText = "insert into Ships(id_Ship, length, name) " +
                            "values(@id_Ship, @length, @name)";                     

                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@id_Ship",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = index,
                            IsNullable = false
                        });                                       
                            sqlCommand.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@length",
                                SqlDbType = System.Data.SqlDbType.Float,
                                Value = float.Parse(objectPlanet.Length),
                                IsNullable = false
                            });                                              
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@name",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Name,
                            IsNullable = false
                        });                       
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                long[] array = new long[objectPlanet.Pilots.Count];
                string str = "";
                int indexArray = 0;
                if (objectPlanet.Pilots.Count > 0)
                {
                   // Console.WriteLine("tut");
                    foreach (var item in objectPlanet.Pilots)
                    {
                        for (int i = item.Length - 2; ;)
                        {
                           // Console.WriteLine("item[i]=" + item[i]);
                            if (item[i] == '/') break;
                            str += item[i];
                            i--;
                        }
                       // Console.WriteLine("str: " + str);
                        if (str.Length > 0)
                        {
                            Array.Reverse(str.ToArray());
                            array[indexArray] = long.Parse(str);
                            indexArray++;
                        }
                      
                        WebRequest request = WebRequest.Create(item);
                        WebResponse response = request.GetResponse();

                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        var result = reader.ReadToEnd();

                        Person people = JsonConvert.DeserializeObject<Person>(result);
                     //   Console.WriteLine("peopleName:" + people.Name);                       
                        peoplesInPlanet.Add(people);
                    }


                }
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    //string isTest = ConfigurationManager.AppSettings["isTest"].ToString();
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    sqlCommand.Connection = sqlConnection;
                    SqlDataReader reader1 = null;
                    for (int i = 0; i < indexArray; i++)
                    {
                        sqlCommand.CommandText = "update People set id_Ship=" + index.ToString() + "where id_Person=" + array[i].ToString();
                        reader1 = sqlCommand.ExecuteReader();
                        reader1.Close();
                    }
                    sqlCommand.CommandText = "select*from People";
                    reader1 = sqlCommand.ExecuteReader();
                    List<int> listIdPerson = new List<int>();
                    while (reader1.Read())
                    {
                        listIdPerson.Add(int.Parse(reader1["id_Person"].ToString()));
                    }
                    reader1.Close();
                    foreach (int num in listIdPerson)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (num == array[i]) array[i] = -1;
                        }
                    }
                    SqlCommand sqlCommand1;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > 0)
                        {
                            Console.WriteLine("eho");
                           // Console.WriteLine("CREATE PERSON");
                            sqlCommand1 = new SqlCommand();
                            sqlCommand1.Connection = sqlConnection;
                            //   sqlCommand1.CommandText = "insert People(id_Person,id_Ship,birth_year,eye_color,gender,hair_color,height," +
                            //       "homeworld,mass,name,skin_color,url) values('" +
                            //   array[i].ToString() + "','" + index.ToString() + "','" + peoplesInPlanet[i].BirthYear + "','" + peoplesInPlanet[i].EyeColor + "','" +
                            //   peoplesInPlanet[i].Gender + "','" + peoplesInPlanet[i].HairColor + "','" + peoplesInPlanet[i].Height + "','" +
                            //   peoplesInPlanet[i].Homeworld + "','" + peoplesInPlanet[i].Mass + "','" + peoplesInPlanet[i].Name + "','" +
                            //   peoplesInPlanet[i].SkinColor + "','" + peoplesInPlanet[i].Url + "')";
                            sqlCommand1.CommandText = "insert into People(id_Person,id_Ship,name)" +
                                   "values(@id_Person,@id_Ship,@name)";
                            sqlCommand1.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@id_Person",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Value = array[i],
                                IsNullable = false
                            });
                            sqlCommand1.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@id_Planet",
                                SqlDbType = System.Data.SqlDbType.Int,
                                Value = index,
                                IsNullable = false
                            });
                            sqlCommand1.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@name",
                                SqlDbType = System.Data.SqlDbType.VarChar,
                                Value = peoplesInPlanet[i].Name,
                                IsNullable = false
                            });
                            reader1 = sqlCommand1.ExecuteReader();
                            reader1.Close();
                        }
                    }
                    reader1.Close();
                }
                return objectPlanet;
            }
            catch (WebException ex) when (ex.Response != null)
            {
               // Console.WriteLine("cath");
                return null;
            }
        }




    }
}
