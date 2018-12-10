namespace PhoneBook
{
    class Contact
    {
        private string name;
        private string number;
        private int callcount;

        public Contact(string name, string number, int callcount = 0)
        {
            this.name = name;
            this.number = number;
            this.callcount = callcount;
        }

        public string ContactName
        {
            get { return name; }
            set { name = value; }
        }

        public string ContactNumber
        {
            get { return number; }
            set { number = value; }
        }

        public int ContactOutgoing
        {
            get { return callcount; }
            set { callcount = value; }
        }

        public void Call()
        {
            callcount++;
        }
    }
}
