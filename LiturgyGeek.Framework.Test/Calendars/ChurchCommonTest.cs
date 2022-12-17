using LiturgyGeek.Framework.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Test.Calendars
{
    [TestClass]
    public class ChurchCommonTest
    {
        [TestMethod]
        public void TestSerialization()
        {
            var original = new ChurchCommon
            {
                Occasions = new Dictionary<string, ChurchOccasion>
                {
                    { "christmas", new ChurchOccasion("Christmas", "The Nativity of Our Lord and Savior Jesus Christ") },
                },
            };
            var text = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<ChurchCommon>(text);

            Assert.AreEqual("{\"Occasions\":{\"christmas\":{\"Name\":\"Christmas\",\"LongName\":\"The Nativity of Our Lord and Savior Jesus Christ\"}}}", text);

            Assert.AreEqual("Christmas", result!.Occasions["christmas"].Name);
            Assert.AreEqual("The Nativity of Our Lord and Savior Jesus Christ", result!.Occasions["christmas"].LongName);
        }
    }
}
