using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.Models
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;

        public PowerEntity()
        {

        }

        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
    }
}
