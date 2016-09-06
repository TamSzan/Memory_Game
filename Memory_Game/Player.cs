using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Memory_Game 
{
    class Player : IComparable
    {
        public string Name { get; }
        public int Score { set; get; }
        public int ID { get; }
        static int id = 0;
        public Label Lab { get; set; }

        public Player(string name,Label label)
        {
            Name = name;
            Score = 0;
            ID = ++id;
            Lab = label;
        }

        public override string ToString()
        {
            return "Player" + ID +":  " + Name + "\nScore:  " + Score;
        }

        public int CompareTo(object obj)
        {
            Player temp = (Player)obj;
            if (temp.Score > Score)
                return 1;
            else if (temp.Score < Score)
                return -1;
            else return 0;
        }
    }
}
