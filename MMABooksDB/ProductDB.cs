using System;
using System.Collections.Generic;
using System.Text;
using MMABooksTools;
using MMABooksProps;
using System.Data;

using DBBase = MMABooksTools.BaseSQLDB;
using DBConnection = MySql.Data.MySqlClient.MySqlConnection;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using DBParameter = MySql.Data.MySqlClient.MySqlParameter;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using DBDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using DBDbType = MySql.Data.MySqlClient.MySqlDbType;

namespace MMABooksDB
{
    public class ProductDB : DBBase, IReadDB, IWriteDB
    {
        public IBaseProps Create(IBaseProps p)
        {
            int rowsAffected = 0;
            ProductProps props = (ProductProps)p;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_ProductCreate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("prod_code", DBDbType.VarChar);         
            command.Parameters.Add("prod_description", DBDbType.VarChar);  
            command.Parameters.Add("unit_price", DBDbType.Decimal);       
            command.Parameters.Add("on_hand", DBDbType.Int32);           

            command.Parameters["prod_code"].Value = props.ProductCode;
            command.Parameters["prod_description"].Value = props.Description;
            command.Parameters["unit_price"].Value = props.UnitPrice;
            command.Parameters["on_hand"].Value = props.OnHandQuantity;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID = 1;
                    return props;
                }
                else
                    throw new Exception("Unable to insert record. " + props.ToString());
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }
        public bool Delete(IBaseProps p)
        {
            ProductProps props = (ProductProps)p;
            int rowsAffected = 0;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_ProductDelete";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("code", DBDbType.VarChar);
            command.Parameters.Add("conCurrId", DBDbType.Int32);
            command.Parameters["code"].Value = props.ProductCode;
            command.Parameters["conCurrId"].Value = props.ConcurrencyID;

            try
            {
                DBDataReader reader = RunProcedure(command);
                if (reader.Read())
                {
                    rowsAffected = reader.GetInt32(0);
                }

                if (rowsAffected == 1)
                {
                    return true;
                }
                else
                {
                    string message = "Record cannot be deleted. It has been edited by another user.";
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }

        public IBaseProps Retrieve(object key)
        {
            DBDataReader data = null;
            ProductProps props = new ProductProps();
            DBCommand command = new DBCommand();

            command.CommandText = "usp_ProductSelect";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("prod_code", DBDbType.VarChar);       
            command.Parameters["prod_code"].Value = key.ToString();

            try
            {
                data = RunProcedure(command);
                if (!data.IsClosed)
                {
                    if (data.Read())
                    {
                        props.SetState(data);
                    }
                    else
                        throw new Exception("Record does not exist in the database.");
                }
                return props;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (data != null)
                {
                    if (!data.IsClosed)
                        data.Close();
                }
            }
        }

        public object RetrieveAll()
        {
            List<ProductProps> list = new List<ProductProps>();
            DBDataReader reader = null;
            ProductProps props;

            try
            {
                reader = RunProcedure("usp_ProductSelectAll");
                if (!reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        props = new ProductProps();
                        props.SetState(reader);
                        list.Add(props);
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
        }


        public bool Update(IBaseProps p)
        {
            int rowsAffected = 0;
            ProductProps props = (ProductProps)p;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_ProductUpdate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("code", DBDbType.VarChar);
            command.Parameters.Add("name", DBDbType.VarChar);
            command.Parameters.Add("price", DBDbType.Decimal);
            command.Parameters.Add("qty", DBDbType.Int32);
            command.Parameters.Add("c", DBDbType.Int32);

            command.Parameters["code"].Value = props.ProductCode;
            command.Parameters["name"].Value = props.Description;
            command.Parameters["price"].Value = props.UnitPrice;
            command.Parameters["qty"].Value = props.OnHandQuantity;
            command.Parameters["c"].Value = props.ConcurrencyID;

            try
            {
                DBDataReader reader = RunProcedure(command);
                if (reader.Read())
                {
                    rowsAffected = reader.GetInt32(0);
                }

                if (rowsAffected == 1)
                {
                    props.ConcurrencyID++;
                    return true;
                }
                else
                {
                    string message = "Record cannot be updated. It has been edited by another user.";
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }
    }
}