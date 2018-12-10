using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhoneBook
{
    class PhoneBook
    {
        private List<Contact> Contacts;

        public List<Contact> ContactList
        {
            get { return Contacts; }
            set { Contacts = value; }
        }

        public PhoneBook()
        {
            ContactList = new List<Contact>();
        }

        public async Task<int> CreateContact(string name, string number)
        {
            string phone = await CheckNumber(number);
            if(ContactList.Exists(c => c.ContactName == name & c.ContactNumber == phone))
            {
                return 2;
            }
            else if (phone != string.Empty)
            {
                AddContactToList(new Contact(name, phone));
                return 1;
            }
            return 0;
        }

        private void AddContactToList(Contact newContact)
        {
            ContactList.Add(newContact);
        }

        private void RemoveContactFromList(string name)
        {
            var toRemove = ContactList.DefaultIfEmpty(null).First(c => c.ContactName.Equals(name));
            ContactList.Remove(Contacts.Find(c => c.ContactName.Equals(name)));
        }

        public Task CallContact(Contact contact)
        {
            return Task.Run(() =>
            {
                ContactList.Find(c => c == contact).Call();
            });            
        }

        public void RemoveContact(Contact contact)
        {
            ContactList.Remove(contact);
        }

        public Task<string> CheckNumber(string number)
        {
            return Task.Run(() =>
            {
                /*
                 * Match using regex for mentioned patterns
                 * 0878123456
                 * +359878123456
                 * 00359878123456
                */
                string PhonePattern = @"(?:(00359){1}|(\+359){1}|(0))(8[7-9][2-9]{1}[0-9]{0,6})";

                Match match = Regex.Match(number, PhonePattern);
                if (match.Success)
                {
                    /*
                     * If number matches successfully grab only the 4-th group
                     * which is the number starting after the operato's code till the end
                     * We can add the normalization pre-fix +359 that way regardless of the begining
                    */
                    return "+359" + match.Groups[4].ToString(); // Hack-fixing for the time being
                }
                else
                {
                    //Invalid - As Assignment Document suggest, we Ignore Them
                    return string.Empty;
                }
            });
        }

        public Task<bool> ReadFromFile(string filePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    Parallel.ForEach(File.ReadLines(filePath), async line =>
                    {
                        Console.WriteLine(line);
                        string[] ContactInfo = RemoveUnwantedCharacters(line).Split(',');
                        string number = await CheckNumber(ContactInfo[1]);
                        if (number != string.Empty)
                        {
                            AddContactToList(new Contact(ContactInfo[0], number));
                            Console.Write(ContactList.ToString());
                        }
                    });
                    Console.Write(ContactList.ToString());
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong" + e);
                    return false;
                }
            });
        }

        private static readonly Regex RemovalRegex = new Regex(@"[().]");

        public static string RemoveUnwantedCharacters(string input)
        {
            return RemovalRegex.Replace(input, "");
        }
    }
}
