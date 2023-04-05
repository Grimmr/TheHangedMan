using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Game.Actions
{
    public class SelectOption : Action
    {
        public int Choice;
        public SelectOption(int c) 
        {
            Choice = c;
        }

        public override string ToString()
        {
            return "SELECT_OPTION " + Choice.ToString();
        }
    }
}
