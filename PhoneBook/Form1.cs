using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        PhoneBook book;
        BindingSource source;
        AutoCompleteStringCollection completesource = new AutoCompleteStringCollection();
        public Form1()
        {
            InitializeComponent();
            book = new PhoneBook();
            source = new BindingSource();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Browse Text Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                await book.ReadFromFile(openFileDialog1.FileName);
                completesource.AddRange(book.ContactList.Select(c => c.ContactName).ToArray());
                textBox2.AutoCompleteCustomSource = completesource;
                textBox3.AutoCompleteCustomSource = completesource;
                rebind_data();
                FormTransform.TransformSize(this, 643, 776);
            }
        }

        private async void dataGridView1_CellClickAsync(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                {
                    Contact selectedContact = (Contact)dataGridView1.CurrentRow.DataBoundItem;
                    await book.CallContact(selectedContact);
                    rebind_data();
                }
                if (e.ColumnIndex == 1)
                {
                    Contact selectedContact = (Contact)dataGridView1.CurrentRow.DataBoundItem;
                    book.RemoveContact(selectedContact);
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    completesource.Remove(selectedContact.ContactName);
                    rebind_data();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void rebind_data()
        {
            Console.WriteLine("Calling Rebind.");
            source.DataSource = (from m in book.ContactList orderby m.ContactOutgoing descending, m.ContactName select m).ToList();
            dataGridView1.DataSource = source;
            dataGridView1.Update();
            dataGridView1.Refresh();
        }

        private async void btnAdd_ClickAsync(object sender, EventArgs e)
        {
            if (txtName.Text.Length <= 1)
            {
                MessageBox.Show("Common.. Input a proper name please.");
            }
            else if (txtNumber.Text.Length <= 1)
            {
                MessageBox.Show("Really..?! Is that a proper length number ?");
            }
            int status = await book.CreateContact(txtName.Text, txtNumber.Text);
            if (status == 0)
            {
                MessageBox.Show("Phone number not in proper format.");
            }
            else if (status == 2)
            {
                MessageBox.Show("Duplicate Entry. Contact already exists.");
            }
            completesource.Add(txtName.Text);
            rebind_data();
        }

        private void ctnDelete_Click(object sender, EventArgs e)
        {
            var contact = book.ContactList.Find(c => c.ContactName.Equals(textBox2.Text));
            if (contact != null)
            {
                book.ContactList.Remove(contact);
                DataGridViewRow row = dataGridView1.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["phonename"].Value.ToString().Equals(contact.ContactName)).First();
                dataGridView1.Rows.Remove(row);
                completesource.Remove(textBox2.Text);
            }
            else
            {
                MessageBox.Show("Contact doesn't exist. Make sure to select from suggested drop down list.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var contact = book.ContactList.Find(c => c.ContactName.Equals(textBox3.Text));
            if (contact != null)
            {
                label6.Text = contact.ContactNumber;
            }
            else
            {
                MessageBox.Show("Contact doesn't exist. Make sure to select from suggested drop down list.");
            }
        }
    }
}
