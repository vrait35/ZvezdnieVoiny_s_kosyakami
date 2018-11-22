using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace StarWars
{
    class Program
    {
        static void Main(string[] args)
        {
            StarWarsResponse starWarsResponse = new StarWarsResponse();
            Console.WriteLine("1-планета, 2-герой, 3-корабль");
            Console.WriteLine("введите номер: ");
            int numberUi = 0;
            bool isTrue = false;
            while (!isTrue)
            {
             isTrue=int.TryParse(Console.ReadLine(), out numberUi);
                if (!isTrue) Console.WriteLine("нормально введите");
            }
            isTrue = false;
            switch (numberUi)
            {
                case 1:
                    {
                        Console.WriteLine("введите id  планеты");
                        int idPlanet = int.Parse(Console.ReadLine());
                        List<Person> list = new List<Person>();
                        Planet planet= starWarsResponse.Planet(idPlanet, list);
                        if(planet!=null)
                        Console.WriteLine("имя планеты: "+planet.Name);
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("введите id чела: ");
                        int idPerson = int.Parse(Console.ReadLine());
                        Person person= starWarsResponse.People(idPerson);
                        if(person!=null)
                        Console.WriteLine("имя героя: "+person.Name);
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("введите id самолета:");
                        int idShip = int.Parse(Console.ReadLine());
                        Starship starship= starWarsResponse.ShipAdd(idShip);
                        if(starship!=null)
                        Console.WriteLine("имя корабля: "+starship.Name);
                        break;
                    }
            }   
        }
    }
}









