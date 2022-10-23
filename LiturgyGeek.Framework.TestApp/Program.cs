using LiturgyGeek.Framework.Calendars;
using LiturgyGeek.Framework.Core;
using LiturgyGeek.Framework.Globalization;

namespace LiturgyGeek.Framework.TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var calendar = new ChurchCalendarSystem(new RevisedJulianCalendar(), new JulianPaschalCalendar());

            const int year = 2022;
            const int month = 4;

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            foreach (var churchEvent in calendar.ResolveAll(startDate, endDate,
                                                            OcaCalendar.Events.SelectMany(Event => Event.Dates.Select(Date => (Event, Date))))
                                        .OrderBy<(ChurchEvent Event, DateTime Date), DateTime>(e => e.Date))
            {
                Console.WriteLine($"{churchEvent.Date:d} {churchEvent.Event.Name}");
            }
        }

        private static ChurchCalendar OcaCalendar = new ChurchCalendar
        {
            Seasons = new[]
            {
                new ChurchSeason("ordinary", "1/1", "12/31", isDefault: true),
                new ChurchSeason("publican.pharisee", "-10/Sunday", "-10/Saturday"),
                new ChurchSeason("cheesefare", "-8/Monday", "-7/Sunday"),
                new ChurchSeason("lent", "-7/Monday", "-2/Friday"),
                new ChurchSeason("holy.week", "-1/Saturday", "-1/Saturday"),
                new ChurchSeason("pascha", "1/Sunday", "7/Saturday"),
                new ChurchSeason("pentecost.week", "8/Sunday", "8/Saturday"),
                new ChurchSeason("apostles.fast", "9/Monday", "6/28"),
                new ChurchSeason("dormition.fast", "8/1", "8/14"),
                new ChurchSeason("nativity.fast", "11/15", "12/19"),
                new ChurchSeason("nativity.forefeast", "12/20", "12/24"),
                new ChurchSeason("nativity.afterfeast", "12/26", "1/4"),
            },
            Events = new[]
            {
                ChurchEvent.ByName("-11/Sunday", "Zacchaeus Sunday"),
                ChurchEvent.ByName("-10/Sunday", "Publican and Pharisee"),
                ChurchEvent.ByName("-9/Sunday", "The Prodigal Son"),
                ChurchEvent.ByName("-8/Sunday", "The Last Judgment"),
                ChurchEvent.ByName("-8/Sunday", "Meatfare"),
                ChurchEvent.ByName("-7/Sunday", "Forgiveness Sunday"),
                ChurchEvent.ByName("-7/Sunday", "Cheesefare"),
                ChurchEvent.ByName("-6/Sunday", "Sunday of Orthodoxy"),
                ChurchEvent.ByName("-5/Sunday", "Gregory Palamas"),
                ChurchEvent.ByName("-4/Sunday", "Veneration of the Cross"),
                ChurchEvent.ByName("-3/Sunday", "John Climacus"),
                ChurchEvent.ByName("-2/Sunday", "Mary of Egypt"),
                ChurchEvent.ByName("-2/Saturday", "Lazarus Saturday"),
                ChurchEvent.ByName("-1/Sunday", "Palm Sunday"),
                ChurchEvent.ByName("-1/Monday", "Holy Monday"),
                ChurchEvent.ByName("-1/Tuesday", "Holy Tuesday"),
                ChurchEvent.ByName("-1/Wednesday", "Holy Wednesday"),
                ChurchEvent.ByName("-1/Thursday", "Holy Thursday"),
                ChurchEvent.ByName("-1/Friday", "Holy Friday"),
                ChurchEvent.ByName("-1/Saturday", "Holy Saturday"),
                ChurchEvent.ByName("1/Sunday", "Pascha"),
                ChurchEvent.ByName("1/Monday", "Bright Monday"),
                ChurchEvent.ByName("1/Tuesday", "Bright Tuesday"),
                ChurchEvent.ByName("1/Wednesday", "Bright Wednesday"),
                ChurchEvent.ByName("1/Thursday", "Bright Thursday"),
                ChurchEvent.ByName("1/Friday", "Bright Friday"),
                ChurchEvent.ByName("1/Saturday", "Bright Saturday"),
                ChurchEvent.ByName("2/Sunday", "St Thomas Sunday"),
                ChurchEvent.ByName("3/Sunday", "Myrrhbearing Women"),
                ChurchEvent.ByName("4/Sunday", "Paralytic"),
                ChurchEvent.ByName("4/Wednesday", "Midfeast of Pentecost"),
                ChurchEvent.ByName("5/Sunday", "Samaritan Woman"),
                ChurchEvent.ByName("6/Sunday", "Blind Man"),
                ChurchEvent.ByName("6/Wednesday", "Leavetaking of Pascha"),
                ChurchEvent.ByName("6/Thursday", "Ascension"),
                ChurchEvent.ByName("7/Sunday", "Fathers of 1st Ecumenical Council"),
                ChurchEvent.ByName("8/Sunday", "Pentecost"),
                ChurchEvent.ByName("9/Sunday", "All Saints"),
                ChurchEvent.ByName("9/Monday", "Apostles' Fast Begins"),
                ChurchEvent.ByName("10/Sunday", "All Saints of North America"),
                ChurchEvent.ByName("11/Sunday", "All Saints of Britain and Ireland"),
                ChurchEvent.ByName("8/1", "Dormition Fast Begins"),
                ChurchEvent.ByName("11/15", "Nativity Fast Begins"),
                ChurchEvent.ByName("9/8", "Nativity of the Theotokos"),
                ChurchEvent.ByName("9/14", "Elevation of the Cross"),
                ChurchEvent.ByName("11/21", "Entrance of the Theotokos"),
                ChurchEvent.ByName("12/25", "Christmas"),
                ChurchEvent.ByName("1/6", "Theophany"),
                ChurchEvent.ByName("2/2", "Meeting of Our Lord"),
                ChurchEvent.ByName("3/25", "Annunciation"),
                ChurchEvent.ByName("8/6", "Transfiguration"),
                ChurchEvent.ByName("8/15", "Dormition"),
                ChurchEvent.ByName("1/1", "Circumcision of Our Lord"),
                ChurchEvent.ByName("1/1", "Basil the Great"),
                ChurchEvent.ByName("1/17", "Anthony the Great"),
                ChurchEvent.ByName("1/30", "Three Holy Hierarchs"),
                ChurchEvent.ByName("2/27", "Raphael of Brooklyn"),
                ChurchEvent.ByName("3/18", "Nikolai of Zhicha"),
                ChurchEvent.ByName("3/31", "Innocent of Alaska (Repose)"),
                ChurchEvent.ByName("10/6", "Innocent of Alaska (Glorification"),
                ChurchEvent.ByName("4/7", "Tikhon of Moscow (Repose)"),
                ChurchEvent.ByName("10/9", "Tikhon of Moscow (Glorification)"),
                ChurchEvent.ByName("4/23", "St George"),
                ChurchEvent.ByName("5/7", "Alexis Toth"),
                ChurchEvent.ByName("5/8", "St John"),
                ChurchEvent.ByName("5/11", "Cyril and Methodius"),
                ChurchEvent.ByName("5/21", "Constantine and Helen"),
                ChurchEvent.ByName("6/24", "Nativity of John the Baptist"),
                ChurchEvent.ByName("6/29", "Peter and Paul"),
                ChurchEvent.ByName("7/5", "Sergius of Rádonezh (Relics)"),
                ChurchEvent.ByName("9/25", "Sergius of Rádonezh (Repose)"),
                ChurchEvent.ByName("7/15", "Prince Vladimir"),
                ChurchEvent.ByName("7/19", "Seraphim of Sárov (Relics)"),
                ChurchEvent.ByName("1/2", "Seraphim of Sárov (Repose)"),
                ChurchEvent.ByName("7/20", "Elijah"),
                ChurchEvent.ByName("7/26", "Yakov Netsvétov"),
                ChurchEvent.ByName("8/1", "Wood of the Cross"),
                ChurchEvent.ByName("8/9", "Herman of Alaska (Glorification)"),
                ChurchEvent.ByName("12/13", "Herman of Alaska (Repose)"),
                ChurchEvent.ByName("8/13", "Tikhon of Zadónsk"),
                ChurchEvent.ByName("8/29", "Beheading of John the Baptist"),
                ChurchEvent.ByName("9/1", "Church New Year"),
                ChurchEvent.ByName("9/24", "Hieromonk Juvenaly and Peter the Aleut"),
                ChurchEvent.ByName("10/1", "Protection of the Theotokos"),
                ChurchEvent.ByName("10/31", "John Kochurov"),
                ChurchEvent.ByName("11/8", "Archangel Michael and the Other Bodiless Powers"),
                ChurchEvent.ByName("12/4", "Alexander Hotovitzky"),
                ChurchEvent.ByName("12/6", "Nicholas of Myra"),
            },
        };
    }
}