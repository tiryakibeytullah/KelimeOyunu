using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsFormsApplication66666666666
{
    public partial class Form1 : Form
    {
        public void VeriTemizleme()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
        }        
        //Permütasyon algoritmasının olduğu kısım.
        //Burada textbox'a girilen harfleri kendi arasında komple karıştırır.
        public static IEnumerable<string> Permutate(string source)
        {
            if (source.Length == 1) return new List<string> { source };

            var permutations = from c in source
                               from p in Permutate(new String(source.Where(x => x != c).ToArray()))
                               select c + p;
            return permutations;
        }
        //Access veritabanı kelime kütüphanesine bağlantı kodu.
        OleDbConnection baglanti = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\ilhan\Desktop\Ayrık Yapılar - Kelime Projesi\WindowsFormsApplication66666666666\KelimeÖdevi.accdb");
        Random rastgele = new Random();
        int harfsayac = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.MaxLength = 8;
            radioButton1.Checked = true;
            listBox1.Visible = false;           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {         
            listBox1.Items.Clear();
            //Textboxta olan metini alıp char'a çevirdikten sonra dizinin içerisine atıyoruz.
            string metin = textBox1.Text;
            char[] karakterler = metin.ToCharArray();
            string[] harfler = new string[textBox1.Text.Count()];

            //Metinde olan harfleri dizi içerisine atıyoruz.
            for(int i=0;i<textBox1.Text.Count();i++)
            {
                harfler[i] = karakterler[i].ToString();
            }

            //Bu kısımda ise 8'in kombinasyonları uygulanıyor.En az 3 harfli kelime olucağı için kombinasyon 3'den başlıyor.
            int kombinasyon = 3;
            for(int i=0;i<=harfler.Length;i++)
            {
                var gkkelimeler = DenemeKombinasyon.Combinations(harfler,kombinasyon);

                foreach(var item in gkkelimeler)
                {
                    string x = string.Join("",item.ToArray());
                    listBox1.Items.Add(x);
                }
                kombinasyon++;
            }
            //Bu kısımda textbox'a girilen harfleri kendi arasında tamamıyla karıştırıyor.
            int counter = 1;
            foreach (var p in Permutate(textBox1.Text))
            {
                listBox1.Items.Add($"{p}");
            }
            //Burada ise listbox1'de aynı olan kelimeleri birbirinden ayırt etmek için siliyor.
            string[] silme = new string[listBox1.Items.Count];
            listBox1.Items.CopyTo(silme,0);
            var silme1 = silme.Distinct();
            listBox1.Items.Clear();
            foreach(string s in silme1)
            {
                listBox1.Items.Add(s);
            }
            MessageBox.Show("Harfleri karıştırma işlemi tamamlandı.","Uyarı");          
        }

        private void button2_Click(object sender, EventArgs e)
        {    
            /*Bu kısımda ise kullanıcının ya da random gelen harflerden oluşan karışık kelimeler ile,
              kendi kelime database'mizde olan kelimelerimiz eşleşiyor ise listbox2'ye yazdırılıyor.
            */                     
            listBox2.Items.Clear();
            baglanti.Open();
            OleDbCommand komut = new OleDbCommand("Select kelime from kelimeler", baglanti);
            OleDbDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {            
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == dr[0].ToString())//Aynı olan kelimeler
                    {
                        listBox2.Items.Add(dr[0].ToString());
                    }
                    if(dr[0].ToString().Length == listBox1.Items[i].ToString().Length+1)//Joker harf eklenerek aynı olan kelimeler
                    {
                        if (dr[0].ToString().Contains(listBox1.Items[i].ToString()))
                        {
                            listBox2.Items.Add(dr[0].ToString());
                        }
                    }                    
                }                                                          
            }
            baglanti.Close();
            //Listbox2'de aynı olan verileri temizliyor.
            string[] silme = new string[listBox2.Items.Count];
            listBox2.Items.CopyTo(silme, 0);
            var silme1 = silme.Distinct();
            listBox2.Items.Clear();
            foreach (string s in silme1)
            {
                listBox2.Items.Add(s);
            }
            //Hiç eşleşen kelime yok ise program hata mesajı verir.
            if(listBox2.Items.Count==0)
            {
                MessageBox.Show("Eşleşen kelime bulunamadı ..!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }       

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Textbox1'e sadece harf girişi yapılması için yazılan kod.
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                 && !char.IsSeparator(e.KeyChar);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Textbox1'e girilen harfleri küçük harfe çevirir.
            int verigirissayisi = textBox1.TextLength;                       
            textBox1.Text = textBox1.Text.ToLower();
        }      
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            button3.Visible = true;
            VeriTemizleme();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            button3.Visible = false;
            VeriTemizleme();
        }

        private void button3_Click(object sender, EventArgs e)
        {  //Random harflerin gelmesi için yazılan kod.        
            string girilenmetin = "";
            int harfler;
            string[] dizi = { "a", "b", "c", "ç", "d", "e", "f", "g", "ğ", "h", "ı", "i", "j", "k", "l", "m", "n", "o", "ö", "p", "r", "s", "ş", "t", "u", "ü", "v", "y", "z" };           
            do
            {
                harfler = rastgele.Next(0,dizi.Length);
                textBox1.Text += dizi[harfler];
                harfsayac++;
            } while (harfsayac<8);           
            if(textBox1.Text.Length==8)
            { button3.Enabled = false; }
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VeriTemizleme();
            button3.Enabled = true;
        }
        private void yeniOyunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;                      
            VeriTemizleme();                                     
        }

        private void oyunHakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("8 HARF BİR KELİME\n\nOyunumuz 8 adet harf ve ayriyeten de 1 adet joker harften oluşan (istenilen harfin yerine kullanılıcak) ufak çaplı bir oyundur.Amaç; gelen karışık harflerden oluşan bir kelime bulmak. DataBase'de yaklaşık 13bin adet kelimenin arasından eşlesen kelimeye göre puanlandırma yapılmaktadır.En az 3 harften oluşan anlamlı bir Türkçe kelime bulunduğunda program sonlanır.Kullanıcı ister 8 adet harfi kendisi el ile girebilir isterse de random olarak programa atama işlemi yaptırabilir.\n\nPuanlama\n\n3 Harf : 3 Puan\n4 Harf : 4 Puan\n5 Harf : 5 Puan\n6 Harf : 7 Puan\n7 Harf : 9 Puan\n8 Harf : 11 Puan\n9 Harf : 15 Puan\n", "OYUN BİLGİLENDİRME", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
        }
        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Çıkış Yapmak Ister misiniz ?", "ÇIKIŞ", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                this.Close();
                Application.Exit();
            }                    
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            //Puanlama sistemi.
            int adet = listBox2.Items.Count;
            if(adet!=0)
            {
                Random rnd = new Random();
                int sayi = rnd.Next(0, adet);
                listBox2.SelectedIndex = sayi;
                textBox3.Text = listBox2.SelectedItem.ToString();
            }
            if (textBox3.Text.Length == 3)
                textBox4.Text = "3";
            if (textBox3.Text.Length == 4)
                textBox4.Text = "4";
            if (textBox3.Text.Length == 5)
                textBox4.Text = "5";
            if (textBox3.Text.Length == 6)
                textBox4.Text = "7";
            if (textBox3.Text.Length == 7)
                textBox4.Text = "9";
            if (textBox3.Text.Length == 8)
                textBox4.Text = "11";
            if (textBox3.Text.Length == 9)
                textBox4.Text = "15";


        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Database'de eşlesen kelimenin anlamını getirmek için yazılan sql kodu.
            baglanti.Open();
            OleDbCommand komutgetir = new OleDbCommand("Select anlam from kelimeler where kelime=@p1", baglanti);
            komutgetir.Parameters.AddWithValue("@p1", listBox2.SelectedItem);
            OleDbDataReader dr = komutgetir.ExecuteReader();
            while (dr.Read())
            {              
                textBox2.Text = dr[0].ToString();
            }
            baglanti.Close();
        }
    }
    //Bu fonksiyonda ise harflerin kombinasyonu oluşturulmaktadır.
    public static class DenemeKombinasyon
    {
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } : elements.SelectMany((e, i) => elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }   
}
