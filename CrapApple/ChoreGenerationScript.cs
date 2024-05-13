//V1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal class ChoreGenerationScript
    {
        private List<String> verbList = new List<String> { "Wash", "Sweep", "Mop", "Dust", "Vacuum", "Scrub", "Clean", "Wipe", "Polish", "Organize", "Sort", "Fold", "Iron", "Rinse", "Disinfect", "Scrub", "Sanitize", "Tidy", "Empty", "Dispose" };
        private List<String> nounList = new List<String> { "Lawn", "Shelves", "Garden", "Windows", "Floor", "Carpet", "Trash", "Laundry", "Clothes", "Plants", "Dishes" };

        public Chore GenerateChore(int idIterator)
        {
            //generate random name
            Random rnd = new Random();
            int verbIndex = rnd.Next(verbList.Count);
            int nounIndex = rnd.Next(nounList.Count);
            String verb = verbList[verbIndex];
            String noun = nounList[nounIndex];
            String choreName = verb + " the " + noun;
            string id = idIterator.ToString();
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly nextWeek = today.AddDays(7);

            //create new chore
            return new Chore(id, choreName, "A chore to " + verb + " the " + noun, rnd.Next(1, 7), null, nextWeek, false, false);
        }
    }
}
