using System.Drawing;
using HexagonalTest.PlayerAPI;

namespace Hexagonal
{
    public class Player
    {
        private int id;
        private int fields;
        private int dices;
        private int bank;
        private Color color;

        public Player(int id, Color color, IPlayerLogic playerLogic)
        {
            this.id = id;
            this.dices = 0;
            this.fields = 0;
            this.bank = 0;
            this.color = color;
            this.PlayerLogic = playerLogic;
        }

        public int ID
        {
            get
            {
                return id;
            }
            internal set
            {
                this.id = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            internal set
            {
                this.color = value;
            }
        }

        public int Fields
        {
            get
            {
                return fields;
            }
            internal set
            {
                this.fields = value;
            }
        }

        public int Bank
        {
            get
            {
                return bank;
            }
            internal set
            {
                this.bank = value;
            }
        }

        public int Dices
        {
            get
            {
                return dices;
            }
            internal set
            {
                this.dices = value;
            }
        }

        internal IPlayerLogic PlayerLogic { get; }

        internal void addField()
        {
            this.fields++;
        }

        internal void removeField()
        {
            if (fields > 0)
            {
                this.fields--;
            }
        }
    }
}