using System;
using System.Collections.Generic;
using System.Text;
using MMABooksTools;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MMABooksProps
{
    [Serializable()]
    public class CustomerProps : IBaseProps
    {
        //properties for the customer 
        public int CustomerId { get; set; } = 0;
        public int CustomerID { get; set; }
        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string City { get; set; } = "";


        public string State { get; set; } = "";

        public string ZipCode { get; set; } = "";

       //ConcurrencyID. Dont manipulate directly


        public int ConcurrencyID { get; set; } = 0;

        

        public object Clone()
        {
            CustomerProps p = new CustomerProps();
            p.CustomerId = this.CustomerId;
            p.Name = this.Name;
            p.Address = this.Address;
            p.City = this.City;
            p.State = this.State;   
            p.ZipCode = this.ZipCode;
            p.ConcurrencyID = this.ConcurrencyID;
            return p;
        }

        public string GetState()
        {
            string jsonString;
            jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }

        public void SetState(string jsonString)
        {
            CustomerProps p = JsonSerializer.Deserialize<CustomerProps>(jsonString);
            this.CustomerId = p.CustomerId;
            this.Name = p.Name;
            this.Address = p.Address;
            this.City = p.City;
            this.State = p.State;
            this.ZipCode = p.ZipCode;
            this.ConcurrencyID = p.ConcurrencyID;
        }

        public void SetState(DBDataReader dr)
        {
            this.CustomerId = (int)dr["CustomerID"];
            this.Name = (string)dr["Name"];
            this.Address = (string)dr["Address"];
            this.City = (string)dr["City"];
            this.State = (string)dr["State"];
            this.ZipCode = (string)dr["ZipCode"];

            this.ConcurrencyID = (Int32)dr["ConcurrencyID"];
        }
    }
}
