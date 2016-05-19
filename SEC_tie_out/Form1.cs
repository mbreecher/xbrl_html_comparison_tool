using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;

namespace SEC_tie_out
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParseInstanceXML();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        #region parse files

        //public List<context> ParseInstanceXML()
        public void ParseInstanceXML()
        {
            Dictionary<string, string> namespaces = new Dictionary<string, string>();

            XDocument xdoc = ReadXMLFile();
            // create namespace lookup to properly populate tag names
            foreach (XAttribute attr in xdoc.Root.Attributes())
                namespaces.Add(attr.Value, attr.Name.LocalName);
            //namespaces[ns.Key]

            foreach (XElement el in xdoc.Root.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "context":
                        context temp = new context(el.FirstAttribute.Value);

                        foreach (XElement child in el.Descendants())
                        {
                            if (child.Name.LocalName == "instant" | child.Name.LocalName == "startDate")
                            {
                                temp.Start = Convert.ToDateTime(child.Value);
                            }
                            else if (child.Name.LocalName == "endDate")
                            {
                                temp.End = Convert.ToDateTime(child.Value);
                            }
                            foreach (XAttribute child_attr in child.Attributes())
                            {
                                if (child.Name.LocalName == "explicitMember")
                                {
                                    temp.add_axis_member(child_attr.Value, child.Value);
                                }
                            }
                        }

                        temp.print();
                        break;

                    case "unit":
                        Console.WriteLine("Unit");
                        Console.ReadLine();
                        break;

                    case "schemaRef":
                        // skip
                        break;

                    default:
                        Console.WriteLine("Fact");
                        Console.WriteLine("{0}:{1}", namespaces[el.Name.Namespace.ToString()], el.Name.LocalName);
                        Console.ReadLine();
                        break;
                }
                
                /*Console.WriteLine("  Attributes:");
                foreach (XAttribute attr in el.Attributes())
                    Console.WriteLine("attribute {0} ", attr);
                Console.WriteLine("  Elements:");

                foreach(XElement child in el.Descendants())
                {
                    Console.WriteLine("child localname: {0}, value: {1}", child.Name.LocalName, child.Value);
                    foreach (XAttribute child_attr in child.Attributes())
                        Console.WriteLine("child attribute name {0}: value{1}", child_attr.Name, child_attr.Value);
                }

                el.ToString();
                Console.ReadLine();
                */
            }
        }

        public XDocument ReadXMLFile()
        {
            XDocument xdoc = new XDocument();
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "XML Files|*.xml";

            if (of.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(of.FileName, true))
                    {
                        xdoc = XDocument.Load(streamReader);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            return xdoc;
        }

        #endregion
    }

    #region XBRL objects
    class xbrl_facts
    {
        public string concept;
        public string value;
        public string unit;
        public string dec;
        public DateTime start;
        public DateTime end;
        public string context_id;
        public context context;

        public xbrl_facts(string concept, string value, string unit, string dec)
        {
            this.concept = concept;
            this.value = value;
            this.unit = unit;
            this.dec = dec;
        }

        public void set_context(string context_id)
        {

        }
    }

    class context
    {
        public string id;
        public List<axis_member> members = new List<axis_member>();
        public bool instant = true;

        private DateTime start;
        public DateTime Start
        {
            get { return start; }
            set { start = Convert.ToDateTime(value); }
        }

        private DateTime end;
        public DateTime End
        {
            get { return end; }
            set { end = Convert.ToDateTime(value); instant = false; }
        }
        

        public context(string id)
        {
            this.id = id;
        }

        public void add_axis_member(string axis, string member)
        {
            members.Add(new axis_member(axis, member));
        }
        public void print()
        {
            Console.WriteLine("Context: {0}", this.id);
            if (instant) { Console.WriteLine("Instant Date: {0}", this.start.ToString("MM/dd/yyyy")); }
            else { Console.WriteLine("Duration Date: {0} - {1}", this.start.ToString("MM/dd/yyyy"), this.end.ToString("MM/dd/yyyy")); }
            foreach (var item in members)
            {
                Console.WriteLine("Axis: {0} Member: {1}", item.axis, item.member);
            }
        }
        
    }
    class axis_member
    {
        public string axis;
        public string member;

        public axis_member(string axis, string member)
        {
            this.axis = axis;
            this.member = member;
        }
    }

    #endregion
}