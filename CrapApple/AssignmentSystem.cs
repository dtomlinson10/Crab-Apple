using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal class AssignmentSystem
    {
        public String systemID;
        public List<String> choreList;
        public List<User> userList;
        public AssignmentSystem(String systemID, List<String> choreList, List<User> userList)
        {
            this.userList = new List<User>();
            this.choreList = new List<String>();
            this.systemID = "";
        }

        public void rankChores(List<Chore> choresList)
        {

        }

        public void beginAuction(List<Chore> choresList)
        {

        }
    }
}
